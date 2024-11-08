using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level5Triggers : LevelTriggers
{
    private Tilemap tilemap; // Reference to the Tilemap
    private Vector3 levelCenter; // Center position of the level
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_ZoomOutAndShowLevel") // This is the name of the gameobject that is being triggered.
        {
            StartCoroutine(ZoomOutAndShowLevel());
        }
        if (_sTriggerName == "1_LevelTransition") 
        {
            // This is where the level transition trigger occurs
            Debug.Log("Level of level triggered");

            ManageGameplay.Instance.LoadSceneWithFade("6_Level6");
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
    }
}
