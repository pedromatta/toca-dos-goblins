using Assets.Scripts.Combat;
using Assets.Scripts.Entities;
using Assets.Scripts.World;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using Assets.Scripts.Data;
using Assets.Scripts.Helpers;
using Assets.Scripts.Controllers;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Managers
{
    public enum GameState
    {
        MainMenu,
        InGame,
        Combat,
        CharacterCreation,
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; }
        public Personagem Player;
        public SistemaCombate CombateAtual;

        public Dictionary<string, Item> Items;
        public Dictionary<string, Habilidade> Habilidades;
        public Dictionary<string, Raca> Racas;
        public Dictionary<string, Classe> Classes;
        public Dictionary<string, Inimigo> Inimigos;
        public Dictionary<string, NPC> NPCs;
        public Dictionary<string, Missao> Missoes;

        public Mapa Mundo;
        public Area CurrentArea;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            Debug.LogWarning("GameManager started. Loading game data and initializing state.");
            LoadGameData();
            ChangeState(GameState.MainMenu);
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            switch(newState)
            {
                case GameState.MainMenu:
                    SceneManager.LoadScene("MainMenu");
                    break;
                case GameState.InGame:
                    SceneManager.LoadScene(CurrentArea.Nome);
                    break;
                case GameState.Combat:
                    SceneManager.LoadScene("CombatScene");
                    break;
                case GameState.CharacterCreation:
                    SceneManager.LoadScene("CharacterCreation");
                    break;
            }
        }

        public void ChangeArea(string areaName)
        {
            if (Mundo == null || Mundo.Areas == null || Mundo.Areas.Count == 0)
            {
                Debug.LogError("Mundo ou áreas não carregados");
                return;
            }

            // Save current position for the current area before changing
            if (CurrentArea != null)
            {
                var playerObj = FindFirstObjectByType<PlayerController>();
                if (playerObj != null)
                {
                    SceneTransition.AreaPlayerPositions[CurrentArea.Nome] = playerObj.transform.position;
                }
            }

            CurrentArea = Mundo.Areas.Find(a => a.Nome == areaName);
            if (CurrentArea == null)
            {
                Debug.LogError($"Area '{areaName}' não foi encontrada");
                return;
            }

            // Set the next position: if we have a saved position for this area, use it; else, use Vector3.zero (handled in WorldExplorationManager)
            if (SceneTransition.AreaPlayerPositions.TryGetValue(CurrentArea.Nome, out var savedPos))
                SceneTransition.PlayerWorldPosition = savedPos;
            else
                SceneTransition.PlayerWorldPosition = Vector3.zero;

            SceneManager.LoadScene(CurrentArea.Nome);

            if (CurrentState != GameState.InGame)
                CurrentState = GameState.InGame;

            WorldExplorationManager.Instance.OnAreaChanged(CurrentArea);
        }

        private void LoadGameData()
        {
            Items = DataLoader.LoadItems();
            Habilidades = DataLoader.LoadHabilidades();
            Classes = DataLoader.LoadClasses(Items, Habilidades);
            Racas = DataLoader.LoadRacas(Habilidades);
            Inimigos = DataLoader.LoadInimigos(Items);
            Missoes = DataLoader.LoadMissoes(Items, Inimigos);
            NPCs = DataLoader.LoadNPCs(Missoes);
            var Areas = DataLoader.LoadAreas(NPCs, Inimigos);
            Mundo = DataLoader.LoadMapa(Areas);
        }
        public void StartCombat(Inimigo inimigo, Vector3 playerPosition)
        {
            CombateAtual = new SistemaCombate(Player, inimigo);
            SceneTransition.PlayerWorldPosition = playerPosition;
            ChangeState(GameState.Combat);
        }

        public void SaveGame()
        {
            var saveData = new GameSaveData
            {
                personagem = DataLoader.ToJsonPersonagem(Player),
                currentAreaName = CurrentArea?.Nome,
                areas = Mundo.Areas.Select(area => new AreaSaveData
                {
                    nome = area.Nome,
                    inimigos = area.Inimigos.Select(i => new InimigoSaveData
                    {
                        id = area.Nome + "_" + i.Nome,
                        nome = i.Nome,
                        morto = i.VidaAtual <= 0
                    }).ToList(),
                    npcs = area.NPCs.Select(n => new NPCSaveData
                    {
                        nome = n.Nome,
                        missaoConcluida = n.MissaoDisponivel?.Concluida ?? false
                    }).ToList()
                }).ToList()
            };

            string json = JsonUtility.ToJson(saveData, true);

#if UNITY_WEBGL && !UNITY_EDITOR
            PlayerPrefs.SetString("savegame", json);
            PlayerPrefs.Save();
            Debug.Log("Game saved to PlayerPrefs (WebGL).");
#else
            string path = Path.Combine(Application.persistentDataPath, "savegame.json");
            File.WriteAllText(path, json);
            Debug.Log("Game saved to: " + path);
#endif
        }

        public void LoadGame()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!PlayerPrefs.HasKey("savegame"))
            {
                Debug.LogWarning("No save file found in PlayerPrefs (WebGL).");
                return;
            }
            string json = PlayerPrefs.GetString("savegame");
#else
            string path = Path.Combine(Application.persistentDataPath, "savegame.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning("No save file found.");
                return;
            }
            string json = File.ReadAllText(path);
#endif
            var saveData = JsonUtility.FromJson<GameSaveData>(json);

            // Load personagem
            Player = DataLoader.FromJsonPersonagem(saveData.personagem, Classes, Racas, Habilidades, Items, Missoes);

            // Restore world state
            foreach (var areaSave in saveData.areas)
            {
                var area = Mundo.Areas.FirstOrDefault(a => a.Nome == areaSave.nome);
                if (area == null) continue;

                foreach (var inimigoSave in areaSave.inimigos)
                {
                    var inimigo = area.Inimigos.FirstOrDefault(i => (area.Nome + "_" + i.Nome) == inimigoSave.id);
                    if (inimigo != null)
                    {
                        if(inimigoSave.morto && inimigo.PermanentDeath)
                        {
                            area.Inimigos.Remove(inimigo);
                        }
                        else
                        {
                            inimigo.VidaAtual = inimigo.VidaMaxima;
                        }
                    }
                }

                foreach (var npcSave in areaSave.npcs)
                {
                    var npc = area.NPCs.FirstOrDefault(n => n.Nome == npcSave.nome);
                    if (npc != null && npc.MissaoDisponivel != null)
                    {
                        npc.MissaoDisponivel.Progresso = npcSave.missaoProgresso;
                        npc.MissaoDisponivel.Concluida = npcSave.missaoConcluida;
                    }
                }
            }

            // Restore current area
            CurrentArea = Mundo.Areas.FirstOrDefault(a => a.Nome == saveData.currentAreaName);
        }
    }
}
