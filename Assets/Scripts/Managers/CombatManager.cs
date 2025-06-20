using Assets.Scripts.Combat;
using Assets.Scripts.Controllers;
using Assets.Scripts.Data;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Habilidades;
using Assets.Scripts.Entities.Itens;
using Assets.Scripts.Entities.Missoes;
using Assets.Scripts.Helpers;
using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public class CombatManager : MonoBehaviour
    {

        public Transform playerPlaceholder;
        public Transform enemyPlaceholder;
        public CharacterPrefabDatabase characterPrefabDatabase;
        public CombatUI combatUI;
        public GameObject combatCanvas; // Assign in inspector
        public GameObject combatActorsRoot; // Assign in inspector

        public static Inimigo EnemyToRemove;

        private Personagem jogador;
        private Inimigo inimigo;
        private SistemaCombate sistemaCombate;

        private GameObject playerInstance;
        private GameObject enemyInstance;

        private Animator playerAnimator;
        private Animator enemyAnimator;

        private void Start()
        {
            jogador = GameManager.Instance.Player;
            inimigo = GameManager.Instance.CombateAtual.InimigoAtual;

            // Instantiate player/enemy at combat positions
            playerInstance = CharacterFactory.InstantiatePlayer(jogador, characterPrefabDatabase,  playerPlaceholder, true);
            enemyInstance = CharacterFactory.InstantiateEnemy(inimigo, enemyPlaceholder, true);

            InitializeCombat();
        }

        public void InitializeCombat()
        {
            playerAnimator = playerInstance.GetComponent<Animator>();
            enemyAnimator = enemyInstance.GetComponent<Animator>();

            sistemaCombate = new SistemaCombate(jogador, inimigo);
            sistemaCombate.OnStateChanged += OnCombatStateChanged;

            SetupUI();
            BlockPlayerMovement(true);

            combatUI.SetButtonsInteractable(false, false, false, false);

            combatUI.ShowCombatLog($"Um {inimigo.Nome} quer brigar", OnIntroAdvance, true);
            UpdateUI();
        }

        private void EndCombat()
        {
            BlockPlayerMovement(false);
            GameManager.Instance.ChangeState(GameState.InGame);
        }
        private void SetupUI()
        {
            combatUI.ClearCombatLog();
            combatUI.SetPlayerInfo(jogador);
            combatUI.SetEnemyInfo(inimigo);

            combatUI.attackButton.onClick.RemoveAllListeners();
            combatUI.skillButton.onClick.RemoveAllListeners();
            combatUI.potionButton.onClick.RemoveAllListeners();
            combatUI.runButton.onClick.RemoveAllListeners();

            combatUI.attackButton.onClick.AddListener(OnAttack);
            combatUI.skillButton.onClick.AddListener(OnSkill);
            combatUI.potionButton.onClick.AddListener(OnPotion);
            combatUI.runButton.onClick.AddListener(OnRun);
        }

        private void UpdateUI()
        {
            combatUI.SetPlayerInfo(jogador);
            combatUI.SetEnemyInfo(inimigo);
        }

        private void OnCombatStateChanged(EstadoCombate newState)
        {
            UpdateUI();
        }

        // --- LOGIC FLOW ---

        private void OnIntroAdvance()
        {
            if (sistemaCombate.Estado == EstadoCombate.TurnoJogador)
            {
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    combatUI.ShowCombatLog($"O que {jogador.Nome} vai fazer?", null, false);
                    combatUI.SetButtonsInteractable(true, true, true, true);
                    UpdateUI();
                }, $"Turno de {jogador.Nome}"));
            }
            else
            {
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    StartCoroutine(ShowLogAndTimerThen(OnEnemyAttack, $"{inimigo.Nome} ataca!"));
                }, $"Turno de {inimigo.Nome}"));
            }
            UpdateUI();
        }

        // --- PLAYER ACTIONS ---

        private void OnAttack()
        {
            combatUI.SetButtonsInteractable(false, false, false, false);

            // Show confirmation log, then after timer, resolve the action
            StartCoroutine(ShowLogAndTimerThen(() =>
            {
                playerAnimator?.SetTrigger("attack");
                string result = sistemaCombate.JogadorAtacar();
                UpdateUI();
                if (result.Contains("causando"))
                {
                    int dano = ParseDano(result);
                    // Show result log, then after timer, continue
                    StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, $"O ataque acertou, causando {dano} pontos de dano."));
                }
                else
                {
                    StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, "O ataque errou!"));
                }
            }, $"{jogador.Nome} atacou."));
        }

        private void OnSkill()
        {
            combatUI.ShowSkillSelectionPanel(jogador.Habilidades, habilidade =>
            {
                combatUI.SetButtonsInteractable(false, false, false, false);
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    string result;
                    if (habilidade.CustoMana > jogador.ManaAtual)
                    {
                        StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, "Mana insuficiente para usar a habilidade."));
                    }
                    else if (habilidade.GetType().Equals(typeof(HabilidadeDano)))
                    {
                        result = sistemaCombate.JogadorHabilidadeDano(habilidade);
                        UpdateUI();
                        int dano = habilidade.Efeito;
                        StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, $"A habilidade causou {dano} de dano."));
                    }
                    else
                    {
                        result = sistemaCombate.JogadorHabilidadeCura(habilidade);
                        UpdateUI();
                        int cura = habilidade.Efeito;
                        StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, $"A habilidade curou {cura} de vida."));
                    }
                }, $"{jogador.Nome} usou {habilidade.Nome}."));
            });

            combatUI.skillBackButton.onClick.RemoveAllListeners();
            combatUI.skillBackButton.onClick.AddListener(() =>
            {
                combatUI.skillSelectionPanel.SetActive(false);
                combatUI.SetButtonsInteractable(true, true, true, true);
            });
        }

        private void OnPotion()
        {
            var grouped = new Dictionary<string, (Pocao pocao, int count)>();
            foreach (var item in jogador.Inventario.Itens)
            {
                if (item is Pocao pocao)
                {
                    if (grouped.ContainsKey(pocao.Nome))
                        grouped[pocao.Nome] = (grouped[pocao.Nome].pocao, grouped[pocao.Nome].count + 1);
                    else
                        grouped[pocao.Nome] = (pocao, 1);
                }
            }

            var uniquePocoes = new List<Pocao>();
            var counts = new Dictionary<string, int>();
            foreach (var kvp in grouped)
            {
                uniquePocoes.Add(kvp.Value.pocao);
                counts[kvp.Key] = kvp.Value.count;
            }

            combatUI.ShowPotionSelection(uniquePocoes, pocao =>
            {
                combatUI.SetButtonsInteractable(false, false, false, false);
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    sistemaCombate.JogadorUsarPocao(pocao);
                    UpdateUI();
                    int cura = pocao.HealingAmount;
                    StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, $"A pocao curou {cura} de vida"));
                }, $"{jogador.Nome} tomou uma pocao"));
            }, counts);

            combatUI.potionBackButton.onClick.RemoveAllListeners();
            combatUI.potionBackButton.onClick.AddListener(() =>
            {
                combatUI.potionSelectionPanel.SetActive(false);
                combatUI.SetButtonsInteractable(true, true, true, true);
            });
        }

        private void OnRun()
        {
            combatUI.SetButtonsInteractable(false, false, false, false);
            StartCoroutine(ShowLogAndTimerThen(() =>
            {
                string result = sistemaCombate.Run();
                UpdateUI();
                bool sucesso = result.Contains("conseguiu fugir");
                if (sucesso)
                {
                    StartCoroutine(ShowLogAndTimerThen(OnVictoryOrDefeat, "Voce fugiu"));
                }
                else
                {
                    StartCoroutine(ShowLogAndTimerThen(OnPlayerActionResult, "Nao foi possivel fugir"));
                }
            }, $"{jogador.Nome} esta tentando fugir"));
        }

        private void OnPlayerActionResult()
        {
            UpdateUI();
            if (sistemaCombate.Estado == EstadoCombate.TurnoJogador)
            {
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    combatUI.ShowCombatLog($"O que {jogador.Nome} vai fazer?", null, false);
                    combatUI.SetButtonsInteractable(true, true, true, true);
                    UpdateUI();
                }, $"Turno de {jogador.Nome}"));
            }
            else if (sistemaCombate.Estado == EstadoCombate.TurnoInimigo)
            {
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    StartCoroutine(ShowLogAndTimerThen(OnEnemyAttack, $"{inimigo.Nome} ataca!"));
                }, $"Turno de {inimigo.Nome}"));
            }
            else
            {
                OnVictoryOrDefeat();
            }
        }

        // --- ENEMY ACTIONS ---

        private void OnEnemyAttack()
        {
            enemyAnimator?.SetTrigger("attack");
            string result = sistemaCombate.InimigoAtacar();
            UpdateUI();
            if (result.Contains("causando"))
            {
                int dano = ParseDano(result);
                StartCoroutine(ShowLogAndTimerThen(OnEnemyActionResult, $"{inimigo.Nome} atacou, causando {dano} pontos de dano"));
            }
            else
            {
                StartCoroutine(ShowLogAndTimerThen(OnEnemyActionResult, $"{inimigo.Nome} errou o ataque!"));
            }
        }

        private void OnEnemyActionResult()
        {
            UpdateUI();
            if (sistemaCombate.Estado == EstadoCombate.TurnoJogador)
            {
                StartCoroutine(ShowLogAndTimerThen(() =>
                {
                    combatUI.ShowCombatLog($"O que {jogador.Nome} vai fazer?", null, false);
                    combatUI.SetButtonsInteractable(true, true, true, true);
                    UpdateUI();
                }, $"Turno de {jogador.Nome}"));
            }
            else
            {
                OnVictoryOrDefeat();
            }
        }

        // --- END OF COMBAT ---

        private void OnVictoryOrDefeat()
        {
            UpdateUI();
            if (sistemaCombate.Estado == EstadoCombate.Vitoria)
            {
                StartCoroutine(VictorySequence());
            }
            else if (sistemaCombate.Estado == EstadoCombate.Derrota)
            {
                StartCoroutine(GameOverSequence());
            }
            else
            {
                StartCoroutine(EscapeSequence());
            }
        }

        private IEnumerator VictorySequence()
        {
            combatUI.ShowCombatLog($"{jogador.Nome} venceu a batalha!", null, false);
            yield return new WaitForSeconds(1.2f);

            GameManager.Instance.Player.GanharExperiencia(inimigo.XP);
            combatUI.ShowCombatLog($"{jogador.Nome} recebeu {inimigo.XP} pontos de experiencia.", null, false);
            yield return new WaitForSeconds(1.2f);

            if (inimigo.Recompensas != null)
            {
                foreach (var item in inimigo.Recompensas)
                {
                    var rewardItem = DataLoader.CloneItem(item);
                    if (GameManager.Instance.Player.Inventario.AdicionarItem(rewardItem))
                    {
                        AtualizarMissoesDeColeta(rewardItem);
                        combatUI.ShowCombatLog($"{jogador.Nome} coletou {item.Nome}.", null, false);
                        yield return new WaitForSeconds(1.2f);
                    }
                    else
                    {
                        combatUI.ShowCombatLog($"{jogador.Nome} nao pode coletar {item.Nome} (inventario cheio).", null, false);
                        yield return new WaitForSeconds(1.2f);
                    }
                }
            }

            EnemyToRemove = inimigo;

            AtualizarMissoesDeExterminio();

            inimigo.VidaAtual = inimigo.VidaMaxima;

            EndCombat();
        }

        private IEnumerator GameOverSequence()
        {
            combatUI.ShowCombatLog($"{jogador.Nome} foi derrotado! Game Over.", null, true);
            yield return new WaitForSeconds(1.8f);

            GameManager.Instance.LoadGame();

            inimigo.VidaAtual = inimigo.VidaMaxima;

            EndCombat();
        }

        private IEnumerator EscapeSequence()
        {
            yield return new WaitForSeconds(1.2f);

            inimigo.VidaAtual = inimigo.VidaMaxima;

            EndCombat();
        }

        // --- UTILS ---

        private int ParseDano(string result)
        {
            var parts = result.Split(' ');
            for (int i = 0; i < parts.Length; i++)
            {
                if ((parts[i] == "causando" || parts[i] == "curou") && i + 1 < parts.Length)
                {
                    if (int.TryParse(parts[i + 1], out int value))
                        return value;
                }
            }
            return 0;
        }

        private void BlockPlayerMovement(bool block)
        {
            PlayerController.IsMovementBlocked = block;
        }

        // --- TIMER LOG UTILITY ---

        private IEnumerator ShowLogAndTimerThen(System.Action next, string message, float time = 1.2f)
        {
            combatUI.ShowCombatLog(message, null, false);
            yield return new WaitForSeconds(time);
            next?.Invoke();
        }
        private void AtualizarMissoesDeColeta(Item item)
        {
            foreach (var missao in jogador.Missoes)
            {
                if (missao is MissaoColeta missaoColeta)
                    missaoColeta.Coletou(item);
            }
        }

        private void AtualizarMissoesDeExterminio()
        {
            foreach (var missao in jogador.Missoes)
            {
                if (missao is MissaoExterminio missaoExterminio)
                missaoExterminio.Derrotou(inimigo);
            }
        }
    }
}