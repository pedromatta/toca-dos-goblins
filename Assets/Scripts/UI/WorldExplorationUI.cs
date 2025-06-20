using Assets.Scripts.Controllers;
using Assets.Scripts.Managers;
using Assets.Scripts.UI.WorldExplorationPanels;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class WorldExplorationUI : MonoBehaviour
    {
        public Button AreaName;
        public GameObject AreaDescription;

        public Button PauseButton;
        public Button InventoryButton;
        public Button QuestsButton;
        public Button StatsButton;

        public StatsPanel StatsPanel; 
        public InventarioPanel InventarioPanel;
        public QuestsPanel QuestsPanel;
        public PausePanel PausePanel;

        private IPanel _activePanel;

        void Start()
        {
            AreaName.GetComponentInChildren<TMP_Text>().text = GameManager.Instance.CurrentArea.Nome;
            AreaDescription.GetComponentInChildren<TMP_Text>().text = GameManager.Instance.CurrentArea.Descricao;

            AreaName.onClick.AddListener(() => StartCoroutine(ShowAreaDescription()));
            StatsButton.onClick.AddListener(() => OpenPanel(StatsPanel));
            InventoryButton.onClick.AddListener(() => OpenPanel(InventarioPanel));
            QuestsButton.onClick.AddListener(() => OpenPanel(QuestsPanel));
            PauseButton.onClick.AddListener(() => OpenPanel(PausePanel));

            if (StatsPanel != null)
                StatsPanel.gameObject.SetActive(false);
        }

        private IEnumerator ShowAreaDescription()
        {
            AreaDescription.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            AreaDescription.SetActive(false);
        }

        private void OpenPanel(IPanel panel)
        {
            if (_activePanel != null && _activePanel.IsOpen)
                return;

            _activePanel = panel;
            SetWorldElementsInteractable(false);

            panel.OnPanelClosed += OnPanelClosed;
            panel.Open();
        }

        private void OnPanelClosed()
        {
            if (_activePanel != null)
                _activePanel.OnPanelClosed -= OnPanelClosed;

            _activePanel = null;
            SetWorldElementsInteractable(true);
        }

        private void SetWorldElementsInteractable(bool interactable)
        {
            PlayerController.IsMovementBlocked = !interactable;
            PauseButton.interactable = interactable;
            InventoryButton.interactable = interactable;
            QuestsButton.interactable = interactable;
            StatsButton.interactable = interactable;
            AreaName.interactable = interactable;
        }
    }
}