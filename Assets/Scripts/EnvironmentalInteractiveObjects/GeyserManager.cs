using System.Collections;
using Pathfinding;
using UnityEngine;

public class GeyserManager : MonoBehaviour
{
    [SerializeField] private Transform positionToMoveTo;
    [SerializeField] private float moveSpeed = 5f; // Speed of movement towards the target

    // 1. Object Collides with Geyser
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // 2. Disable pathfinding and collisions
            AIPath aiPath = other.gameObject.GetComponent<AIPath>();
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            other.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

            // 3. Start moving the player directly towards the target position
            StartCoroutine(MoveToPosition(other.gameObject, positionToMoveTo.position));
        }
    }

    // Coroutine to move the object towards the target position
    private IEnumerator MoveToPosition(GameObject player, Vector3 targetPosition)
    {
        // While the object is not yet at the target position
        while (Vector3.Distance(player.transform.position, targetPosition) > 0.1f)
        {
            // Move the object towards the target position
            player.transform.position = Vector3.MoveTowards(player.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Wait for the next frame before continuing
            yield return null;
        }

        // 4. When object reaches the position:
        // 4.1. Restore the pathfinding system and collisions
        AIPath aiPath = player.GetComponent<AIPath>();
        if (aiPath != null)
        {
            aiPath.canMove = true;
            aiPath.destination = positionToMoveTo.position;
        }
        player.GetComponent<CapsuleCollider2D>().enabled = true;
        
    }
}