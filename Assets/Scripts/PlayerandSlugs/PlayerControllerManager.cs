using UnityEngine;
using Pathfinding;

/// <summary>
/// This class manages the player controller, primarily the input and movement of the player. It utilizes the
/// IsoSpriteDirectionManager to change animations throughout movement. 
/// </summary>
public class PlayerControllerManager : MonoBehaviour
{
    private AIPath aiPath; // Reference to A* pathfinding component
    private IsoSpriteDirectionManager spriteDirectionManager; // Reference to IsoSpriteDirectionManager

    private Vector3 targetPosition;

    [SerializeField] public RuntimeAnimatorController idleAnimatorController;
    [SerializeField] public RuntimeAnimatorController throwAnimatorController;
    void Start()
    {
        aiPath = GetComponent<AIPath>(); // Get the AIPath component
        spriteDirectionManager = GetComponent<IsoSpriteDirectionManager>(); // Get the IsoSpriteDirectionManager component
        targetPosition = transform.position;
    }

    private float updateInterval = 1.0f; // Interval in seconds to update the target position
    private float timeSinceLastUpdate = 0.0f; // Timer to track the interval

    void Update()
    {
        // Check if the right mouse button is held down or has been clicked
        if (Input.GetMouseButton(1) && ManageGameplay.Instance.PlayerCanIssueMoveCommands)
        {
            // Increment the timer by the time elapsed since the last frame
            timeSinceLastUpdate += Time.deltaTime;

            // If the right mouse button was just pressed or the update interval has passed
            if (Input.GetMouseButtonDown(1) || timeSinceLastUpdate >= updateInterval)
            {
                // Reset the timer
                timeSinceLastUpdate = 0.0f;

                // Get the mouse position in world space
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0; 

                // Set the target position for the A* pathfinding system
                targetPosition = mousePosition;
                aiPath.destination = targetPosition; // Tell A* where to move
            }
        }
        else
        {
            // Reset the timer when the mouse button is not held
            timeSinceLastUpdate = 0.0f;
        }

        // Get movement direction
        Vector3 direction = aiPath.velocity.normalized;

        // Update sprite direction
        if (spriteDirectionManager != null)
        {
            spriteDirectionManager.UpdateSpriteDirection(direction);
        }
    }

}