using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Missoes;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.WorldExplorationPanels
{
    public class QuestsPanel : MonoBehaviour, IPanel
    {
        public event Action OnPanelClosed;
        public bool IsOpen => gameObject.activeSelf;

        public Transform QuestListContent;
        public GameObject QuestListItemPrefab;
        public TMP_Text QuestNameText;
        public TMP_Text QuestDetailsText;
        public Button CompleteButton;
        public Button CloseButton;

        private Personagem personagem;
        private List<Missao> quests;
        private Missao selectedQuest;

        void Start()
        {
            personagem = GameManager.Instance.Player;
            CloseButton.onClick.AddListener(ClosePanel);
            CompleteButton.onClick.AddListener(ReceiveReward);
            RefreshUI();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            RefreshUI();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            OnPanelClosed?.Invoke();
        }

        private void ClosePanel() => Close();

        private void RefreshUI()
        {
            ClearQuestDetails();
            personagem = GameManager.Instance.Player;
            quests = personagem?.Missoes ?? new List<Missao>();

            foreach (Transform child in QuestListContent)
                Destroy(child.gameObject);

            var sortedQuests = quests
                .OrderBy(q => q.Concluida)
                .ThenBy(q => q.Nome)
                .ToList();

            foreach (var quest in sortedQuests)
            {
                var questButtonObj = Instantiate(QuestListItemPrefab, QuestListContent);
                var questButton = questButtonObj.GetComponent<Button>();
                var questText = questButtonObj.GetComponentInChildren<TMP_Text>();
                questText.text = quest.Nome;
                questButton.interactable = !quest.Concluida;

                var capturedQuest = quest;
                questButton.onClick.AddListener(() => SelectQuest(capturedQuest));
            }

            if (sortedQuests.Count > 0)
            {
                if (selectedQuest == null || !sortedQuests.Contains(selectedQuest))
                {
                    selectedQuest = sortedQuests.FirstOrDefault(q => !q.Concluida) ?? sortedQuests[0];
                }
                SelectQuest(selectedQuest);
            }
            else
            {
                selectedQuest = null;
                ClearQuestDetails();
            }
        }

        private void SelectQuest(Missao quest)
        {
            selectedQuest = quest;
            UpdateQuestDetails();
        }

        private void UpdateQuestDetails()
        {
            if (selectedQuest == null)
            {
                ClearQuestDetails();
                return;
            }

            QuestNameText.text = selectedQuest.Nome;
            QuestDetailsText.text = $"{selectedQuest.Descricao}\n\n" +
                $"Progresso: {selectedQuest.Progresso}/{selectedQuest.QuantidadeNecessaria}\n";

            if (selectedQuest is MissaoColeta coleta)
                QuestDetailsText.text += $"Item alvo: {coleta.ItemParaColeta.Nome}\n";
            else if (selectedQuest is MissaoExterminio exterminio)
                QuestDetailsText.text += $"Inimigo alvo: {exterminio.InimigoParaExterminio.Nome}\n";

            QuestDetailsText.text += "Recompensas: ";

            if(selectedQuest.RecompensaItem != null)
                QuestDetailsText.text += $"{selectedQuest.RecompensaItem.Nome}, ";

            QuestDetailsText.text += $"{selectedQuest.RecompensaXP}XP";

            CompleteButton.interactable = !selectedQuest.Concluida && selectedQuest.Progresso >= selectedQuest.QuantidadeNecessaria;
        }

        private void ClearQuestDetails()
        {
            QuestNameText.text = "";
            QuestDetailsText.text = "";
            CompleteButton.interactable = false;
        }

        private void ReceiveReward()
        {
            if (selectedQuest == null || selectedQuest.Concluida)
                return;

            selectedQuest.Completar(personagem);

            if (!selectedQuest.Concluida)
            {
                QuestDetailsText.text = "A missão não pôde ser concluída, verifique se há espaço no inventário";
            }

            RefreshUI();
        }
    }
}