using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursor;

    private void Awake()
    {
        if (cursor != null)
        {
            // Center hotspot for a 32x32 cursor
            var hotspot = new Vector2(cursor.width / 2, cursor.height / 2);
            Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Cursor texture not assigned in CursorManager.");
        }
    }
}
