using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is a singleton game manager used throughout all the scenes of the game and manages the overall gameplay
/// loop of each level. It is referenced throughout all scenes. It holds an array of all the child, individual,
/// level manager objects, which it then manages in accordance with each level. 
///
/// To reference in scripts, use "ManageGameplay.Instance." 
/// </summary>
public sealed class ManageGameplay : MonoBehaviour
{
    // Static instance for singleton pattern
    public static ManageGameplay Instance { get; private set; }

    [SerializeField] private GameObject playerCharacter;

    // References to level manager GameObjects
    [SerializeField] private GameObject[] levelManagers;
    private SceneManager sceneManager;
    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // Initialize player reference 
        if (playerCharacter == null)
        {
            playerCharacter = GameObject.FindWithTag("Player");
        }
    }
    
    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from the event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    // This is called when a scene is loaded 
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        // Activate the appropriate level manager
        ActivateLevelManager(scene.name);
        // Initializes the level.
        GetLevelManager(scene.name).GetComponent<LevelManager>().InitializeLevel();
    }
    // A bunch of debug loaders for loading into each level
    private void Update()
    {
        if (Input.GetKeyDown("0"))
        {
            SceneManager.LoadScene("0_Introduction");
        }
        if (Input.GetKeyDown("1"))
        {
            SceneManager.LoadScene("1_Level1");
        }
        if (Input.GetKeyDown("2"))
        {
            // Add further scene loads as required...
        }
    }

    // Remove player controls (TODO probably call this when its needed in tutorial)
    public void RemovePlayerControl()
    {
        if (playerCharacter != null)
        {
            playerCharacter.GetComponent<AIPath>().canMove = false;
            // TODO disable player being able to throw the bros
        }
    }
    // Give back player controls (TODO probably call this when its needed in tutorial)
    public void ReturnPlayerControl()
    {
        if (playerCharacter != null)
        {
            playerCharacter.GetComponent<AIPath>().canMove = true;
        }
    }
    
    private void ActivateLevelManager(string sceneName)
    {
        // Deactivate all level managers first
        foreach (var manager in levelManagers)
        {
            if (manager != null)
            {
                manager.SetActive(false);
            }
        }

        // Activate the level manager gameObject for the current scene
        GameObject currentLevelManager = GetLevelManager(sceneName);
        if (currentLevelManager != null)
        {
            currentLevelManager.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ERROR: No Level Manager found for scene, \n" +
                             "There exists no gamemanager associated for this level, \n" +
                             "(you might of misspelled something) or haven't created the gamemanager" + sceneName);
        }
    }
    // Gets the corresponding level manager for the current scene.
    public GameObject GetLevelManager(string sceneName)
    {
        foreach (var manager in levelManagers)
        {
            if (manager.name == sceneName + "Manager")
            {
                return manager;
            }
        }
        return null;
    }
}