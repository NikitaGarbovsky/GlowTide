using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelEnemyCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Geyser"))
        {
            Debug.Log("EelEnemy Stunned on Geyser Triggered!!!!");
            gameObject.GetComponentInParent<EelEnemyManager>().m_currentEelState = EelEnemyManager.EelState.Stunned;
            // Turns off the Geyser
            other.gameObject.GetComponent<GeyserManager>().m_bActive = false;
        }
    }
}
