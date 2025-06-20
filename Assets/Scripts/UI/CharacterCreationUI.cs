using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Entities;
using Assets.Scripts.Managers;
using UnityEngine.UI;
using System.Linq;
using Assets.Scripts.Data;

[System.Serializable]
public struct CharacterPrefabEntry
{
    public string Race;
    public string Classe;
    public GameObject Prefab;
}

public class CharacterCreationUI : MonoBehaviour
{
    public Personagem PersonagemCriado;
    public TMP_InputField NomeInput;
    public Button RacaButton;
    public Button ClasseButton;
    public Button CriarButton;

    public CharacterPrefabDatabase CharacterPrefabEntries;
    public Image CharacterPreviewImage;

    public Button ForcaButton;
    public Button DestrezaButton;
    public Button InteligenciaButton;
    public Button ConstituicaoButton;

    public TMP_Text AttributePoints;

    public GameObject StatsBanner;

    private List<Raca> RacasDisponiveis;
    private List<Classe> ClassesDisponiveis;
    private int CurrentRaceIndex = 0;
    private int CurrentClassIndex = 0;

    void Start()
    {
        NomeInput.textComponent.textWrappingMode = TextWrappingModes.Normal;

        RacasDisponiveis = GameManager.Instance.Racas.Values.ToList();
        ClassesDisponiveis = GameManager.Instance.Classes.Values.ToList();

        UpdateRaceButtonText();
        UpdateClassButtonText();

        PersonagemCriado = CreatePersonagem();

        UpdateAttributePointsText();
        UpdateStatsBannerText();
        DisplayCharacter();

        RacaButton.onClick.AddListener(ChangeRace);
        ClasseButton.onClick.AddListener(ChangeClass);
        ForcaButton.onClick.AddListener(() => UpgradeAttribute("forca"));
        InteligenciaButton.onClick.AddListener(() => UpgradeAttribute("inteligencia"));
        DestrezaButton.onClick.AddListener(() => UpgradeAttribute("destreza"));
        ConstituicaoButton.onClick.AddListener(() => UpgradeAttribute("constituicao"));

        CriarButton.onClick.AddListener(SalvarPersonagem);

        NomeInput.onValueChanged.AddListener((value) =>
        {
            CriarButton.interactable = !string.IsNullOrWhiteSpace(value);
        });
        CriarButton.interactable = !string.IsNullOrWhiteSpace(NomeInput.text);
    }

    void UpgradeAttribute(string attribute)
    {
        PersonagemCriado.DistribuirPontos(attribute);
        DisplayCharacter();
    }

    void ChangeRace()
    {
        CurrentRaceIndex = (CurrentRaceIndex + 1) % RacasDisponiveis.Count;
        UpdateRaceButtonText();
        PersonagemCriado = CreatePersonagem();
        DisplayCharacter();
    }

    void ChangeClass()
    {
        CurrentClassIndex = (CurrentClassIndex + 1) % ClassesDisponiveis.Count;
        UpdateClassButtonText();
        PersonagemCriado = CreatePersonagem();
        DisplayCharacter();
    }

    void SalvarPersonagem()
    {
        string nome = NomeInput.text.Trim();
        if (string.IsNullOrEmpty(nome))
        {
            CriarButton.interactable = false;
            return;
        }

        PersonagemCriado.Nome = nome;

        GameManager.Instance.Player = PersonagemCriado;
        GameManager.Instance.CurrentArea = GameManager.Instance.Mundo.Areas[0];
        GameManager.Instance.SaveGame();
        GameManager.Instance.ChangeState(GameState.InGame);
    }

