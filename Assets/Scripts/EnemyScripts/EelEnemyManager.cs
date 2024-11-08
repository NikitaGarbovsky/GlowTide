using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class manages the Enemy Eel functionality.
/// </summary>
public class EelEnemyManager : MonoBehaviour
{ 
    public enum EelState
    {
        Patrol, // Patroll Waypoint List
        Investigate, // Stop and Look at Last Player Location 
        Chase, // Chase Player
        Stunned, // Stop Eel For Stun Time
        Returning // Returns to the Closest waypoint after stunned
    }

    [Header("Eel")]
    [SerializeField] public EelState m_currentEelState; // Current State of the Eel
    [SerializeField] GameObject m_eel; // Game Object of the Eel
    [SerializeField] GameObject m_player; // Refference to the Player
    [SerializeField] AIPath m_aiPath; // AIPath Component
    [SerializeField] IsoSpriteDirectionManager m_directionManager; // IsoSpriteDirectionManager Script for Animation 
    [SerializeField] bool m_ShowGizmos = false;

    [Header("UI")]
    [SerializeField] Image m_UIImage; // Canvas Image for worldspace ui
    [SerializeField] Vector2 m_imageOffset; // offset from the Eel Center
    [SerializeField] Sprite m_investigatingSprite; // Sprite for Investigating
    [SerializeField] Sprite m_chaseSprite; // Sprite for Chaseing
    [SerializeField] Sprite m_stunnedSprite; // Sprite for Stunned

    [Header("Patrol")]
    [SerializeField, Min(0.0f)] float m_speed; // Movement Speed
    [SerializeField] List<Transform> m_waypoints; // List of all Waypoints to Patrol
    int m_waypointIndex; // Index of the Current Waypoint

    [Header("Investigate")]
    [SerializeField, Min(0.0f)] float m_rotationTime; // Modifies how long it takes to look at Player Position when spotted
    [SerializeField, Min(0.0f)] float m_investigationTime; // Modifies how long it takes to look at Player Position when spotted
    float m_currentRotationTime = 0; // Variable for Rotational Lerping
    float m_eelStartAngle = 0; // Variable for Rotational Lerping

    [Header("Chase")]
    [SerializeField] float m_chaseSpeedModifier; // Increase the Speed while Chasing
    [SerializeField, Min(0.0f)] float m_killDistance; // Distance to Kill Player (Restart Level)
    [SerializeField] float m_killCircleOffset; // Distance to Kill Player (Restart Level)
    Vector2 m_killCirclePosition; // Poistion of kill circle

    [Header("Stunned")]
    [SerializeField] float m_stunTime; // Time the Eel is stunned
    float m_currentStunTime = 0; // Current Stun Time

    [Header("View Cone")]
    [SerializeField, Min(0.0f)] float m_sightRadius; // Max Distance the Eel can See
    [SerializeField, Min(0.0f)] float m_hardSightAngle; // Angle of Sight Where the player is chase
    [SerializeField, Min(0.0f)] float m_softSightAngle; // Agnel of Sight Where the playher is being investigated

    bool m_canSee = true; // If the Eel can See (turned off during Stun and Return)
    float m_eelAngle; // Angle the Eel is Facing
    float m_angleToPlayer; // Angle Between the Eel and Player
    float m_lastPlayerAngle; // When Spotted Last Known Position of the Player


    private void Start()
    {
        m_aiPath.enabled = false;
        m_player = ManageGameplay.Instance.playerCharacter;
    }

