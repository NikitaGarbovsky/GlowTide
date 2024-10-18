using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

public class SeaSlugBroFollower : MonoBehaviour
{
    // Represents the current state of the slug
    public enum SlugState
    {
        FollowingPlayer,
        MovingToObject,
        Idle
    }

    // The current state (starts in Idle)
    public SlugState currentState = SlugState.Idle;
    private Transform targetObject;
    private GameObject m_Player;  // Reference to the player (seahorse)
    private AIPath aiPath;    // Reference to A* Pathfinding component on this gameobject
    private Rigidbody2D rb;   // Rigidbody for handling physics

    public float m_chaseSpeed = 3f; // Speed when chasing the player
    public float m_wanderSpeed = 1f; // Speed when wandering
    public float moveSpeed = 5f;  // Speed when moving to assigned object

    public bool m_wandering = true;
    private float m_waitTime;

    private Vector3 CorrectedPlayerPosition;

    // Define an event for when the slug reaches its target
    public event Action<SeaSlugBroFollower> OnReachedTarget;
    private void Start()
    {
        // Find the player gameobject 
        m_Player = GameObject.FindWithTag("Player"); 
        aiPath = GetComponent<AIPath>(); // Get the A* pathfinding component
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component for movement control
        m_waitTime = 0;
    }

    private void Update()
    {
        switch (currentState)
        {
            case SlugState.FollowingPlayer:
                FollowPlayer();
                break;
            case SlugState.MovingToObject:
                MoveToObject();
                break;
            case SlugState.Idle:
                Wander();
                break;
        }
    }

    private void FollowPlayer()
    {
        aiPath.enabled = true;
        if (m_Player != null)
        {
            // Adjust position to follow slightly behind the player
            CorrectedPlayerPosition = new Vector3(m_Player.transform.position.x, m_Player.transform.position.y - 0.5f);
            aiPath.destination = CorrectedPlayerPosition;

            // Adjust speed based on distance to the player
            float distanceToPlayer = Vector2.Distance(transform.position, m_Player.transform.position);
            if (distanceToPlayer > 1.5f)
            {
                aiPath.maxSpeed = m_chaseSpeed;
            }
            else
            {
                aiPath.maxSpeed = m_wanderSpeed; 
            }
        }
    }

    private void Wander()
    {
        if (m_wandering)
        {  
            aiPath.enabled = true;
            if (m_waitTime <= 0)
            {
                // Randomly choose a nearby position to move to
                float xPos = Random.Range(-0.5f, 0.5f);
                float yPos = Random.Range(-0.5f, 0.5f);
                aiPath.destination = new Vector3(transform.position.x + xPos, transform.position.y + yPos);
                m_waitTime = Random.Range(5, 7);
            }
            m_waitTime -= Time.deltaTime;
            aiPath.maxSpeed = m_wanderSpeed;
        }
        else
        {
            aiPath.enabled = false;
        }
    }

    public void MoveToAssignedObject(Transform target)
    {
        targetObject = target;
        currentState = SlugState.MovingToObject;
        aiPath.enabled = true;
        aiPath.destination = targetObject.position;
        aiPath.maxSpeed = moveSpeed;
    }

    private void MoveToObject()
    {
        if (targetObject != null)
        {
            aiPath.destination = targetObject.position;
            aiPath.maxSpeed = moveSpeed;

            // Check if arrived at the target
            if (Vector2.Distance(transform.position, targetObject.position) < 0.1f)
            {
                currentState = SlugState.Idle;
                aiPath.enabled = false;

                // Snap to the exact spot
                transform.position = targetObject.position;
                
                // Invoke the event to notify that the slug has reached its target
                OnReachedTarget?.Invoke(this);
            }
        }
    }

    // Method to start following the player
    public void StartFollowingPlayer()
    {
        currentState = SlugState.FollowingPlayer;
    }

    // Method to stop following the player and become idle
    public void StopFollowingPlayer()
    {
        currentState = SlugState.Idle;
        aiPath.enabled = false;
    }
}
