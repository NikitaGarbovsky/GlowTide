using System;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

/// <summary>
/// This class manages all functionality related to the different states of the SeaSlugBroFollower,
/// It handles how the seaslug acts, collides, moves, follows and allows associated variables to be manipulated.
/// </summary>
public class SeaSlugBroFollower : MonoBehaviour
{
    // Enum statemachine to represent the various states fo the slug.
    public enum ESlugState
    {
        FollowingPlayer,  // The slug is following the player
        MovingToObject,   // The slug is moving toward an assigned object
        Idle,             // The slug is idle and not moving
        Thrown            // The slug has been thrown and is moving in the air
    }

    // Current state of the sea slug (default is Idle)
    public ESlugState m_eCurrentState = ESlugState.Idle;

    private GameObject m_goTargetObject;  // Object that the slug will move toward
    private GameObject m_goPlayer;        // Reference to the player 
    private AIPath m_aiPath;              // Reference to the A* Pathfinding component on this object
    private Rigidbody2D m_rbSlug;         // Rigidbody for handling physics interactions

    [Header("Movement Speeds")]
    public float m_fChaseSpeed = 3f;   // Speed when chasing the player
    public float m_fWanderSpeed = 1f;  // Speed when wandering
    public float m_fMoveSpeed = 5f;    // Speed when moving toward an object
    public float m_fThrowSpeed = 10f;  // Speed when thrown

    [Header("Throwing Settings")]
    public float m_fThrowDistance = 10f;  // Maximum distance the slug can be thrown
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject trailBubblesPrefab; // the prefab of the bubbles effect when thrown.

    private GameObject trailBubblesInstance;
    
    private float m_fWaitTime;                  // Time to wait when wandering before choosing a new position
    private Vector3 m_v3CorrectedPlayerPosition; // Adjusted position slightly behind the player

    private Vector2 m_v2ThrownTargetPosition;    // The target position when the slug is thrown

    // Event triggered when the slug reaches its target object
    public event Action<SeaSlugBroFollower> OnReachedTarget;

    private void Start()
    {
        // Grab the reference to the player through the gamemanager 
        m_goPlayer = ManageGameplay.Instance.playerCharacter;

        // Initialize references to the AIPath component and Rigidbody2D
        m_aiPath = GetComponent<AIPath>();
        m_rbSlug = GetComponent<Rigidbody2D>();

        // Initialize wait time to 0
        m_fWaitTime = 0;
    }

    // Called every fixed time step (used for physics-related updates)
    private void FixedUpdate()
    {
        // Basic state machine switcher using the statemachine enum
        switch (m_eCurrentState)
        {
            case ESlugState.FollowingPlayer:
                FollowPlayer();
                break;
            case ESlugState.MovingToObject:
                MoveToObject();
                break;
            case ESlugState.Thrown:
                MoveToThrownPosition();
                break;
            case ESlugState.Idle:
                Wander();
                break;
        }
    }

    // Function to follow the player
    private void FollowPlayer()
    {
        if (m_goPlayer != null)
        {
            // Adjust position to follow slightly behind the player
            m_v3CorrectedPlayerPosition = new Vector3(m_goPlayer.transform.position.x, m_goPlayer.transform.position.y - 0.5f);

            // Ensure the AIPath is enabled for pathfinding
            if (!m_aiPath.enabled)
            {
                m_aiPath.enabled = true;
            }

            // Set the AI pathfinding destination to the corrected player position
            m_aiPath.destination = m_v3CorrectedPlayerPosition;

            // Adjust speed based on distance from the player
            float fDistanceToPlayer = Vector2.Distance(transform.position, m_goPlayer.transform.position);
            if (fDistanceToPlayer > 1.5f)
            {
                m_aiPath.maxSpeed = m_fChaseSpeed;
            }
            else
            {
                m_aiPath.maxSpeed = m_fWanderSpeed;
            }
        }
    }

    // Function to randomly wander when idle
    private void Wander()
    {
        if (m_fWaitTime <= 0)
        {
            // Randomly choose a nearby position to move toward
            float fXPos = Random.Range(-0.5f, 0.5f);
            float fYPos = Random.Range(-0.5f, 0.5f);
            m_aiPath.destination = new Vector3(transform.position.x + fXPos, transform.position.y + fYPos);

            // Set wait time for the next random move
            m_fWaitTime = Random.Range(5, 7);
        }

        // Decrease wait time each frame
        m_fWaitTime -= Time.deltaTime;

        // Set wandering speed
        m_aiPath.maxSpeed = m_fWanderSpeed;
    }

    // Public function to assign an object for the slug to move toward
    public void MoveToAssignedObject(GameObject _goTarget)
    {
        m_goTargetObject = _goTarget;
        m_eCurrentState = ESlugState.MovingToObject;
    }

