using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SlugInteractableObject : MonoBehaviour
{
    public int m_RequiredSlugs;
    protected List<GameObject> m_slugList;
    protected List<GameObject> m_slugSpotList;

    public void AddToSlugList(GameObject _slug)
    {
        SeaSlugBroFollower slugFollower = _slug.GetComponent<SeaSlugBroFollower>();
        if (slugFollower != null)
        {
            if (m_slugList.Count < m_slugSpotList.Count)
            { 
                m_slugList.Add(_slug);
                slugFollower.MoveToAssignedObject(m_slugSpotList[m_slugList.Count - 1]);
                CheckCondition();
            }
        }
    }

    protected virtual void CheckCondition()
    {

    }
}
