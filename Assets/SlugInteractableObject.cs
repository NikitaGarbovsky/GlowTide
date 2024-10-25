using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlugInteractableObject : MonoBehaviour
{
    public int m_RequiredSlugs;
    public List<GameObject> m_slugSpotList;
    public List<GameObject> m_slugList;

    public void AddToSlugList(GameObject _slug)
    {
        SeaSlugBroFollower slugFollower = _slug.GetComponent<SeaSlugBroFollower>();
        if (slugFollower != null)
        {
            if (m_slugList.Count < m_slugSpotList.Count && m_slugList.Contains(_slug) == false)
            { 
                m_slugList.Add(_slug);
                slugFollower.MoveToAssignedObject(m_slugSpotList[m_slugList.Count - 1]);
                slugFollower.SetStuckToPoint(true);
                CheckCondition();
            }
        }
    }

    protected virtual void CheckCondition()
    {

    }
}
