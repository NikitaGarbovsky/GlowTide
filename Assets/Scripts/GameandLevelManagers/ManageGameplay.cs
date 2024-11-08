using System;
using System.Collections;
using Cinemachine;
using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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

    public bool PlayerCanIssueMoveCommands = true;
    public bool PlayerCanCallBros = false;
    public bool PlayerCanThrowBros = false;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineFramingTransposer framingTransposer;
    
    [SerializeField] public GameObject playerCharacter;

    // References to level manager GameObjects
    [SerializeField] private GameObject[] levelManagers;
    private SceneManager sceneManager;
    
    // This is the object that is used for the fade-out/in effect for scene loading
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] public float fadeDuration = 2f;
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
        // Gets the camera components from the child game object. (the main camera)
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        
        if (fadeCanvasGroup == null)
        {
            fadeCanvasGroup = GetComponentInChildren<CanvasGroup>();
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
        
        GetPlayerReference();
        
        // Activate the appropriate level manager
        ActivateLevelManager(scene.name);
        // Initializes the level.
        GetLevelManager(scene.name).GetComponent<LevelManager>().InitializeLevel();
        
    }
    // After scene loads we call this to get a new reference to the player, (wont need this anymore if the player 
    // becomes a global object.)
    void GetPlayerReference()
    {
        // Initialize player reference 
        if (playerCharacter == null)
        {
            playerCharacter = GameObject.FindWithTag("Player");
        }
        
        // Sets the camera to follow the player gameobject
        gameObject.GetComponentInChildren<CinemachineVirtualCamera>().Follow = playerCharacter.transform;
        gameObject.GetComponentInChildren<CinemachineVirtualCamera>().LookAt = playerCharacter.transform;
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
            playerCharacter.GetComponent<AIPath>().destination = playerCharacter.transform.position;
            playerCharacter.GetComponent<AIPath>().canMove = false;
            PlayerCanIssueMoveCommands = false;
        }
    }
    // Give back player controls 
    public void ReturnPlayerControl()
    {
        if (playerCharacter != null)
        {
            playerCharacter.GetComponent<AIPath>().canMove = true;
            PlayerCanIssueMoveCommands = true;
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
    // This is executed from each individual ObjectTrigger script in scenes,
    // It passes its name(string) of the gameobject as a reference to the associated script, in the associated level. 
    public void ExecuteLevelManagerTrigger(string _sTriggerName)
    {
        GetLevelManager(SceneManager.GetActiveScene().name).GetComponent<LevelManager>().levelTrigger
            .ExecuteLevelTrigger(_sTriggerName);
    }
    // Pans the camera to a certain direction, for a certain duration
    // (the panning position is relative to the player, which the camera still follows)
    public IEnumerator PanCamera(Vector2 _v2TargetOffset, float _fPanDuration)
    {
        Vector2 startOffset = framingTransposer.m_TrackedObjectOffset;
        float elapsedTime = 0f;

        while (elapsedTime < _fPanDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _fPanDuration;
            framingTransposer.m_TrackedObjectOffset = Vector2.Lerp(startOffset, _v2TargetOffset, t);
            yield return null;
        }

        // Ensure it reaches the exact target value at the end
        framingTransposer.m_TrackedObjectOffset = _v2TargetOffset;
    }
    
    // Pans & Zooms out the camera. (this is called when starting a level)
    public IEnumerator PanAndZoomCamera(Vector2 targetOffset, float targetOrthographicSize, float duration)
    {
        Vector2 startOffset = framingTransposer.m_TrackedObjectOffset;
        float startOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp the camera offset
            framingTransposer.m_TrackedObjectOffset = Vector2.Lerp(startOffset, targetOffset, t);

            // Lerp the orthographic size
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startOrthographicSize, targetOrthographicSize, t);

            yield return null;
        }

        // Ensure final values are set
        framingTransposer.m_TrackedObjectOffset = targetOffset;
        virtualCamera.m_Lens.OrthographicSize = targetOrthographicSize;
    }
    // This is the primary method that is called throughout the codebase that will load a scene with the fading effect
    public void LoadSceneWithFade(string sceneName)
    {
        // Uses the passed in scene name (E.g. "1_Level1") to load the corresponding scene (with the fade effect)
        StartCoroutine(LoadSceneWithFadeCoroutine(sceneName));
    }

    private IEnumerator LoadSceneWithFadeCoroutine(string sceneName)
    {
        // Start fade-out
        yield return StartCoroutine(Fade(1f));

        // Load the scene
        if (sceneName == "reset")
        {
            // Reload current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }

        // Wait for one frame to allow the scene to load
        yield return null;

        // Start fade-in
        yield return StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0f;

        // Enable interaction blocking during fade-out
        if (targetAlpha == 1f)
        {
            fadeCanvasGroup.interactable = true;
            fadeCanvasGroup.blocksRaycasts = true;
        }

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;

        // Disable interaction blocking after fade-in
        if (targetAlpha == 0f)
        {
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }


}