    private void Update()
    {
        switch (m_currentEelState)
        {
            case EelState.Patrol:
                m_canSee = true;
                PatrolPoints();
                CheckForPlayerInSight();
                m_UIImage.enabled = false;
                break;
            case EelState.Investigate:
                m_canSee = true;
                LookForPlayer();
                CheckForPlayerInSight();
                m_UIImage.enabled = true;
                m_UIImage.sprite = m_investigatingSprite;
                break;
            case EelState.Chase:
                m_canSee = true;
                ChasePlayer();
                m_UIImage.enabled = true;
                m_UIImage.sprite = m_chaseSprite;
                break;
            case EelState.Stunned:
                m_canSee = false;
                Stunned();
                m_UIImage.enabled = true;
                m_UIImage.sprite = m_stunnedSprite;
                break;
            case EelState.Returning:
                m_canSee = false;
                Returning();
                break;
            default:
                break;
        }

        // Update UI Position
        float spriteAngle = ((float)m_directionManager.GetCurrentDirection() / 16.0f) * (2 * Mathf.PI);
        m_UIImage.transform.position = new Vector2(m_eel.transform.position.x + (m_imageOffset.x * Mathf.Cos(spriteAngle)) ,
                                                   m_eel.transform.position.y + (m_imageOffset.y * Mathf.Sin(spriteAngle) + 0.5f));

        // Update Kill Position
        m_killCirclePosition = new Vector2(m_eel.transform.position.x + (m_killCircleOffset * Mathf.Cos(m_eelAngle)),
                                           m_eel.transform.position.y + (m_killCircleOffset * Mathf.Sin(m_eelAngle)));
        // Kill Player
        if (Vector2.Distance(m_player.transform.position, m_killCirclePosition) <= m_killDistance && m_canSee)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Restart Level");
        }
        
