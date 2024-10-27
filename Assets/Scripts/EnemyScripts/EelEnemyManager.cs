using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class manages the Enemy Eel functionality.
/// The Enemy Eels have the following functionality:
///
/// 1. A state machine to manage the patrolling, idling or pursuing states.
///     1.1. Patrolling state has the Eel patrol along a path that is customizable.
///     1.2. Idle state has the Eel idle in place.
///     1.3. Pursuing state has the Eel pursue the player character.
/// 2. Holds references to gameobjects (patrol points) that the eel pursues through.
/// 3. 
/// </summary>
public class EelEnemyManager : MonoBehaviour
{ 
    enum EelState
    {
        Patrol, // Patroll Waypoint List
        Investigate, // Stop and Look at Last Player Location 
        Chase, // Chase Player
        Stunned // Stop Eel For Stun Time
    }

    [Header("Eel")]
    [SerializeField] EelState m_currentEelState; // Current State of the Eel
    [SerializeField] GameObject m_eel; // Game Object of the Eel
    [SerializeField] GameObject m_player; // Refference to the Player
    [SerializeField] AIPath m_aiPath; // AIPath Component
    [SerializeField] IsoSpriteDirectionManager m_directionManager; // IsoSpriteDirectionManager Script for Animation 

    [Header("UI")]
    [SerializeField] Image m_UIImage;
    [SerializeField] float m_imageOffset;
    [SerializeField] Sprite m_investigatingSprite;
    [SerializeField] Sprite m_chaseSprite;
    [SerializeField] Sprite m_stunnedSprite;

    [Header("Patrol")]
    [SerializeField, Min(0.0f)] float m_speed; // Movement Speed
    [SerializeField] List<Transform> m_waypoints; // List of all Waypoints to Patrol
    int m_waypointIndex; // Index of the Current Waypoint

    [Header("Investigate")]
    [SerializeField, Min(0.0f)] float m_rotationTime; // Modifies how long it takes to look at Player Position when spotted
    float m_currentRotationTime = 0; // Variable for Rotational Lerping

    [Header("Chase")]
    [SerializeField] float m_chaseSpeedModifier;
    [SerializeField, Min(0.0f)] float m_killDistance; // Distance to Kill Player (Restart Level)
    [SerializeField] float m_killCircleOffset; // Distance to Kill Player (Restart Level)
    Vector2 m_killCirclePosition;

    [Header("Stunned")]
    [SerializeField] float m_stunTime;
    float m_currentStunTime = 0;

    [Header("View Cone")]
    [SerializeField, Min(0.0f)] float m_sightRadius; // Max Distance the Eel can See
    [SerializeField, Min(0.0f)] float m_hardSightAngle; // Angle of Sight Where the player is chase
    [SerializeField, Min(0.0f)] float m_softSightAngle; // Agnel of Sight Where the playher is being investigated

    float m_eelAngle; // Angle the Eel is Facing
    float m_angleToPlayer; // Angle Between the Eel and Player
    float m_lastPlayerAngle; // When Spotted Last Known Position of the Player


    private void Start()
    {
        m_aiPath.enabled = false;
    }

    private void Update()
    {
        switch (m_currentEelState)
        {
            case EelState.Patrol:
                PatrolPoints();
                CheckForPlayerInSight();
                m_UIImage.enabled = false;
                break;
            case EelState.Investigate:
                LookForPlayer();
                CheckForPlayerInSight();
                m_UIImage.enabled = true;
                m_UIImage.sprite = m_investigatingSprite;
                break;
            case EelState.Chase:
                ChasePlayer();
                m_UIImage.enabled = true;
                m_UIImage.sprite = m_chaseSprite;
                break;
            case EelState.Stunned:
                Stunned();
                m_UIImage.enabled = true;
                m_UIImage.sprite = m_stunnedSprite;
                break;
            default:
                break;
        }

        // Update UI Position
        float spriteAngle = ((float)m_directionManager.GetCurrentDirection() / 16.0f) * (2 * Mathf.PI);
        m_UIImage.transform.position = new Vector2(m_eel.transform.position.x + (m_imageOffset * Mathf.Cos(spriteAngle)) ,
                                                   m_eel.transform.position.y + (m_imageOffset * Mathf.Sin(spriteAngle) + 0.5f));

        // Update Kill Position
        m_killCirclePosition = new Vector2(m_eel.transform.position.x + (m_killCircleOffset * Mathf.Cos(m_eelAngle)),
                                           m_eel.transform.position.y + (m_killCircleOffset * Mathf.Sin(m_eelAngle)));
        // Kill Player
        if (Vector2.Distance(m_player.transform.position, m_killCirclePosition) <= m_killDistance 
            && m_currentEelState != EelState.Stunned)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Restart Level");
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
        if (Vector2.Distance(m_eel.transform.position, m_waypoints[m_waypointIndex].position) <= 0)
        {
            // Increment m_waypointIndex
            m_waypointIndex++;
            // Stop m_waypointIndex from going out of bounds
            if (m_waypointIndex > m_waypoints.Count - 1) 
            {
                m_waypointIndex = 0;
            }

            // Update m_eelAngle
            m_eelAngle = Mathf.Atan2(m_waypoints[m_waypointIndex].position.y - m_eel.transform.position.y,
                                     m_waypoints[m_waypointIndex].position.x - m_eel.transform.position.x);
        }
    }

