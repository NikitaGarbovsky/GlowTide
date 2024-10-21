using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class inherited from the LevelTriggers class, it executes functionality related to individual triggers
/// associated with the INTRO level.
/// </summary>
public class LevelIntroTriggers : LevelTriggers
{
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_CallingBros") // This is the name of the gameobject that is being triggered.
        {
            // This is where the "Calling bros" trigger occurs
            Debug.Log("Calling bros triggered");
        }
    }
}
