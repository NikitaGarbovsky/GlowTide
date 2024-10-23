using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PopUpTextTimer : MonoBehaviour
{

    public TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        text.enabled = false;
        ShowText();
        Invoke("HideText", 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowText()
    {
        text.enabled = true;
    }

    public void HideText()
    {
        text.enabled = false;
    }
}
