using Assets.Scripts.Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.WorldExplorationPanels
{
    public class PausePanel : MonoBehaviour, IPanel
    {
        public event Action OnPanelClosed;
        public bool IsOpen => gameObject.activeSelf;

        public Button MainMenuButton;
        public Button SaveButton;
        public Button LoadButton;
        public Button CloseButton;

        void Start()
        {
            MainMenuButton.onClick.AddListener(GoToMainMenu);
            SaveButton.onClick.AddListener(SaveGame);
            LoadButton.onClick.AddListener(LoadGame);
            CloseButton.onClick.AddListener(ClosePanel);
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            OnPanelClosed?.Invoke();
        }

        private void ClosePanel() => Close();

        private void GoToMainMenu()
        {
            Close();
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }

        private void SaveGame()
        {
            Close();
            GameManager.Instance.SaveGame();
        }

        private void LoadGame()
        {
            Close();
            GameManager.Instance.LoadGame();
            GameManager.Instance.ChangeState(GameState.InGame);
        }
    }
}