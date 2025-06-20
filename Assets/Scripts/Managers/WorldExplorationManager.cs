using Assets.Scripts.Controllers;
using Assets.Scripts.Data;
using Assets.Scripts.Entities;
using Assets.Scripts.Helpers;
using Assets.Scripts.World;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers
{
    public class WorldExplorationManager : MonoBehaviour
    {
        public static WorldExplorationManager Instance { get; private set; }

        public Transform npcPlaceholder;
        public Transform[] enemyPlaceholders; 
        public Transform playerPlaceholder;
        public CharacterPrefabDatabase characterPrefabDatabase;
        public GameObject worldActorsRoot; 

        private Area currentArea;
        private Personagem jogador;
        private static Dictionary<string, HashSet<string>> defeatedEnemiesByArea = new Dictionary<string, HashSet<string>>();
        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            jogador = GameManager.Instance.Player;
            currentArea = GameManager.Instance.CurrentArea ?? GameManager.Instance.Mundo.Areas[0];
            GameManager.Instance.CurrentArea = currentArea;

            SpawnAreaContent(currentArea);

            if (SceneTransition.PlayerWorldPosition != Vector3.zero)
            {
                InstantiatePlayerAtPosition(SceneTransition.PlayerWorldPosition, worldActorsRoot.transform);
            }
            else
            {
                InstantiatePlayerAtPosition(playerPlaceholder.position, worldActorsRoot.transform);
                SceneTransition.AreaPlayerPositions[currentArea.Nome] = playerPlaceholder.position;
            }

            SceneTransition.PlayerWorldPosition = Vector3.zero;

            if (SceneTransition.EnemyToFight != null && currentArea.Inimigos.Contains(SceneTransition.EnemyToFight))
            {
                currentArea.Inimigos.Remove(SceneTransition.EnemyToFight);
                SceneTransition.EnemyToFight = null;
            }
        }

        public void OnAreaChanged(Area newArea)
        {
            currentArea = newArea;
            var keys = defeatedEnemiesByArea.Keys.ToList();

            foreach (var key in keys)
            {
                if (key != currentArea.Nome && defeatedEnemiesByArea[key].Count > 0)
                    defeatedEnemiesByArea.Remove(key);
            }

            ClearAreaContent();
            SpawnAreaContent(currentArea);
        }

        private void SpawnAreaContent(Area area)
        {
            foreach (var npc in area.NPCs)
                CharacterFactory.InstantiateNPC(npc, npcPlaceholder);

            if (!defeatedEnemiesByArea.ContainsKey(area.Nome))
                defeatedEnemiesByArea[area.Nome] = new HashSet<string>();

            var defeatedEnemies = defeatedEnemiesByArea[area.Nome];

            for (int i = 0; i < area.Inimigos.Count; i++)
            {
                var enemy = area.Inimigos[i];

                if (defeatedEnemies.Contains(enemy.Nome))
                    continue;

                var placeholder = enemyPlaceholders.Length > 0 ? enemyPlaceholders[i % enemyPlaceholders.Length] : null;
                if (placeholder != null)
                    CharacterFactory.InstantiateEnemy(enemy, placeholder, false);
            }

            if (CombatManager.EnemyToRemove != null)
                RemoveEnemyFromScene(CombatManager.EnemyToRemove);
        }

        private void ClearAreaContent()
        {
            foreach (Transform child in npcPlaceholder.parent)
                if (child != npcPlaceholder) Destroy(child.gameObject);

            // Clear all enemy placeholders' siblings except the placeholders themselves
            foreach (var placeholder in enemyPlaceholders)
            {
                if (placeholder == null) continue;
                foreach (Transform child in placeholder.parent)
                {
                    if (child != placeholder && !System.Array.Exists(enemyPlaceholders, p => p == child))
                        Destroy(child.gameObject);
                }
            }
        }

        public GameObject InstantiatePlayerAtPosition(Vector3 position, Transform parent)
        {
            return CharacterFactory.InstantiatePlayerAtPosition(jogador, characterPrefabDatabase, position, parent, false);
        }

        public void SetWorldActive(bool active)
        {
            worldActorsRoot.SetActive(active);
        }

        public Area GetCurrentArea() => currentArea;
        public Personagem GetJogador() => jogador;

        public void RemoveEnemyFromScene(Inimigo inimigo)
        {
            // Remove enemy GameObject from the world by matching by name
            var enemyObjects = GameObject.FindObjectsByType<EnemyController>(FindObjectsSortMode.InstanceID);

            foreach (var enemyObj in enemyObjects)
            {
                if (enemyObj.EnemyData != null && enemyObj.EnemyData.Nome == inimigo.Nome)
                {
                    Destroy(enemyObj.gameObject);
                    CombatManager.EnemyToRemove = null;
                    break;
                }
            }

            if(!defeatedEnemiesByArea.ContainsKey(currentArea.Nome))
                defeatedEnemiesByArea[currentArea.Nome] = new HashSet<string>();

            defeatedEnemiesByArea[currentArea.Nome].Add(inimigo.Nome);

            // Remove from area if PermanentDeath
            if (inimigo.PermanentDeath && currentArea.Inimigos.Exists(e => e.Nome == inimigo.Nome))
            {
                currentArea.Inimigos.RemoveAll(e => e.Nome == inimigo.Nome);
                GameManager.Instance.SaveGame();
            }
        }
    }
}