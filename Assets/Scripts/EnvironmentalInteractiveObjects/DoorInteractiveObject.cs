using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DoorInteractiveObject : InteractiveObject  
{
    [SerializeField] int m_iObjectConditionAmount = 0;

    [SerializeField] GameObject Grid;

    [SerializeField] List<Transform> slugSpots = new List<Transform>();
    private int slugSpotIndex = 0; // Tracks the next available slug spot

    private int slugsReachedTarget = 0; // Count of slugs that have reached their spots
    
    private void Start()
    {
        m_iCondition = m_iObjectConditionAmount;
    }

    protected override void ExecuteObjectAction()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        // Before destroying the door, reset assigned SeaSlugs
        foreach (var seaSlug in m_lstAssignedSeaSlugs)
        {
            if (seaSlug != null)
            {
                SeaSlugBroFollower slugFollower = seaSlug.GetComponent<SeaSlugBroFollower>();
                if (slugFollower != null)
                {
                    // Reset the slug's state
                    slugFollower.m_eCurrentState = SeaSlugBroFollower.ESlugState.Idle;
                }
            }
        }
        
        // Get the sprite render for the door 
        SpriteRenderer[] renderers = GetComponents<SpriteRenderer>();
        float duration = 2f;
        float elapsedTime = 0f;

        // Fade out over time
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            foreach (var sr in renderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }
            }
            yield return null;
        }

        // Ensure alpha is set to 0
        foreach (var sr in renderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;
            }
        }

        // Grab the polygon collider of the door
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();

        // Update the pathfinding graph
        Bounds doorBounds = collider.bounds;
        Grid.GetComponent<AstarPath>().UpdateGraphs(doorBounds);

        // Destroy the door GameObject
        Destroy(gameObject);
    }


    public override void AddSlugToSlugList(GameObject seaSlug)
    {
        // Assign the slug to a spot
        if (slugSpotIndex < slugSpots.Count)
        {
            Transform targetSpot = slugSpots[slugSpotIndex];
            slugSpotIndex++;

            // Move the seaslug to the target spot immediately
            seaSlug.transform.position = targetSpot.position;

            // Change the slug's state to Assigned
            SeaSlugBroFollower slugFollower = seaSlug.GetComponent<SeaSlugBroFollower>();
            if (slugFollower != null)
            {
                slugFollower.m_eCurrentState = SeaSlugBroFollower.ESlugState.Assigned;
                // Disable AIPath and stop movement
                slugFollower.m_aiPath.enabled = false;
                slugFollower.m_rbSlug.velocity = Vector2.zero;
                slugFollower.m_rbSlug.isKinematic = true;
            }

            // Increase the count of slugs that have reached their spots
            slugsReachedTarget++;

            // Check if the condition is met
            if (slugsReachedTarget >= m_iObjectConditionAmount)
            {
                ExecuteObjectAction();
            }
        }
        else
        {
            Debug.LogWarning("No more slug spots available for this object.");
        }
    }

    public override void RemoveSlugFromSlugList(GameObject seaSlug)
    {
        base.RemoveSlugFromSlugList(seaSlug);

        // Decrease the count of slugs that have reached their spots
        slugsReachedTarget--;
        
        slugSpotIndex--;
    }
}
