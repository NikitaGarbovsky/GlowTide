using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SeaSlugBroFollower : MonoBehaviour
{
    public Transform player;  // Reference to the player transform (seahorse)
    private AIPath aiPath;    // Reference to A* Pathfinding component on this gameobject

    private void Start()
    {
        // Finds the player gameobject and applies its transform to the variable
        player = GameObject.FindWithTag("Player").transform; 
        aiPath = GetComponent<AIPath>(); // Gets the A* pathfinding component from THIS gameobject
    }

    private void Update()
    {
        // Continuously set the destination of the sea slug bro to the player's position
        if (player != null)
        {
            aiPath.destination = player.position;
        }
    }
}
