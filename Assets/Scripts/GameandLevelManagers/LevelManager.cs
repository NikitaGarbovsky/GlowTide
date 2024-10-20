using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string m_sLevelName;
    public LevelTriggers levelTrigger;
    private void OnEnable()
    {
        // Check if the name ends with "Manager" and remove it if present
        if (m_sLevelName == "" && gameObject.name.EndsWith("Manager"))
        {
            m_sLevelName = gameObject.name.Substring(0, gameObject.name.Length - "Manager".Length);
        }

        levelTrigger = GetComponent<LevelTriggers>();
    }

    public void InitializeLevel()
    {
        Debug.Log("Initializing " + m_sLevelName);

        // TODO initialize first level parts here
    }
}