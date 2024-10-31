using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Level2Triggers : LevelTriggers
{
    private Tilemap tilemap; // Reference to the Tilemap
    private Vector3 levelCenter; // Center position of the level
    [SerializeField] private GameObject EelBossPrefab;
    
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_ZoomOutAndShowLevel") // This is the name of the gameobject that is being triggered.
        {
            StartCoroutine(ZoomOutAndShowLevel());
        }

        if (_sTriggerName == "1_EelBoss")
        {
            // Implement Eel boss functionality here. 
            // 1. Freeze player position, remove controls.
            ManageGameplay.Instance.RemovePlayerControl();
            ManageGameplay.Instance.PlayerCanCallBros = false;
            ManageGameplay.Instance.PlayerCanThrowBros = false;
            
            // 2. Set bros position.
            
            // 3. Instantiate Eel boss at EelBossSpawn gameobject

            // Access the GridGraph directly through AstarPath
            var gridGraph = AstarPath.active.data.gridGraph;

            // Define the world position you're interested in
            Vector3 vEelBossSpawnWorldPosition = new Vector3(8.1f, 0.16f, 0f);

            // Find the nearest node to the specified world position
            var nearestNode = gridGraph.GetNearest(vEelBossSpawnWorldPosition).node;

            // Convert the node's internal position to a Vector3 (if you need the actual world position)
            
            Vector3 eelSpawnPosition = (Vector3)nearestNode.position;

            GameObject EelBossInstance = Instantiate(EelBossPrefab, eelSpawnPosition, Quaternion.identity);
            // 4. Set MovetoPoint method in EelBoss object
            // THIS IS THE END POSITION OF THE EEL, FROM THE 
            EelBossInstance.GetComponent<BigEelController>().MovetoPoint(new Vector2(-8.63f, -10.33f));
            // 5. When Eel boss collides with bros, bros get destroyed.
            // 6. When Eel boss has ended movement, destroy object
            // 7. Return movement & controls to player
        }
        if (_sTriggerName == "2_LevelTransition") 
        {
            // This is where the level transition trigger occurs
            Debug.Log("Level of level triggered");
            // TODO add a fade out for level transitions.
            SceneManager.LoadScene("2_Level2");
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
        float zoomOutSize = 10f; // Adjust this value based on your level size
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
    }
}
