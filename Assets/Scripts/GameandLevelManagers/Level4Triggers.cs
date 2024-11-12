using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4Triggers : LevelTriggers
{
    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "1_LevelTransition") 
        {
            // This is where the level transition trigger occurs
            Debug.Log("Level of level triggered");

            ManageGameplay.Instance.LoadSceneWithFade("5_Level5");
        }
    }
}
