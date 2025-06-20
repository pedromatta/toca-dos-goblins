using Assets.Scripts.Entities;
using Assets.Scripts.Managers;
using Assets.Scripts.UI;
using Assets.Scripts.World;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class NPCController : MonoBehaviour
    {
        private NPC npcData;
        public NPC NpcData => npcData;

        public GameObject interactIcon;
        private Animator interactAnim => interactIcon.GetComponent<Animator>();

        private int currentDialogueIndex = 0;
        private bool firstDialogueShown = false;

        public DialogueUI dialogueUI;

        public void Setup(NPC npc)
        {
            npcData = npc;
        }

        private void Awake()
        {
            dialogueUI = DialogueUI.Instance;
        }

        public void Interact()
        {
            if (npcData == null || npcData.Dialogos == null || npcData.Dialogos.Count == 0)
                return;

            if(GameManager.Instance.Player.Missoes.Contains(npcData.MissaoDisponivel) && !firstDialogueShown && currentDialogueIndex == 0)
            {
                firstDialogueShown = true;
                currentDialogueIndex = 1;
            }

            if (!firstDialogueShown)
            {
                var lines = npcData.Dialogos[0].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                var dialogueLines = new List<string>();
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        dialogueLines.Add(line.Trim());
                }

                if (npcData.MissaoDisponivel != null && GameManager.Instance.Player != null)
                {
                    GameManager.Instance.Player.Missoes.Add(npcData.MissaoDisponivel);
                    dialogueLines.Add($"Voce iniciou a missao {npcData.MissaoDisponivel.Nome}.");
                }

                Debug.Log($"dialogueUI: {dialogueUI}");
                Debug.Log($"dialogueLines: {string.Join(", ", dialogueLines)}");
                dialogueUI.ShowDialogue(dialogueLines.ToArray(), npcData.Nome);
                firstDialogueShown = true;
                currentDialogueIndex = 1;
            }
            else
            {
                if (npcData.Dialogos.Count > 1)
                {
                    if (currentDialogueIndex >= npcData.Dialogos.Count)
                        currentDialogueIndex = 1;

                    var lines = npcData.Dialogos[currentDialogueIndex].Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    var dialogueLines = new List<string>();
                    foreach(var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            dialogueLines.Add(line.Trim());
                    }
                    dialogueUI.ShowDialogue(dialogueLines.ToArray(), npcData.Nome);
                    currentDialogueIndex++;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {

            interactAnim.Play("NpcSpeech");
            if (collision.CompareTag("Player"))
            {
                PlayerController.nearbyNpc = this;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            interactAnim.Play("NpcSpeechClose");
            if (collision.CompareTag("Player"))
            {
                PlayerController.nearbyNpc = null;
            }
        }
    }
}