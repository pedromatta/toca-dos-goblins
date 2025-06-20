using Assets.Scripts.Entities;
using Assets.Scripts.Managers;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    public Button newButton;
    public Button loadButton;

    private string savePath;

    void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        bool hasSave = PlayerPrefs.HasKey("savegame");
#else
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        bool hasSave = File.Exists(savePath);
#endif

        loadButton.interactable = hasSave;
        loadButton.enabled = hasSave;

        newButton.onClick.AddListener(OnNewButtonClicked);
        loadButton.onClick.AddListener(OnLoadButtonClicked);
    }

    void OnNewButtonClicked()
    {
        GameManager.Instance.ChangeState(GameState.CharacterCreation);
    }

    void OnLoadButtonClicked()
    {
        GameManager.Instance.LoadGame();
        GameManager.Instance.ChangeState(GameState.InGame);
    }
}
