using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PopUpTextChecker : MonoBehaviour
{

    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text.enabled = false;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            text.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            text.enabled = false;
        }
    }
}
