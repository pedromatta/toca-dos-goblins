using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Assets.Scripts.Entities;
using System;

namespace Assets.Scripts.Controllers
{
    public class ItemController : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public Image Icon;
        public TMP_Text AmountText;
        public Button RemoveButton;

        [HideInInspector] public Item ItemData;
        [HideInInspector] public int Amount;
        [HideInInspector] public Action<ItemController> OnRemove;
        [HideInInspector] public Action<ItemController> OnClick;
        [HideInInspector] public Action<ItemController> OnEndDragAction;
        [HideInInspector] public Action<ItemController> OnPointerEnterSlot;
        [HideInInspector] public Action OnPointerExitSlot;

        public void Setup(Item item, int amount, Sprite icon)
        {
            ItemData = item;
            Amount = amount;
            Icon.sprite = icon;
            AmountText.text = amount > 1 ? amount.ToString() : string.Empty;
        }

        // Fix for CS0102: Removed duplicate OnClick method definition.
        public void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke(this);
        public void OnDrag(PointerEventData eventData) { /* Drag visuals handled by panel */ }
        public void OnEndDrag(PointerEventData eventData) => OnEndDragAction?.Invoke(this);
        public void OnPointerEnter(PointerEventData eventData) => OnPointerEnterSlot?.Invoke(this);
        public void OnPointerExit(PointerEventData eventData) => OnPointerExitSlot?.Invoke();
    }
}