    void DisplayCharacter()
    {
        string selectedRace = RacasDisponiveis[CurrentRaceIndex].Nome;
        string selectedClass = ClassesDisponiveis[CurrentClassIndex].Nome;

        var entry = CharacterPrefabEntries.GetPrefab(selectedRace, selectedClass);

        if (entry != null)
        {
            SpriteRenderer sr = entry.GetComponent<SpriteRenderer>();
            if (sr != null && CharacterPreviewImage != null)
            {
                CharacterPreviewImage.sprite = sr.sprite;
                CharacterPreviewImage.enabled = true;

                UpdateAttributeButton(ForcaButton, "FOR", PersonagemCriado.Forca);
                UpdateAttributeButton(DestrezaButton, "DES", PersonagemCriado.Destreza);
                UpdateAttributeButton(InteligenciaButton, "INT", PersonagemCriado.Inteligencia);
                UpdateAttributeButton(ConstituicaoButton, "CON", PersonagemCriado.Constituicao);

                UpdateAttributePointsText();
                UpdateStatsBannerText();
            }
            else if (CharacterPreviewImage != null)
            {
                CharacterPreviewImage.enabled = false;
            }
        }
        else if (CharacterPreviewImage != null)
        {
            CharacterPreviewImage.enabled = false;
            Debug.LogWarning($"No prefab found for Race: {selectedRace}, Class: {selectedClass}");
        }
    }

    Personagem CreatePersonagem()
    {
        string nome = NomeInput.text;
        Classe classe = ClassesDisponiveis[CurrentClassIndex];
        Raca raca = RacasDisponiveis[CurrentRaceIndex];
        return new Personagem(nome, classe, raca);
    }

    void UpdateRaceButtonText()
    {
        RacaButton.GetComponentInChildren<TMP_Text>().text = $"Raca: {RacasDisponiveis[CurrentRaceIndex].Nome}";
    }

    void UpdateClassButtonText()
    {
        ClasseButton.GetComponentInChildren<TMP_Text>().text = $"Classe: {ClassesDisponiveis[CurrentClassIndex].Nome}";
    }

    void UpdateAttributeButton(Button button, string label, int value)
    {
        var texts = button.GetComponentsInChildren<TMP_Text>();
        if (texts.Length >= 2)
        {
            texts[0].text = label;
            texts[1].text = value.ToString();
        }
    }

    void UpdateAttributePointsText()
    {
        AttributePoints.text = $"Pontos Disponiveis: {PersonagemCriado.PontosAtributosDisponiveis}";
    }

    void UpdateStatsBannerText()
    {
        var statsDisplay = StatsBanner.GetComponentsInChildren<Image>().ToList();

        var level = statsDisplay[0];
        var hp = statsDisplay[1];
        var mana = statsDisplay[2];
        var defense = statsDisplay[3];
        var carryCapacity = statsDisplay[4];
        var xp = statsDisplay[5];

        level.GetComponentInChildren<TMP_Text>().text = $"Nivel: {PersonagemCriado.Nivel}";

        hp.GetComponentsInChildren<TMP_Text>()[0].text = "Vida";
        hp.GetComponentsInChildren<TMP_Text>()[1].text = $"{PersonagemCriado.VidaAtual}/\n{PersonagemCriado.VidaMaxima}";

        mana.GetComponentsInChildren<TMP_Text>()[0].text = "Mana";
        mana.GetComponentsInChildren<TMP_Text>()[1].text = $"{PersonagemCriado.ManaAtual}/\n{PersonagemCriado.ManaMaxima}";

        defense.GetComponentsInChildren<TMP_Text>()[0].text = "Defesa";
        defense.GetComponentsInChildren<TMP_Text>()[1].text = PersonagemCriado.Defesa.ToString();

        carryCapacity.GetComponentsInChildren<TMP_Text>()[0].text = "Capacidade de Carga";
        carryCapacity.GetComponentsInChildren<TMP_Text>()[1].text = PersonagemCriado.CapacidadeCarga.ToString();

        xp.GetComponentsInChildren<TMP_Text>()[0].text = "Experiencia";
        xp.GetComponentsInChildren<TMP_Text>()[1].text = $"{PersonagemCriado.Experiencia}/\n{PersonagemCriado.ExperienciaParaProximoNivel}";
    }
}