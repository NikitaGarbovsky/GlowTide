using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Intro() { SceneManager.LoadSceneAsync(1); }

    public void Level1() { SceneManager.LoadSceneAsync(2); }

    public void Level2() { SceneManager.LoadSceneAsync(3); }

    public void Quit()
    {
        Application.Quit();
    }
}
