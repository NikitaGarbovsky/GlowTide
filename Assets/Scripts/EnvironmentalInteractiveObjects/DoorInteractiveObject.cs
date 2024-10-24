using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class DoorInteractiveObject : InteractiveObject
{
    [SerializeField] int m_iObjectConditionAmount = 0;
    //[SerializeField] Transform SlugSpot1;
    //[SerializeField] Transform SlugSpot2;
    //[SerializeField] Transform SlugSpot3;
    //[SerializeField] Transform SlugSpot4;
    //[SerializeField] Transform SlugSpot5;

    [SerializeField] GameObject Grid;

    [SerializeField] List<Transform> slugSpots = new List<Transform>();
    private int slugSpotIndex = 0; // Tracks the next available slug spot

    private int slugsReachedTarget = 0; // Count of slugs that have reached their spots
    
    private void Start()
    {
        m_iCondition = m_iObjectConditionAmount;
        //// Initialize the slug spots list
        //if (SlugSpot1 != null) slugSpots.Add(SlugSpot1);
        //if (SlugSpot2 != null) slugSpots.Add(SlugSpot2);
        //if (SlugSpot3 != null) slugSpots.Add(SlugSpot3);
        //if (SlugSpot4 != null) slugSpots.Add(SlugSpot4);
        //if (SlugSpot5 != null) slugSpots.Add(SlugSpot5);
    }

    private void Update()
    {
    }

    protected override void ExecuteObjectAction()
    {
        // Door disappears
        // Grabs the bounds of the door,
        Bounds doorBounds = GetComponent<PolygonCollider2D>().bounds;
        // Destroys it,
        Debug.Log(slugSpotIndex);
        Debug.Log(slugSpots.Count);
        Debug.Log(slugsReachedTarget);
        Debug.Log(m_iObjectConditionAmount);
        Destroy(gameObject);  
        // Then updates the pathfinding graph (removes the collision) 
        Grid.GetComponent<AstarPath>().UpdateGraphs(doorBounds);
    }

    public override void AddSlugToSlugList(GameObject seaSlug)
    {
        base.AddSlugToSlugList(seaSlug);

        // Assign the slug to a spot
        if (slugSpotIndex < slugSpots.Count)
        {
            Transform targetSpot = slugSpots[slugSpotIndex];
            slugSpotIndex++;

            // Command the slug to move to the target spot
            SeaSlugBroFollower slugFollower = seaSlug.GetComponent<SeaSlugBroFollower>();
            if (slugFollower != null)
            {
                //slugFollower.MoveToAssignedObject(targetSpot);

                // Subscribe to the slug's OnReachedTarget event
                slugFollower.OnReachedTarget += SlugReachedTarget;
            }
        }
        else
        {
            Debug.LogWarning("No more slug spots available for this object.");
        }
    }

    private void SlugReachedTarget(SeaSlugBroFollower slug)
    {
        slugsReachedTarget++;

        // Unsubscribe from the event to avoid memory leaks
        slug.OnReachedTarget -= SlugReachedTarget;

        // Check if all assigned slugs have reached their spots
        if (slugsReachedTarget >= m_iObjectConditionAmount)
        {
            ExecuteObjectAction();
        }
    }
}