    void CheckForPlayerInSight()
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
                m_aiPath.enabled = true;
            }
            // Soft Sight --- Investigate
            else if (m_angleToPlayer <= m_eelAngle + m_softSightAngle && m_angleToPlayer >= m_eelAngle - m_softSightAngle)
            {
                m_lastPlayerAngle = m_angleToPlayer;
                m_currentEelState = EelState.Investigate;
            }
        }
    }

    void LookForPlayer()
    {
        m_currentRotationTime += Time.deltaTime / m_rotationTime;
        if (m_currentRotationTime < 1)
        {
            m_eelAngle = Mathf.LerpAngle(m_eelAngle, m_lastPlayerAngle, m_currentRotationTime);
            Vector3 newDirection = new Vector3(Mathf.Cos(m_eelAngle), Mathf.Sin(m_eelAngle), 0);
            m_directionManager.UpdateSpriteDirection(newDirection);
        }
        else if (m_currentRotationTime > 1.5f)
        {
            m_currentEelState = EelState.Patrol;
        }
    }

    void ChasePlayer()
    {
        m_aiPath.destination = new Vector3(m_player.transform.position.x, m_player.transform.position.y, 0);
        m_eelAngle = Mathf.Atan2(m_player.transform.position.y - m_eel.transform.position.y,
                                 m_player.transform.position.x - m_eel.transform.position.x);
        m_aiPath.maxSpeed = m_speed * m_chaseSpeedModifier;
        m_directionManager.UpdateSpriteDirection(m_aiPath.velocity.normalized);
    }

    void Stunned()
    { 
        m_currentStunTime += Time.deltaTime;
        if (m_currentStunTime > m_stunTime)
        {
            m_currentStunTime = 0;
            m_currentEelState = EelState.Patrol;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (m_angleToPlayer <= m_eelAngle + m_hardSightAngle && m_angleToPlayer >= m_eelAngle - m_hardSightAngle)
        {
            Gizmos.color = Color.red;
        }
        else if (m_angleToPlayer <= m_eelAngle + m_softSightAngle && m_angleToPlayer >= m_eelAngle - m_softSightAngle)
        {
            Gizmos.color = Color.yellow;
        }
        Vector3 gizmoLine = new Vector3(Mathf.Cos(m_angleToPlayer) * m_sightRadius, Mathf.Sin(m_angleToPlayer) * m_sightRadius, 0);
        gizmoLine += m_eel.transform.position;

        Gizmos.DrawLine(m_eel.transform.position, gizmoLine);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_killCirclePosition, m_killDistance);
        gizmoLine = new Vector3(Mathf.Cos(m_eelAngle + m_hardSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle + m_hardSightAngle) * m_sightRadius, 0);
        gizmoLine += m_eel.transform.position;

        Gizmos.DrawLine(m_eel.transform.position, gizmoLine);
        gizmoLine = new Vector3(Mathf.Cos(m_eelAngle - m_hardSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle - m_hardSightAngle) * m_sightRadius, 0);
        gizmoLine += m_eel.transform.position;
        Gizmos.DrawLine(m_eel.transform.position, gizmoLine);

        Gizmos.color = Color.yellow;
        gizmoLine = new Vector3(Mathf.Cos(m_eelAngle + m_softSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle + m_softSightAngle) * m_sightRadius, 0);
        gizmoLine += m_eel.transform.position;
        Gizmos.DrawLine(m_eel.transform.position, gizmoLine);
        gizmoLine = new Vector3(Mathf.Cos(m_eelAngle - m_softSightAngle) * m_sightRadius, Mathf.Sin(m_eelAngle - m_softSightAngle) * m_sightRadius, 0);
        gizmoLine += m_eel.transform.position;
        Gizmos.DrawLine(m_eel.transform.position, gizmoLine);
    }
}
