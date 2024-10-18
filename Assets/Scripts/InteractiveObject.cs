using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script should be an abstract class that allows the gameObject this is attached too to be interacted with
// in a variety of ways. 
// The way it does this should be the following:
// 1. Player mouses over this gameobject, it high-lights to reflect this, TODO
// 2. The player presses the launch sea slug key, 
// 3. The seaslug is launched towards this game object, that seaslug is then "assigned" to this object on mouse click
// 4. This gameObject holds a list of all seaslugs that are assigned to it
// 5. Sea slugs that are assigned to this gameobject persist near it, not following the player, they are in a
// "Assigned" State, and they hold a reference to the object they are assigned too
// 6. This object has a certain required amount of seaslugs that need to be assigned to it to "Meet" its requirements
// 7. Once the certain amount of slugs assignment condition is met, the object executes its condition. 

public abstract class InteractiveObject : MonoBehaviour
{
    ~InteractiveObject() // Destructor
    {
        // When the object is destroyed we want to clear the list and reassign all seaslugs to sit idle. 
    }
    // The condition (amount of slugs in the slug list) that is assigned from the child class then 
    protected int m_iCondition;
    // Holds all a reference to all assigned SeaSlugs to this object
    private List<GameObject> m_lstAssignedSeaSlugs = new List<GameObject>();

    private bool m_bAssignable;
    
    // Depending on what the object is, when the conditions are met we want to execute its action,
    // For example it may fade away to reveal a path, or remove itself as it is a door, or die because it is an enemy.
    protected abstract void ExecuteObjectAction();
    // When the player mouses over the target(this gameobject), 
    private void OnMouseEnter()
    {
        // that's when we want to allow assigning of seaslugs;
        m_bAssignable = true;
        Debug.Log("Interactive Object Mouse Over");
    }
    // If the player is no longer mousing over, 
    private void OnMouseExit()
    {
        // Dont allow slugs to be assignable.
        m_bAssignable = false;
        Debug.Log("Interactive Object MOUSE EXIT");
    }
    protected void CheckIfActionConditionMet()
    {
        if (m_iCondition >= m_lstAssignedSeaSlugs.Count)
        {
            ExecuteObjectAction();
        }
    }
    public virtual  void AddSlugToSlugList(GameObject _seaslug)
    {
        // Add passed in slug to slug list.
        m_lstAssignedSeaSlugs.Add(_seaslug);
        // Check if the amount of slugs matches the condition, If so, execute action
        CheckIfActionConditionMet(); 
    }

    protected int GetSeaSlugListCount()
    {
        return m_lstAssignedSeaSlugs.Count;
    }
    
    
}
