using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level2Triggers : LevelTriggers
{
    private Tilemap tilemap; // Reference to the Tilemap
    private Vector3 levelCenter; // Center position of the level
    [SerializeField] private GameObject EelBossPrefab;
    [SerializeField] public GameObject BroSnackGO;
    [SerializeField] public AudioSource audioSource;
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_ZoomOutAndShowLevel") // This is the name of the gameobject that is being triggered.
        {
            StartCoroutine(ZoomOutAndShowLevel());
        }

        if (_sTriggerName == "1_EelBoss")
        {
            StartCoroutine(EelBossSequence());
        }
        if (_sTriggerName == "2_LevelTransition") 
        {
            // This is where the level transition trigger occurs
            Debug.Log("Level of level triggered");
            
            ManageGameplay.Instance.LoadSceneWithFade("3_Level3");
        }
    }
    private void Start()
    {
        // Find the Tilemap in the scene (assuming there's only one)
        tilemap = FindObjectOfType<Tilemap>();

        if (tilemap != null)
        {
            // Calculate the bounds of the Tilemap
            Bounds tilemapBounds = tilemap.localBounds;

            // Get the center position of the level
            levelCenter = tilemapBounds.center;
        }
        else
        {
            Debug.LogError("Tilemap not found in the scene.");
            // Set a default center position if needed
            levelCenter = Vector3.zero;
        }
    }
    private IEnumerator ZoomOutAndShowLevel()
    {
        // 1. Remove all controls from the player
        ManageGameplay.Instance.RemovePlayerControl();
        ManageGameplay.Instance.PlayerCanCallBros = false;
        ManageGameplay.Instance.PlayerCanThrowBros = false;

        // 2. Calculate the offset between the level center and the player
        Vector2 targetOffset = levelCenter - ManageGameplay.Instance.playerCharacter.transform.position;

        // 3. Camera pans to the level center while zooming out
        float zoomOutSize = 10f; // Adjust this value based on the levels size
        float panDuration = 2f;
        yield return StartCoroutine(ManageGameplay.Instance.PanAndZoomCamera(targetOffset, zoomOutSize, panDuration));

        // 4. Hold the camera at the zoomed-out view for a specified duration
        float holdDuration = 2f; // TODO maybe add this as a variable?
        yield return new WaitForSeconds(holdDuration);

        // 5. Camera returns to default size and position following the player
        yield return StartCoroutine(ManageGameplay.Instance.PanAndZoomCamera(Vector2.zero, 2.75f, panDuration));

        // 6. Return all controls back to the player
        ManageGameplay.Instance.ReturnPlayerControl();
        ManageGameplay.Instance.PlayerCanCallBros = true;
        ManageGameplay.Instance.PlayerCanThrowBros = true;

        BroSnackGO.SetActive(true);
        Invoke("RemoveBroSnackTutorialText", 10.0f);
    }
    private IEnumerator EelBossSequence()
    {
        ManageGameplay.Instance.m_AudioSource.clip = null; // kill the audio
        
        // 1. Freeze player position, remove controls
        ManageGameplay.Instance.RemovePlayerControl();
        ManageGameplay.Instance.PlayerCanCallBros = false;
        ManageGameplay.Instance.PlayerCanThrowBros = false;
        
        // 2. Pan camera to the SE
        Vector3 positionToPanCamera = new Vector2(8f, -5.5f);
        Vector2 targetOffset = positionToPanCamera - ManageGameplay.Instance.playerCharacter.transform.position;
        yield return StartCoroutine(ManageGameplay.Instance.PanCamera(targetOffset, 2f));
        
        // 4. Instantiate the Eel Boss at the spawn position
        Vector3 vEelBossSpawnWorldPosition = new Vector3(12.91f, -0.21f, 0f);
        GameObject EelBossInstance = Instantiate(EelBossPrefab, vEelBossSpawnWorldPosition, Quaternion.identity);
        
        // 5. Command the Eel Boss to move to the target point
        BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
        Vector2 eelBossTargetPosition = new Vector2(-5.7f, -11.96f);
        eelBossController.MovetoPoint(eelBossTargetPosition);
        eelBossController.SetDirectionForMovement(10, "SW"); // Sets the correct movement animation direction
        // Variables to check Eel Boss's progress
        Vector2 slugDestroyPosition = new Vector2(7.409231f, -3.683079f);
        bool slugsDestroyed = false;
        float destroySlugsThreshold = 0.5f; // Adjust the threshold as needed

        float distanceThreshold = 0.1f; // For final target position

        // 6. Loop until Eel Boss reaches its target position
        while (Vector2.Distance(EelBossInstance.transform.position, eelBossTargetPosition) > distanceThreshold)
        {
            // Check if we haven't destroyed slugs yet and the Eel Boss has reached the slugDestroyPosition
            if (!slugsDestroyed && Vector2.Distance(EelBossInstance.transform.position, slugDestroyPosition) <= destroySlugsThreshold)
            {
                // 7. Destroy all bros assigned to the player except one
                GameObject player = ManageGameplay.Instance.playerCharacter;
                PlayerSlugManager slugManager = player.GetComponent<PlayerSlugManager>();
                if (slugManager.m_lAssignedSlugs.Count > 0)
                {
                    GameObject slugToKeep = slugManager.m_lAssignedSlugs[0];

                    for (int i = 1; i < slugManager.m_lAssignedSlugs.Count; i++)
                    {
                        GameObject slugToDestroy = slugManager.m_lAssignedSlugs[i];
                        Destroy(slugToDestroy);
                    }

                    slugManager.m_lAssignedSlugs.Clear();
                    slugManager.m_lAssignedSlugs.Add(slugToKeep);
                    audioSource.Play();
                }
                slugsDestroyed = true;
            }
            yield return null;
        }

        // 8. Destroy the Eel Boss
        Destroy(EelBossInstance);

        // 9. Return movement and controls to the player
        ManageGameplay.Instance.ReturnPlayerControl();
        ManageGameplay.Instance.PlayerCanCallBros = true;
        ManageGameplay.Instance.PlayerCanThrowBros = true;

        // Pan the camera back to the player
        yield return StartCoroutine(ManageGameplay.Instance.PanCamera(Vector2.zero, 2f));
    }

    private void RemoveBroSnackTutorialText()
    {
        BroSnackGO.SetActive(false);
    }
}
