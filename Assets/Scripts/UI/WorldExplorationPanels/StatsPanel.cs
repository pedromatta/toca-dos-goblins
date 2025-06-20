using Assets.Scripts.Data;
using Assets.Scripts.Entities;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.WorldExplorationPanels
{
    public class StatsPanel : MonoBehaviour, IPanel
    {
        public event Action OnPanelClosed;
        public bool IsOpen => gameObject.activeSelf;

        public void Open()
        {
            gameObject.SetActive(true);
            Setup();
        }
        public void Close()
        {
            gameObject.SetActive(false);
            OnPanelClosed?.Invoke();
        }

        private Personagem personagem;
        public Image NomePersonagem;
        public Button RacaButton;
        public Button ClasseButton;
        public Button CriarButton;
        public Button CloseButton;

        public CharacterPrefabDatabase CharacterPrefabEntries;
        public Image CharacterPreviewImage;

        public Button ForcaButton;
        public Button DestrezaButton;
        public Button InteligenciaButton;
        public Button ConstituicaoButton;

        public TMP_Text AttributePoints;

        public GameObject StatsBanner;

        void Setup()
        {
            personagem = GameManager.Instance.Player;

            NomePersonagem.GetComponentInChildren<TMP_Text>().textWrappingMode = TextWrappingModes.Normal;
            NomePersonagem.GetComponentInChildren<TMP_Text>().text = personagem.Nome;

            RacaButton.enabled = false;
            RacaButton.GetComponentInChildren<TMP_Text>().text = personagem.Raca.Nome;
            ClasseButton.enabled = false;
            ClasseButton.GetComponentInChildren<TMP_Text>().text = personagem.Classe.Nome;

            UpdateAttributePointsText();
            UpdateStatsBannerText();
            DisplayCharacter();

            ForcaButton.onClick.AddListener(() => UpgradeAttribute("forca"));
            InteligenciaButton.onClick.AddListener(() => UpgradeAttribute("inteligencia"));
            DestrezaButton.onClick.AddListener(() => UpgradeAttribute("destreza"));
            ConstituicaoButton.onClick.AddListener(() => UpgradeAttribute("constituicao"));

            CloseButton.onClick.AddListener(ClosePanel);
            CriarButton.onClick.AddListener(SalvarPersonagem);
        }

        void UpgradeAttribute(string attribute)
        {
            personagem.DistribuirPontos(attribute);
            DisplayCharacter();
        }

        void ClosePanel()
        {
            Close();
        }

        void SalvarPersonagem()
        {
            GameManager.Instance.Player = personagem;
            GameManager.Instance.SaveGame();
            Close();
        }

        void DisplayCharacter()
        {
            var entry = CharacterPrefabEntries.GetPrefab(personagem.Raca.Nome, personagem.Classe.Nome);

            if (entry != null)
            {
                SpriteRenderer sr = entry.GetComponent<SpriteRenderer>();
                if (sr != null && CharacterPreviewImage != null)
                {
                    CharacterPreviewImage.sprite = sr.sprite;
                    CharacterPreviewImage.enabled = true;

                    UpdateAttributeButton(ForcaButton, "FOR", personagem.Forca);
                    UpdateAttributeButton(DestrezaButton, "DES", personagem.Destreza);
                    UpdateAttributeButton(InteligenciaButton, "INT", personagem.Inteligencia);
                    UpdateAttributeButton(ConstituicaoButton, "CON", personagem.Constituicao);

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
                Debug.LogWarning($"No prefab found for character");
            }
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
            AttributePoints.text = $"Pontos Disponiveis: {personagem.PontosAtributosDisponiveis}";
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

            level.GetComponentInChildren<TMP_Text>().text = $"Nivel: {personagem.Nivel}";

            hp.GetComponentsInChildren<TMP_Text>()[0].text = "Vida";
            hp.GetComponentsInChildren<TMP_Text>()[1].text = $"{personagem.VidaAtual}/\n{personagem.VidaMaxima}";

            mana.GetComponentsInChildren<TMP_Text>()[0].text = "Mana";
            mana.GetComponentsInChildren<TMP_Text>()[1].text = $"{personagem.ManaAtual}/\n{personagem.ManaMaxima}";

            defense.GetComponentsInChildren<TMP_Text>()[0].text = "Defesa";
            defense.GetComponentsInChildren<TMP_Text>()[1].text = personagem.Defesa.ToString();

            carryCapacity.GetComponentsInChildren<TMP_Text>()[0].text = "Capacidade de Carga";
            carryCapacity.GetComponentsInChildren<TMP_Text>()[1].text = $"{personagem.Inventario.PesoAtual()}/\n{personagem.CapacidadeCarga.ToString()}";

            xp.GetComponentsInChildren<TMP_Text>()[0].text = "Experiencia";
            xp.GetComponentsInChildren<TMP_Text>()[1].text = $"{personagem.Experiencia}/\n{personagem.ExperienciaParaProximoNivel}";
        }
    }
}