using UnityEngine;
using Pathfinding;
using UnityEngine.Serialization;

/// <summary>
/// This class manages the player controller, primarily the input and movement of the player. It utilizes the
/// IsoSpriteDirectionManager to change animations throughout movement. 
/// </summary>
public class PlayerControllerManager : MonoBehaviour
{
    private AIPath aiPath; // Reference to A* pathfinding component
    private IsoSpriteDirectionManager spriteDirectionManager; // Reference to IsoSpriteDirectionManager

    private Vector3 targetPosition;
    [SerializeField] GameObject m_destinationObject; // Object that shows the player where theyre moving
    float m_destinationOpacity;

    private Animator playerAnimator;
    [SerializeField] public RuntimeAnimatorController moveAnimatorController;
    [SerializeField] public RuntimeAnimatorController throwAnimatorController;
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
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
                m_destinationOpacity = 3f;
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

        if (m_destinationObject != null)
        {
            m_destinationOpacity -= Time.deltaTime;
            if (m_destinationOpacity < 0) m_destinationOpacity = 0;
            m_destinationObject.transform.position = targetPosition;
            SpriteRenderer renderer = m_destinationObject.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = new Color(1, 1, 1, m_destinationOpacity);
            }
        }

        // Get movement direction
        Vector3 direction = aiPath.velocity.normalized;

        // Update sprite direction
        if (spriteDirectionManager != null)
        {
            spriteDirectionManager.UpdateSpriteDirection(direction);
        }
    }
    public void SwitchToMoveAnimatorController()
    {
        if (playerAnimator != null && moveAnimatorController != null)
        {
            int direction = spriteDirectionManager.GetCurrentDirection();
            playerAnimator.runtimeAnimatorController = moveAnimatorController;
            
            string animationName = spriteDirectionManager.GetCurrentDirection() + "_JELLYFISH_" + 
                                   GetComponent<IsoSpriteDirectionManager>().GetDirectionName(spriteDirectionManager.GetCurrentDirection());
            playerAnimator.Play(animationName, 0, 0f); // Start at beginning of animation
            
            spriteDirectionManager.SetDirection(direction);
        }
        else
        {
            Debug.LogWarning("Player Animator or Idle Animator Controller is not assigned.");
        }
    }
}