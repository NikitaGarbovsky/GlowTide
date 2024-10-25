using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] GameObject m_eel;
    [SerializeField] List<Transform> m_waypoints;
    [SerializeField] float m_speed;

    private int m_waypointIndex;
    

    private void Update()
    {
        m_eel.transform.position = Vector2.MoveTowards(m_eel.transform.position, m_waypoints[m_waypointIndex].position, m_speed * Time.deltaTime);
        if (Vector2.Distance(m_eel.transform.position, m_waypoints[m_waypointIndex].position) <= 0)
        {
            m_waypointIndex++;
            if (m_waypointIndex > m_waypoints.Count - 1)
            {
                m_waypointIndex = 0;
            }
        }
    }
}
