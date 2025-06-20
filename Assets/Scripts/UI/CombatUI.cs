using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Habilidades;
using Assets.Scripts.Entities.Itens;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class CombatUI : MonoBehaviour
    {
        public TMP_Text playerNameText;
        public TMP_Text enemyNameText;
        public TMP_Text combatLogText;

        public Slider playerHPSlider;
        public Slider playerManaSlider;
        public Slider enemyHpSlider;

        public Button attackButton;
        public Button skillButton;
        public Button potionButton;
        public Button runButton;

        public GameObject skillSelectionPanel;
        public GameObject potionSelectionPanel;
        public Transform skillListContainer;
        public Transform potionListContainer;
        public TMP_Text skillDescriptionBox;
        public TMP_Text potionDescriptionBox;
        public Button skillBackButton;
        public Button potionBackButton;
        public GameObject skillButtonPrefab;
        public GameObject potionButtonPrefab;

        private Queue<(string, Action)> logQueue = new Queue<(string, Action)>();
        private bool waitingForInput = true;
        private InputAction advanceLogAction;
        public Button nextLogButton;

        private void Awake()
        {
            waitingForInput = false;
            nextLogButton.onClick.RemoveAllListeners();
            nextLogButton.onClick.AddListener(TryAdvanceLog);
            advanceLogAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/enter");
            advanceLogAction.AddBinding("<Keyboard>/numpadEnter");
            advanceLogAction.performed += ctx => TryAdvanceLog();
            advanceLogAction.Enable();
        }

        private void OnDestroy()
        {
            advanceLogAction?.Dispose();
        }

        /// <summary>
        /// Show a log message. If waitForInput is true, the log will wait for Enter to advance.
        /// If false, the log will show and immediately call onAdvance.
        /// </summary>
        public void ShowCombatLog(string message, Action onAdvance = null, bool waitForInput = true)
        {
            // If nothing is being shown, show immediately
            if (!waitingForInput && logQueue.Count == 0)
            {
                combatLogText.text = message;
                waitingForInput = waitForInput;
                if (!waitForInput)
                {
                    onAdvance?.Invoke();
                }
                else
                {
                    StartCoroutine(WaitForInputThenAdvance(onAdvance));
                }
            }
            else
            {
                logQueue.Enqueue((message, onAdvance));
            }
        }

        private void TryAdvanceLog()
        {
            if (waitingForInput && !skillSelectionPanel.activeSelf && !potionSelectionPanel.activeSelf)
            {
                AdvanceLog(true);
            }
        }

        /// <summary>
        /// Advances the log. If waitForInput is true, waits for Enter before advancing.
        /// </summary>
        private void AdvanceLog(bool waitForInput)
        {
            if (logQueue.Count > 0)
            {
                var (msg, onAdvance) = logQueue.Dequeue();
                combatLogText.text = msg;
                waitingForInput = waitForInput;

                if (!waitForInput)
                {
                    onAdvance?.Invoke();
                    AdvanceLog(false);
                }
                else
                {
                    StartCoroutine(WaitForInputThenAdvance(onAdvance));
                }
            }
            else
            {
                // Do not clear the log here!
                waitingForInput = false;
            }
        }

        private System.Collections.IEnumerator WaitForInputThenAdvance(Action onAdvance)
        {
            // Wait until Enter is pressed (handled by TryAdvanceLog)
            while (waitingForInput)
                yield return null;
            onAdvance?.Invoke();
        }

        public void SetPlayerInfo(Personagem player)
        {
            playerNameText.text = player.Nome;
            playerHPSlider.maxValue = player.VidaMaxima;
            playerHPSlider.value = player.VidaAtual;
            playerHPSlider.GetComponentInChildren<TMP_Text>().text = $"HP: {player.VidaAtual}/{player.VidaMaxima}";
            playerManaSlider.maxValue = player.ManaMaxima;
            playerManaSlider.value = player.ManaAtual;
            playerManaSlider.GetComponentInChildren<TMP_Text>().text = $"Mana: {player.ManaAtual}/{player.ManaMaxima}";
        }

        public void SetEnemyInfo(Inimigo enemy)
        {
            enemyNameText.text = enemy.Nome;
            enemyHpSlider.maxValue = enemy.VidaMaxima;
            enemyHpSlider.value = enemy.VidaAtual;
        }

        public void SetCombatLog(string message)
        {
            combatLogText.text = message;
        }

        public void SetButtonsInteractable(bool attack, bool skill, bool potion, bool run)
        {
            attackButton.interactable = attack;
            skillButton.interactable = skill;
            potionButton.interactable = potion;
            runButton.interactable = run;
        }

        public void ShowSkillSelectionPanel(List<Habilidade> habilidades, Action<Habilidade> onSkillSelected)
        {
            skillSelectionPanel.SetActive(true);
            foreach (Transform child in skillListContainer)
                Destroy(child.gameObject);

            if (habilidades != null)
            {
                foreach (var habilidade in habilidades)
                {
                    var btnObj = Instantiate(skillButtonPrefab, skillListContainer);
                    var btn = btnObj.GetComponent<Button>();
                    btnObj.GetComponentInChildren<TMP_Text>().text = habilidade.Nome;
                    btn.onClick.AddListener(() =>
                    {
                        skillSelectionPanel.SetActive(false);
                        onSkillSelected?.Invoke(habilidade);
                    });

                    var trigger = btnObj.AddComponent<EventTrigger>();
                    var entryEnter = new EventTrigger.Entry
                    {
                        eventID = EventTriggerType.PointerEnter
                    };
                    if(habilidade is HabilidadeCura)
                        entryEnter.callback.AddListener((_) => skillDescriptionBox.text = $"{habilidade.Nome} - {habilidade.CustoMana}PM\nCura: {habilidade.Efeito}\n{habilidade.Descricao}");
                    else
                        entryEnter.callback.AddListener((_) => skillDescriptionBox.text = $"{habilidade.Nome} - {habilidade.CustoMana}PM\nDano: {habilidade.Efeito}\n{habilidade.Descricao}");
                    trigger.triggers.Add(entryEnter);

                    var entryExit = new EventTrigger.Entry
                    {
                        eventID = EventTriggerType.PointerExit
                    };
                    entryExit.callback.AddListener((_) => skillDescriptionBox.text = string.Empty);
                    trigger.triggers.Add(entryExit);
                }
            }
            skillBackButton.onClick.RemoveAllListeners();
            skillBackButton.onClick.AddListener(() => { skillSelectionPanel.SetActive(false); });
        }

        public void ShowPotionSelection(List<Pocao> pocoes, Action<Pocao> onPotionSelected, Dictionary<string, int> counts)
        {
            potionSelectionPanel.SetActive(true);
            foreach (Transform child in potionListContainer)
                Destroy(child.gameObject);

            if (pocoes != null)
            {
                foreach (var pocao in pocoes)
                {
                    var btnObj = Instantiate(potionButtonPrefab, potionListContainer);
                    var btn = btnObj.GetComponent<Button>();
                    int count = counts.TryGetValue(pocao.Nome, out var c) ? c : 1;
                    btnObj.GetComponentInChildren<TMP_Text>().text = $"{pocao.Nome} x{count}";
                    btn.onClick.AddListener(() =>
                    {
                        potionSelectionPanel.SetActive(false);
                        onPotionSelected?.Invoke(pocao);
                    });

                    var trigger = btnObj.AddComponent<EventTrigger>();
                    var entryEnter = new EventTrigger.Entry
                    {
                        eventID = EventTriggerType.PointerEnter
                    };
                    entryEnter.callback.AddListener((_) => potionDescriptionBox.text = $"{pocao.Nome}\nCura: {pocao.HealingAmount}\n{pocao.Descricao}");
                    trigger.triggers.Add(entryEnter);

                    var entryExit = new EventTrigger.Entry
                    {
                        eventID = EventTriggerType.PointerExit
                    };
                    entryExit.callback.AddListener((_) => potionDescriptionBox.text = string.Empty);
                    trigger.triggers.Add(entryExit);
                }
            }
            potionBackButton.onClick.RemoveAllListeners();
            potionBackButton.onClick.AddListener(() => { potionSelectionPanel.SetActive(false); });
        }

        public void ClearCombatLog()
        {
            combatLogText.text = string.Empty;
        }
    }
}