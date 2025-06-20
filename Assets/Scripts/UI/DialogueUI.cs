using Assets.Scripts.World;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public static DialogueUI Instance { get; private set; }

        public GameObject dialoguePanel;
        public TMP_Text dialogueTitle;
        public TMP_Text dialogueText;
        public Button nextButton;

        private string[] lines;
        private int currentLine;
        private Action onDialogueEnd;
        private bool isDialogueActive = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            if (nextButton != null)
            {
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(OnNextClicked);
            }

            if (dialoguePanel != null)
                dialoguePanel.SetActive(false);
        }

        public void ShowDialogue(string[] dialogueLines, string npcName, Action onEnd = null)
        {
            if (isDialogueActive)
                return; // Prevent overlapping dialogues


            dialogueTitle.GetComponent<TMP_Text>().text = npcName;
            lines = dialogueLines;
            currentLine = 0;
            onDialogueEnd = onEnd;
            isDialogueActive = true;
            dialoguePanel.SetActive(true);
            ShowCurrentLine();
        }

        private void ShowCurrentLine()
        {
            if (lines != null && currentLine < lines.Length)
            {
                dialogueText.text = lines[currentLine].Trim();
            }
        }

        public void OnNextClicked()
        {
            currentLine++;
            if (lines != null && currentLine < lines.Length)
            {
                ShowCurrentLine();
            }
            else
            {
                dialoguePanel.SetActive(false);
                isDialogueActive = false;
                onDialogueEnd?.Invoke();
            }
        }
    }
}