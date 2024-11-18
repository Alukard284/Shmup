using UnityEngine;

public class MainMenuInput : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    private Vector2 hotSpot = new Vector2(0, 0);


    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        Cursor.visible = true;
    }
}
