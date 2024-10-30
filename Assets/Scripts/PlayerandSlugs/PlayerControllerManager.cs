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

    void Start()
    {
        aiPath = GetComponent<AIPath>(); // Get the AIPath component
        spriteDirectionManager = GetComponent<IsoSpriteDirectionManager>(); // Get the IsoSpriteDirectionManager component
        targetPosition = transform.position;
    }

    void Update()
    {
        // Handle movement input
        if (Input.GetMouseButtonDown(1) && ManageGameplay.Instance.PlayerCanIssueMoveCommands)
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; 

            // Set the target position for the A* pathfinding system
            targetPosition = mousePosition;
            aiPath.destination = targetPosition; // Tell A* where to move
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