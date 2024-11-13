using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class GeyserManager : MonoBehaviour
{
    [SerializeField] private Transform positionToMoveTo;
    [SerializeField] private float moveSpeed = 5f; // Speed of movement towards the target

    public bool m_bActive = true;

    [SerializeField] private int directionToAnimateThrow = 0;
    AudioSource m_audioSource;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    // 1. Object Collides with Geyser
    private void OnTriggerEnter2D(Collider2D other)
    {
        m_audioSource.Play();
        other.gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "2Up"; 
        other.gameObject.GetComponent<Animator>().SetInteger("Direction",directionToAnimateThrow);
        if (other.gameObject.CompareTag("Player") && m_bActive)
        {
            // 2. Disable pathfinding and collisions
            AIPath aiPath = other.gameObject.GetComponent<AIPath>();
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            other.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;

            // Changes all the assigned slugs to idle when the player uses the Geyser.
            foreach (GameObject slug in other.gameObject.GetComponent<PlayerSlugManager>().m_lAssignedSlugs)
            {
                slug.gameObject.GetComponent<SeaSlugBroFollower>().m_eCurrentState = SeaSlugBroFollower.ESlugState.Idle;
            }
            // 3. Start moving the player directly towards the target position
            StartCoroutine(MoveToPosition(other.gameObject, positionToMoveTo.position));
        }

        if (other.gameObject.CompareTag("Slug") && m_bActive)
        {
            other.gameObject.GetComponent<SeaSlugBroFollower>().m_eCurrentState = SeaSlugBroFollower.ESlugState.Thrown;
            other.gameObject.GetComponent<Animator>().runtimeAnimatorController =
                other.gameObject.GetComponent<SeaSlugBroFollower>().throwAnimatorController;
            // 2. Disable pathfinding and collisions
            AIPath aiPath = other.gameObject.GetComponent<AIPath>();
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            other.gameObject.GetComponent<SeaSlugBroFollower>().StopFollowingPlayer();
            other.gameObject.GetComponent<CircleCollider2D>().enabled = false;

            // 3. Start moving the player directly towards the target position
            StartCoroutine(MoveToPosition(other.gameObject, positionToMoveTo.position));
        }
        
    }

    // Coroutine to move the object towards the target position
    private IEnumerator MoveToPosition(GameObject _obToMove, Vector3 targetPosition)
    {
        // While the object is not yet at the target position
        while (Vector3.Distance(_obToMove.transform.position, targetPosition) > 0.1f)
        {
            // Move the object towards the target position
            _obToMove.transform.position = Vector3.MoveTowards(_obToMove.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Wait for the next frame before continuing
            yield return null;
        }

        // 4. When object reaches the position:
        // 4.1. Restore the pathfinding system and collisions
        AIPath aiPath = _obToMove.GetComponent<AIPath>();
        
        if (_obToMove.gameObject.CompareTag("Slug"))
        {
            _obToMove.gameObject.GetComponent<SeaSlugBroFollower>().m_eCurrentState = SeaSlugBroFollower.ESlugState.Idle;
            _obToMove.gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Ground"; 
            _obToMove.gameObject.GetComponent<CircleCollider2D>().enabled = true;
            _obToMove.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            aiPath.enabled = true;
            if (aiPath != null)
            {
                aiPath.canMove = true;
                aiPath.destination = positionToMoveTo.position;
            }
            _obToMove.gameObject.layer = LayerMask.NameToLayer("Slug");
        }
        else
        {
            _obToMove.gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "Ground"; 
            _obToMove.GetComponent<CapsuleCollider2D>().enabled = true;
            if (aiPath != null)
            {
                aiPath.canMove = true;
                aiPath.destination = positionToMoveTo.position;
            }
        }
        
        
    }
}