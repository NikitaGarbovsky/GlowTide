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
    // Add a reference to the door in the level 
    [SerializeField] private GameObject goDoor;
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_CallingBros") // This is the name of the gameobject that is being triggered.
        {
            StartCoroutine(CallBrosSequence());
        }
        if (_sTriggerName == "1_ThrowingBros") 
        {
            // This is where the "Throwing bros" trigger occurs
            StartCoroutine(ThrowingBrosSequence());
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

        // movement pop-up text is disabled and calling pop-up text is displayed
        m_popUpText[0].enabled= false;
        m_popUpText[1].enabled = true;

        // This is the offset that the camera moves towards. 
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
        m_popUpText[1].enabled = false;
    }

    private IEnumerator ThrowingBrosSequence()
    {
        Debug.Log("Throwing bros triggered");
        // Scriptable event for Throwing bros:
        // 1. Take movement controls away from player,
        ManageGameplay.Instance.RemovePlayerControl();
        // 2. Camera pans towards door
        Vector2 targetOffset = goDoor.transform.position - ManageGameplay.Instance.playerCharacter.transform.position;
        
        // 3. Camera pans to the door
        yield return StartCoroutine(ManageGameplay.Instance.PanCamera(targetOffset, 2f));

        ManageGameplay.Instance.PlayerCanThrowBros = true;
        // 4. Wait until the door has fully disappeared
        m_popUpText[2].enabled = true;
        DoorInteractiveObject doorInteractiveObject = goDoor.GetComponent<DoorInteractiveObject>();

        bool doorDestroyed = false;

        if (doorInteractiveObject != null)
        {
            // Subscribe to the OnDoorDestroyed event
            doorInteractiveObject.OnDoorDestroyed += () => { doorDestroyed = true; };
        }
        else
        {
            Debug.LogError("DoorInteractiveObject component not found on the door GameObject.");
            // Handle the error to avoid an infinite loop
            doorDestroyed = true;

        }

        // Wait until the door is destroyed
        while (!doorDestroyed)
        {
            yield return null;
        }
        
        // 5. When the door has been destroyed, return movement control back to player,
        ManageGameplay.Instance.ReturnPlayerControl();
        // 6. Return camera to normal (pan back to the player)
        yield return StartCoroutine(ManageGameplay.Instance.PanCamera(Vector2.zero, 2f));
        m_popUpText[2].enabled = false;
    }            
}

