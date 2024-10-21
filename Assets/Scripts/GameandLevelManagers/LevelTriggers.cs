using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is an abstract class that is inherited from by separate level trigger classes that have their own trigger
/// functionality.
/// </summary>
public abstract class LevelTriggers : MonoBehaviour
{
    public abstract void ExecuteLevelTrigger(string _sTriggerName);
}
