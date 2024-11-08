using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CursorModeEnum
{
    Default,
    Hover
}

public class CursorController : MonoBehaviour
{


    // Singleton
    public static CursorController m_instance { get; private set; }

    [SerializeField] Texture2D m_cursorTexture;
    [SerializeField] Texture2D m_cursorHoverTexture;
    [SerializeField] Vector2 m_cursorPosition = Vector2.zero;
    [SerializeField] GameObject m_cursorVFX;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

    public void SetCursorMode(CursorModeEnum _mode)
    {
        switch (_mode)
        {
            case CursorModeEnum.Default:
                Cursor.SetCursor(m_cursorHoverTexture, m_cursorPosition, CursorMode.Auto);
                break;
            case CursorModeEnum.Hover:
                Cursor.SetCursor(m_cursorHoverTexture, m_cursorPosition, CursorMode.Auto);
                break;
            default:
                break;
        }
    }
}
