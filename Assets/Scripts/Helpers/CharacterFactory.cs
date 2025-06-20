using Assets.Scripts.Controllers;
using Assets.Scripts.Data;
using Assets.Scripts.Entities;
using Assets.Scripts.UI;
using Assets.Scripts.World;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helpers
{
    public static class CharacterFactory
    {
        public static GameObject InstantiateNPC(NPC npc, Transform placeholder)
        {
            var prefab = Resources.Load<GameObject>($"NPCs/{npc.Nome}");
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab de NPC não encontrado: {npc.Nome}");
                return null;
            }

            var npcObj = Object.Instantiate(prefab, placeholder.position, placeholder.rotation, placeholder.parent);

            if (npcObj.TryGetComponent<NPCController>(out var controller))
            {
                controller.Setup(npc);
                controller.dialogueUI = DialogueUI.Instance;
            }
            else
            {
                Debug.LogWarning($"NPCController não encontrado no prefab {npc.Nome}");
            }
            return npcObj;
        }

        public static GameObject InstantiateEnemy(Inimigo enemy, Transform placeholder, bool forCombat = false)
        {
            var prefab = Resources.Load<GameObject>($"Enemies/{enemy.Nome}");
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab de Inimigo não encontrado: {enemy.Nome}");
                return null;
            }

            var enemyObj = Object.Instantiate(prefab, placeholder.position, placeholder.rotation, placeholder.parent);

            var placeholderRenderer = placeholder.GetComponent<SpriteRenderer>();
            var objRenderer = enemyObj.GetComponent<SpriteRenderer>();
            if (placeholderRenderer != null && objRenderer != null)
            {
                objRenderer.sortingOrder = placeholderRenderer.sortingOrder;
            }

            enemyObj.transform.localScale = placeholder.transform.localScale;

            // Enable/disable scripts based on context
            var enemyController = enemyObj.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.enabled = !forCombat; // Enable only in world
                if (!forCombat)
                {
                    enemyController.Setup(enemy); // <-- Ensure enemyData is set for world enemies
                }
            }

            var combatAnim = enemyObj.GetComponent<CombatAnimationController>();
            if (combatAnim != null)
                combatAnim.enabled = forCombat; // Enable only in combat

            return enemyObj;
        }

        public static GameObject InstantiatePlayer(Personagem jogador, CharacterPrefabDatabase database, Transform placeholder, bool forCombat = false)
        {
            var prefab = database.GetPrefab(jogador.Raca.Nome, jogador.Classe.Nome);

            if (prefab == null)
            {
                Debug.LogError($"Prefab não encontrado para Raça: {jogador.Raca.Nome}, Classe: {jogador.Classe.Nome}");
                return null;
            }

            var playerObj = Object.Instantiate(prefab, placeholder.position, placeholder.rotation, placeholder.parent);

            var placeholderRenderer = placeholder.GetComponent<SpriteRenderer>();
            var objRenderer = playerObj.GetComponent<SpriteRenderer>();
            if (placeholderRenderer != null && objRenderer != null)
            {
                objRenderer.sortingOrder = placeholderRenderer.sortingOrder;
            }

            playerObj.transform.localScale = placeholder.transform.localScale;

            // Enable/disable scripts based on context
            var playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.enabled = !forCombat; // Enable only in world

            var combatAnim = playerObj.GetComponent<CombatAnimationController>();
            if (combatAnim != null)
                combatAnim.enabled = forCombat; // Enable only in combat

            return playerObj;
        }

        public static GameObject InstantiatePlayerAtPosition(Personagem jogador, CharacterPrefabDatabase database, Vector3 position, Transform parent, bool forCombat = false)
        {
            var prefab = database.GetPrefab(jogador.Raca.Nome, jogador.Classe.Nome);
            if (prefab == null)
            {
                Debug.LogError($"Prefab não encontrado para Raça: {jogador.Raca.Nome}, Classe: {jogador.Classe.Nome}");
                return null;
            }
            var playerObj = Object.Instantiate(prefab, position, Quaternion.identity, parent);

            var playerController = playerObj.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.enabled = !forCombat;

            var combatAnim = playerObj.GetComponent<CombatAnimationController>();
            if (combatAnim != null)
                combatAnim.enabled = forCombat;

            return playerObj;
        }

        public static GameObject InstantiateEnemyAtPosition(Inimigo enemy, Vector3 position, Transform parent, bool forCombat = false)
        {
            var prefab = Resources.Load<GameObject>($"Enemies/{enemy.Nome}");
            if (prefab == null)
            {
                Debug.LogWarning($"Prefab de Inimigo não encontrado: {enemy.Nome}");
                return null;
            }
            var enemyObj = Object.Instantiate(prefab, position, Quaternion.identity, parent);

            var enemyController = enemyObj.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.enabled = !forCombat;
                enemyController.Setup(enemy);
            }

            var combatAnim = enemyObj.GetComponent<CombatAnimationController>();
            if (combatAnim != null)
                combatAnim.enabled = forCombat;

            return enemyObj;
        }
    }
}