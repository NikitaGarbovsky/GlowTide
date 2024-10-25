using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : SlugInteractableObject
{
    protected override void CheckCondition()
    {
        if (m_slugList.Count >= m_RequiredSlugs)
        {
            foreach (GameObject slug in m_slugList)
            {
                SeaSlugBroFollower slugFollower = slug.GetComponent<SeaSlugBroFollower>();
                if (slugFollower != null)
                {
                    //slugFollower.SetStuckToPoint(false);
                }
            }
            Destroy(gameObject);
        }
    }
}
