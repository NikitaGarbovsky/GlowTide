using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class DoorInteractiveObject : InteractiveObject
{
    [SerializeField] int m_iObjectConditionAmount = 0;
    [SerializeField] GameObject SlugSpot1;
    [SerializeField] GameObject SlugSpot2;
    [SerializeField] GameObject SlugSpot3;
    [SerializeField] GameObject SlugSpot4;
    [SerializeField] GameObject SlugSpot5;

    [SerializeField] GameObject Grid;
    private void Start()
    {
        m_iCondition = m_iObjectConditionAmount;
    }

    protected override void ExecuteObjectAction()
    {
        if (GetSeaSlugListCount() >= m_iObjectConditionAmount)
        {
            // Grabs the bounds of the door,
            Bounds doorBounds = GetComponent<PolygonCollider2D>().bounds;
            // Destroys it,
            Destroy(gameObject);  
            // Then updates the pathfinding graph (removes the collision) 
            Grid.GetComponent<AstarPath>().UpdateGraphs(doorBounds);
        }
    }

    private void Update()
    {
        CheckIfActionConditionMet();
    }
}
