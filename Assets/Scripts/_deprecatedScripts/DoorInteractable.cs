using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractable : SlugInteractableObject
{
    protected override void CheckCondition()
    {
        if (m_slugList.Count >= m_RequiredSlugs)
        {
            Destroy(gameObject);
        }
    }
}
