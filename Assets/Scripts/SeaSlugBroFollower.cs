using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SeaSlugBroFollower : MonoBehaviour
{
    public GameObject m_Player;  // Reference to the player (seahorse)
    private AIPath aiPath;    // Reference to A* Pathfinding component on this gameobject
    public bool m_bIsFollowingPlayer = true; // Bool to check if it's following the player
    private Rigidbody2D rb;   // Rigidbody for handling launching

    public float m_chaseSpeed;
    public float m_wanderSpeed;
    public bool m_wandering = true;
    private float m_waitTime;

    private Vector3 velocity; // For projectile movement
    public float moveSpeed = 5f;  // Speed when thrown
    public float friction = 0.9f; // Friction to reduce velocity over time

    private Vector3 CorrectedPlayerPosition;
    private void Start()
    {
        // Finds the player gameobject 
        m_Player = GameObject.FindWithTag("Player"); 
        aiPath = GetComponent<AIPath>(); // Get the A* pathfinding component
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody component for movement control
        m_bIsFollowingPlayer = false;
        m_waitTime = 0;
    }

    private void Update()
    {
        if (m_bIsFollowingPlayer)
        {
            // Continue following the player using A* pathfinding
            aiPath.enabled = true;
            if (m_Player != null)
            {
                CorrectedPlayerPosition =
                    new Vector3(m_Player.transform.position.x, (float)(m_Player.transform.position.y - 0.5f));
                aiPath.destination = CorrectedPlayerPosition;
                if (Vector2.Distance(transform.position, m_Player.transform.position) > 1.5f)
                {
                    aiPath.maxSpeed = m_chaseSpeed;
                }
                else
                {
                    aiPath.maxSpeed = m_wanderSpeed; 
                }
            }
        }
        else
        {
            if (m_wandering)
            {  
                if (m_waitTime <= 0)
                {
                    float xPos = Random.Range(-0.5f, 0.5f);
                    float yPos = Random.Range(-0.5f, 0.5f);
                    aiPath.destination = new Vector3(transform.position.x + xPos, transform.position.y + yPos);
                    m_waitTime = Random.Range(5, 7);
                }
                m_waitTime -= Time.deltaTime;
            }

            // Stop following the player and apply projectile logic
            //aiPath.enabled = false; // Disable pathfinding while launched
            //rb.velocity = velocity * moveSpeed; // Move using the current velocity
            //velocity -= velocity * friction * Time.deltaTime; // Apply friction to slow down

            //if (velocity.magnitude < 0.01f) // Stop if velocity is too small
            //{
            //    rb.velocity = Vector2.zero; // Stop movement
            //    velocity = Vector3.zero;    // Reset velocity
            //}
        }
    }

    // --- [ NOT BEING USED ] --- ///
    // Function to set the velocity and launch the seaslugbro
    //public void Launch(Vector3 direction)
    //{
    //    m_bIsFollowingPlayer = false; // Stop following the player when launched
    //    velocity = direction.normalized; // Set the velocity based on the input direction
    //    rb.isKinematic = false; // Enable physics for projectile movement
    //}

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    // Check if the player collides with the sea slug bro when it's stationary
    //    if (other.CompareTag("Player") && !m_bIsFollowingPlayer)
    //    {
    //        // Adds this gameobject (slug) to the players slug launcher list.
    //        m_Player.GetComponent<LaunchSeaSlugBro>().AddSlug(gameObject); 
    //        m_bIsFollowingPlayer = true;  // Start following the player again
            
    //        aiPath.enabled = true;
    //    }
    //}
    /// --- ^^^^^ --- ///
}
