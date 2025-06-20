using Assets.Scripts.Controllers;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Itens;
using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.WorldExplorationPanels
{
    public class InventarioPanel : MonoBehaviour, IPanel
    {
        public event Action OnPanelClosed;
        public bool IsOpen => gameObject.activeSelf;

        public Image EquippedWeaponIcon;
        public Transform ItemGridContent;
        public GameObject ItemSlotPrefab;
        public TMP_Text DescriptionBox;
        public Button CloseButton;

        private Personagem personagem;
        private Dictionary<string, Sprite> itemIcons;

        void Start()
        {
            personagem = GameManager.Instance.Player;
            CloseButton.onClick.AddListener(ClosePanel);
            LoadItemIcons();
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


        private void LoadItemIcons()
        {
            itemIcons = new Dictionary<string, Sprite>();
        }

        private Sprite GetItemIcon(string itemName)
        {
            if (itemIcons.ContainsKey(itemName))
                return itemIcons[itemName];

            if (itemName == "Arma Amorfica")
            {
                switch (personagem.Classe.Nome)
                {
                    case "Guerreiro":
                        itemName = "Arma Amorfica Guerreiro";
                        break;
                    case "Mago":
                        itemName = "Arma Amorfica Mago";
                        break;
                    case "Arqueiro":
                        itemName = "Arma Amorfica Arqueiro";
                        break;
                }
            }

            Sprite icon = Resources.Load<Sprite>($"Items/{itemName}");
            itemIcons[itemName] = icon;
            return icon;
        }

        private void RefreshUI()
        {
            Debug.Log($"personagem: {personagem}");
            Debug.Log($"personagem.Inventario: {personagem?.Inventario}");
            Debug.Log($"personagem.Inventario.ArmaEquipada: {personagem?.Inventario?.ArmaEquipada}");
            Debug.Log($"EquippedWeaponIcon: {EquippedWeaponIcon}");
            var arma = personagem.Inventario.ArmaEquipada;
            EquippedWeaponIcon.sprite = GetItemIcon(arma.Nome);

            DescriptionBox.text = $"{arma.Nome}\n{arma.Descricao}";

            foreach (Transform child in ItemGridContent)
                Destroy (child.gameObject);

            var sorted = personagem.Inventario.Itens.OrderBy(i => i.Nome);
            var grouped = sorted
                .GroupBy(i => i.Nome)
                .Select(g => new { Item = g.First(), Count = g.Count()});

            foreach (var entry in grouped)
            {
                var slot = Instantiate(ItemSlotPrefab, ItemGridContent).GetComponent<ItemController>();
                slot.Setup(entry.Item, entry.Count, GetItemIcon(entry.Item.Nome));

                slot.OnRemove = (ui) =>
                {
                    personagem.Inventario.RemoverItem(ui.ItemData);
                    RefreshUI();
                };

                if (entry.Item is IUsableItem)
                {
                    slot.OnClick = (ui) =>
                    {
                        ((IUsableItem)ui.ItemData).Use(personagem);
                        RefreshUI();
                    };
                }
                else
                {
                    slot.OnClick = null;
                }

                slot.OnPointerEnterSlot = (ui) =>
                {
                    DescriptionBox.text = $"{ui.ItemData.Nome}\n{ui.ItemData.Descricao}";
                };

                slot.OnPointerExitSlot = () =>
                {
                    DescriptionBox.text = $"{arma.Nome}\n{arma.Descricao}";
                };

                slot.RemoveButton.onClick.AddListener(() => RemoveItem(slot.ItemData));
            }
        }

        private void RemoveItem(Item item)
        {
            personagem.Inventario.RemoverItem(item);
            RefreshUI();
        }
    }
}