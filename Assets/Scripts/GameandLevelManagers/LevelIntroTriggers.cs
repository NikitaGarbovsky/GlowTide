using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
/// <summary>
/// This class inherited from the LevelTriggers class, it executes functionality related to individual triggers
/// associated with the INTRO level.
/// </summary>
public class LevelIntroTriggers : LevelTriggers
{
    [SerializeField] private List<TextMeshProUGUI> m_popUpText;

    // Add a reference to the seaslug bro that needs to be called
    [SerializeField] private GameObject seaslugBroToCall;
    
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_CallingBros") // This is the name of the gameobject that is being triggered.
        {
            StartCoroutine(CallBrosSequence());
        }
        if (_sTriggerName == "1_ThrowingBros") 
        {
            // This is where the "Throwing bros" trigger occurs
            Debug.Log("Throwing bros triggered");
            // Scriptable event for Throwing bros:
            // 1. Take movement controls away from player,
            // 2. Camera pans towards door
            // 3. Prompt player to mouse over door, press left mouse to throw sea slug bro,
            // 4. Door disappears,
            // 5. Sea slug bro returns to player
            // 6. When bro has reached player, return movement control back to player,
            // 7. Return camera to normal
        }
        if (_sTriggerName == "2_LevelTransition") 
        {
            // This is where the level transition trigger occurs
            Debug.Log("Level of level triggered");
            // TODO add a fade out for level transitions.
            SceneManager.LoadScene("1_Level1");
        }
        
    }
    private IEnumerator CallBrosSequence()
    {
        // This is where the "Calling bros" trigger occurs
        Debug.Log("Calling bros triggered");
        // Scriptable event for Calling bros:
        // 1. Take movement controls away from player,
        ManageGameplay.Instance.RemovePlayerControl();
        // 2. Calculate the offset between the seaslug and the player
        
        Vector2 targetOffset = seaslugBroToCall.transform.position - ManageGameplay.Instance.playerCharacter.transform.position;

        // 3. Camera pans to the seaslug bro
        yield return StartCoroutine(ManageGameplay.Instance.PanCamera(targetOffset, 2f));
        
        // 4. TODO Pop up box displays prompt to position mouse towards bro and press e to call

        ManageGameplay.Instance.PlayerCanCallBros = true;
        
        // Wait until the player presses 'E' and the seaslug is added to the player's slug list
        PlayerSlugManager playerSlugManager = ManageGameplay.Instance.playerCharacter.GetComponent<PlayerSlugManager>();

        while (!playerSlugManager.m_lAssignedSlugs.Contains(seaslugBroToCall))
        {
            // Wait for the next frame
            yield return null;
        }
        // 4. Bro moves towards player,
        // 5. Return movement controls back to player
        ManageGameplay.Instance.ReturnPlayerControl();
        
        // 6. Return camera to normal (pan back to the player)
        yield return StartCoroutine(ManageGameplay.Instance.PanCamera(Vector2.zero, 2f));
    }
}

