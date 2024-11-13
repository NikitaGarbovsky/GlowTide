using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private GameObject Gamemanager; 
    private void Awake()
    {
        Gamemanager = GameObject.FindWithTag("GameManager");
        if (Gamemanager != null)
        {
            if (Gamemanager.activeSelf == isActiveAndEnabled)
            {
                Destroy(Gamemanager);
            }
        }
    }

    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
