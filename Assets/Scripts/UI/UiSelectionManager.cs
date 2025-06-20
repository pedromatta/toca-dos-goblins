using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UISelectionManager : MonoBehaviour
{
    // Drag your outline Image here in the Inspector
    [SerializeField] private RectTransform outline;

    // Optional: Adjust the padding of the outline
    [SerializeField] private float paddingY = 25f;
    [SerializeField] private float paddingX = 20f;

    private GameObject currentSelected;

    void Update()
    {
        // Get the currently selected UI GameObject
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        // If the selection has changed, update the outline
        if (currentSelected != selectedObject)
        {
            UpdateOutline(selectedObject);
        }
    }

    private void UpdateOutline(GameObject selectedObject)
    {
        currentSelected = selectedObject;

        if (currentSelected == null || !currentSelected.activeInHierarchy)
        {
            outline.gameObject.SetActive(false);
            return;
        }

        if (!currentSelected.TryGetComponent<RectTransform>(out var selectedRect))
        {
            outline.gameObject.SetActive(false);
            return;
        }

        outline.gameObject.SetActive(true);
        outline.SetParent(selectedRect.parent);
        outline.position = selectedRect.position;
        outline.sizeDelta = new Vector2(selectedRect.sizeDelta.x + paddingX * 2, selectedRect.sizeDelta.y + paddingY * 2);
        outline.SetAsLastSibling();
    }
}