    // Function to move toward an assigned object TODO maybe change this in the future, is it rally needed?
    private void MoveToObject()
    {
        if (m_goTargetObject != null)
        {
            // Set AI pathfinding destination to the object's position
            m_aiPath.destination = m_goTargetObject.transform.position;
            m_aiPath.maxSpeed = m_fMoveSpeed;

            // Check if the slug has arrived at the target
            if (Vector2.Distance(transform.position, m_goTargetObject.transform.position) < 0.1f)
            {
                m_eCurrentState = ESlugState.Idle;

                // Snap to the exact target position
                transform.position = m_goTargetObject.transform.position;

                // Invoke the event to notify that the target has been reached
                OnReachedTarget?.Invoke(this);
            }
        }
    }

    // Function to start following the player
    public void StartFollowingPlayer()
    {
        m_eCurrentState = ESlugState.FollowingPlayer;
    }

    // Function to stop following the player and become idle
    public void StopFollowingPlayer()
    {
        m_eCurrentState = ESlugState.Idle;
    }
    
    // Handle collisions while the slug is in the thrown state
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_eCurrentState == ESlugState.Thrown)
        {
            if (trailBubblesInstance != null)
            {
                // Deactivate the bubbles when the target has reached location.
                Destroy(trailBubblesInstance);
            }
            
            Debug.Log($"Seaslug collided with {collision.gameObject.name}");

            // Stop the slug's movement and change its state to Idle
            m_eCurrentState = ESlugState.Idle;
            
            // Revert the slug's layer back to "Slug"
            gameObject.layer = LayerMask.NameToLayer("Slug");
            
            // Returns the seaslug to dynamic, so it can collide with things again.
            m_rbSlug.bodyType = RigidbodyType2D.Dynamic;
            
            // Snap the slug's position to the collision point
            m_rbSlug.position = collision.contacts[0].point;

            // Re-enable the AIPath component
            m_aiPath.enabled = true;
            m_aiPath.destination = gameObject.transform.position;
        }
    }

    // Public function to throw the slug toward a target position
    public void ThrowTowards(Vector2 v2TargetPosition)
    {
        if (trailBubblesPrefab != null)
        {
            trailBubblesInstance = Instantiate(trailBubblesPrefab, transform.position, Quaternion.identity, transform);
        }
        
        // Calculate direction and final target position
        Vector2 v2StartPosition = m_goPlayer.transform.position;
        Vector2 v2Direction = (v2TargetPosition - v2StartPosition).normalized;
        m_v2ThrownTargetPosition = v2StartPosition + v2Direction * m_fThrowDistance;
        
        // Change the slug's layer to "SlugNoCollide" (this is so it doesnt collide with other slugs while being thrown)
        gameObject.layer = LayerMask.NameToLayer("SlugNoCollide");
        
        // Move the slug to the player's position before the throw
        m_rbSlug.position = v2StartPosition;

        // Set the state to Thrown
        m_eCurrentState = ESlugState.Thrown;

        // Disable AIPath to handle manual movement
        m_aiPath.enabled = false;

        // Set Rigidbody2D to Kinematic for controlled movement
        m_rbSlug.bodyType = RigidbodyType2D.Kinematic;
        m_rbSlug.useFullKinematicContacts = true;

        // Reset the velocity to prevent unwanted movement
        m_rbSlug.velocity = Vector2.zero;
    }

    // Function to handle movement while the slug is thrown
    private void MoveToThrownPosition()
    {
        Vector2 v2CurrentPosition = m_rbSlug.position;
        Vector2 v2Direction = (m_v2ThrownTargetPosition - v2CurrentPosition).normalized;
        float fDistanceToTarget = Vector2.Distance(v2CurrentPosition, m_v2ThrownTargetPosition);
        float fMoveDistance = m_fThrowSpeed * Time.fixedDeltaTime;

        // Check if the slug has reached or overshot the target
        if (fMoveDistance >= fDistanceToTarget)
        {
            // Snap to the target position and trigger the "reach" logic
            m_rbSlug.MovePosition(m_v2ThrownTargetPosition);
            OnReachThrownTarget();
        }
        else
        {
            // Move toward the target
            Vector2 v2NewPosition = v2CurrentPosition + v2Direction * fMoveDistance;
            m_rbSlug.MovePosition(v2NewPosition);
        }
    }

    // Function triggered when the slug reaches the thrown target position
    private void OnReachThrownTarget()
    {
        if (trailBubblesInstance != null)
        {
            // Deactivate the bubbles when the target has reached location.
            Destroy(trailBubblesInstance);
        }
        // Set the slug's state to Idle
        m_eCurrentState = ESlugState.Idle;
        
        // Revert the slug's layer back to "Slug"
        gameObject.layer = LayerMask.NameToLayer("Slug");
        
        // Returns the seaslug to dynamic, so it can collide with things again.
        m_rbSlug.bodyType = RigidbodyType2D.Dynamic;
        
        // Re-enable AIPath for future movements
        m_aiPath.enabled = true;
        // When they've reached their destination after being thrown,
        // set the aiPath destination to it (so they stay in place)
        m_aiPath.destination = transform.position;
    }
}
