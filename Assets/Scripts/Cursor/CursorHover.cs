using UnityEngine;
using UnityEngine.EventSystems;

public class CursorHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorController.m_instance.SetCursorMode(CursorModeEnum.Hover);
    }    
    
    public void OnPointerExit(PointerEventData eventData)
    { 
        CursorController.m_instance.SetCursorMode(CursorModeEnum.Default);
    }
}
