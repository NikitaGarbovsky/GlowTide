using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] public CanvasGroup fadeCanvasGroup;
    [SerializeField] public float fadeDuration = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        fadeCanvasGroup.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadSceneWithFade(string sceneName)
    {
        Debug.Log("Pressed");
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
