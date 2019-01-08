using UnityEngine;
using System.Collections;

public class MouseControl : MonoBehaviour
{
    [SerializeField]
    float mouseSensitivity;
    public GameObject vineCube;

    // cursor stuff
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 offset = Vector2.zero;

    void Awake() {
        Cursor.SetCursor(cursorTexture, offset, cursorMode);
    }

    void Update ()
    {
        float mouseX = Input.GetAxis ("Mouse X");
        float mouseY = Input.GetAxis ("Mouse Y");
        
        // Left-click or middle-click to drag camera around
        if(Input.GetMouseButton(0) || Input.GetMouseButton(2))
        {
            transform.Translate(new Vector3(-mouseX, 0.0f, -mouseY) * mouseSensitivity, Space.World);
        }

        // Right-click to move unit
        if(Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast (ray, out hit))
            {
                Instantiate(vineCube, hit.point + new Vector3(0.0f, 0.1f, 0.0f), Quaternion.identity);
            }
        }
    }

    void OnMouseEnter() {
        Cursor.SetCursor(cursorTexture, offset, cursorMode);
    }

    void OnMouseExit() {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
