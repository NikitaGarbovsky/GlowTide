using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteractiveObject : InteractiveObject
{
    [SerializeField] const int m_iObjectConditionAmount = 0;

    private void Start()
    {
        m_iCondition = m_iObjectConditionAmount;
    }

    protected override void ExecuteObjectAction()
    {
        if (GetSeaSlugListCount() >= m_iObjectConditionAmount)
        {
            // Execute door specific code (remove it                                                                       
        }
    }
}