        // Collide with Geyser
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_eel.transform.position, m_killDistance);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Geyser") && m_currentEelState != EelState.Stunned && m_currentEelState != EelState.Returning)
            {
                m_currentEelState = EelState.Stunned;
                m_aiPath.enabled = false;
                break;
            }
        }
    }

    // Moves the Eel to the Waypoint at m_waypointIndex
    // Once at the Waypoint - Update m_waypointIndex & m_eelAngle
    void PatrolPoints()
    {
        // Move Eel
        m_eel.transform.position = Vector2.MoveTowards(m_eel.transform.position, m_waypoints[m_waypointIndex].position, m_speed * Time.deltaTime);
        // Update Direction
        m_directionManager.UpdateSpriteDirection(m_waypoints[m_waypointIndex].position - m_eel.transform.position);
        // Update m_eelAngle
        m_eelAngle = Mathf.Atan2(m_waypoints[m_waypointIndex].position.y - m_eel.transform.position.y,
                                 m_waypoints[m_waypointIndex].position.x - m_eel.transform.position.x);
        if (Vector2.Distance(m_eel.transform.position, m_waypoints[m_waypointIndex].position) <= 0)
        {
            // Increment m_waypointIndex
            m_waypointIndex++;
            // Stop m_waypointIndex from going out of bounds
            if (m_waypointIndex > m_waypoints.Count - 1) 
            {
                m_waypointIndex = 0;
            }
        }
    }

    void CheckForPlayerInSight()
    {
        if (m_canSee)
        {
            // Update m_angleToPlayer
            m_angleToPlayer = Mathf.Atan2(m_player.transform.position.y - m_eel.transform.position.y,
                                            m_player.transform.position.x - m_eel.transform.position.x);


            if (Vector2.Distance(m_player.transform.position, m_eel.transform.position) <= m_sightRadius)
            {
                // Hard Sight --- Chase
                if (m_angleToPlayer <= m_eelAngle + m_hardSightAngle && m_angleToPlayer >= m_eelAngle - m_hardSightAngle)
                {
                    m_currentEelState = EelState.Chase;
                }
                // Soft Sight --- Investigate
                else if (m_angleToPlayer <= m_eelAngle + m_softSightAngle && m_angleToPlayer >= m_eelAngle - m_softSightAngle)
                {
                    m_lastPlayerAngle = m_angleToPlayer;
                    m_currentRotationTime = 0;
                    m_eelStartAngle = m_eelAngle;
                    m_currentEelState = EelState.Investigate;
                }
            }
        }
    }

    void LookForPlayer()
    {
        Debug.Log(m_currentRotationTime);
        m_currentRotationTime += Time.deltaTime / m_rotationTime;
        if (m_currentRotationTime < 1)
        {
            m_eelAngle = Mathf.LerpAngle(m_eelStartAngle, m_lastPlayerAngle, m_currentRotationTime);
            Vector3 newDirection = new Vector3(Mathf.Cos(m_eelAngle), Mathf.Sin(m_eelAngle), 0);
            m_directionManager.UpdateSpriteDirection(newDirection);
        }
        else if (m_currentRotationTime > 1.0f + m_investigationTime)
        {
            m_currentEelState = EelState.Patrol;
        }
    }

    void ChasePlayer()
    {
        m_aiPath.enabled = true;
        m_aiPath.destination = new Vector3(m_player.transform.position.x, m_player.transform.position.y, 0);
        m_eelAngle = Mathf.Atan2(m_aiPath.velocity.normalized.y, m_aiPath.velocity.normalized.x);
        m_aiPath.maxSpeed = m_speed * m_chaseSpeedModifier;
        m_directionManager.UpdateSpriteDirection(m_aiPath.velocity.normalized);
    }

    void Stunned()
    { 
        m_currentStunTime += Time.deltaTime;
        if (m_currentStunTime > m_stunTime)
        {
            m_currentStunTime = 0.0f;
            FindToClosestWaypoint();
            m_currentEelState = EelState.Returning;
        }
    }

    void FindToClosestWaypoint()
    {
        float smallestDist = -Mathf.Infinity;
        int index = 0;
        foreach (Transform waypoint in m_waypoints)
        {
            if (Vector2.Distance(m_eel.transform.position, waypoint.position) < smallestDist)
            {
                smallestDist = Vector2.Distance(m_eel.transform.position, waypoint.position);
                m_waypointIndex = index;
            }
            index++;
        }
        m_aiPath.enabled = true;
        m_aiPath.destination = m_waypoints[m_waypointIndex].position;
    }

    void Returning()
    {
        
        m_eelAngle = Mathf.Atan2(m_aiPath.velocity.normalized.y, m_aiPath.velocity.normalized.x);
        m_directionManager.UpdateSpriteDirection(m_aiPath.velocity.normalized);

        if (Vector2.Distance(m_eel.transform.position, m_waypoints[m_waypointIndex].position) <= (m_aiPath.endReachedDistance + m_aiPath.slowdownDistance))
        {
            m_aiPath.enabled = false;
            // Increment m_waypointIndex
            m_waypointIndex++;
            // Stop m_waypointIndex from going out of bounds
            if (m_waypointIndex > m_waypoints.Count - 1)
            {
                m_waypointIndex = 0;
            }
            m_currentEelState = EelState.Patrol;
        }
    }

    private void OnDrawGizmos()
    {
        if (m_ShowGizmos)
        {
            // Draw Line To Player
            Gizmos.color = Color.white;
            if (m_angleToPlayer <= m_eelAngle + m_hardSightAngle && m_angleToPlayer >= m_eelAngle - m_hardSightAngle && m_canSee)
            {
                Gizmos.color = Color.red;
            }
            else if (m_angleToPlayer <= m_eelAngle + m_softSightAngle && m_angleToPlayer >= m_eelAngle - m_softSightAngle && m_canSee)
            {
                Gizmos.color = Color.yellow;
            }
            Vector3 gizmoLine = new Vector3(Mathf.Cos(m_angleToPlayer) * m_sightRadius, Mathf.Sin(m_angleToPlayer) * m_sightRadius, 0);
            gizmoLine += m_eel.transform.position;
            Gizmos.DrawLine(m_eel.transform.position, gizmoLine);

            // Draw Kill Circle
            Gizmos.color = (m_canSee) ? Color.red : Color.green;
            Gizmos.DrawWireSphere(m_killCirclePosition, m_killDistance);
            gizmoLine = new Vector3(Mathf.Cos(m_eelAngle + m_hardSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle + m_hardSightAngle) * m_sightRadius, 0);
            gizmoLine += m_eel.transform.position;

            // Draw Hard Sight Lines
            Gizmos.DrawLine(m_eel.transform.position, gizmoLine);
            gizmoLine = new Vector3(Mathf.Cos(m_eelAngle - m_hardSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle - m_hardSightAngle) * m_sightRadius, 0);
            gizmoLine += m_eel.transform.position;
            Gizmos.DrawLine(m_eel.transform.position, gizmoLine);

            // Draw Soft Sight Lines
            Gizmos.color = (m_canSee) ?  Color.yellow : Color.green;
            gizmoLine = new Vector3(Mathf.Cos(m_eelAngle + m_softSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle + m_softSightAngle) * m_sightRadius, 0);
            gizmoLine += m_eel.transform.position;
            Gizmos.DrawLine(m_eel.transform.position, gizmoLine);
            gizmoLine = new Vector3(Mathf.Cos(m_eelAngle - m_softSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle - m_softSightAngle) * m_sightRadius, 0);
            gizmoLine += m_eel.transform.position;
            Gizmos.DrawLine(m_eel.transform.position, gizmoLine);
        }
    }
}
