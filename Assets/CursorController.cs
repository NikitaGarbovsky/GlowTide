using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [SerializeField] Texture2D m_cursorTexture;
    [SerializeField] Texture2D m_cursorHoverTexture;
    [SerializeField] Vector2 m_cursorPosition = Vector2.zero;
    [SerializeField] GameObject m_cursorVFX;

    private void Start()
    {
        Cursor.SetCursor(m_cursorTexture, m_cursorPosition, CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            m_cursorVFX.SetActive(true);
            m_cursorVFX.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward);
        }
        else 
        {
            m_cursorVFX.SetActive(false); 
        }
    }
}
