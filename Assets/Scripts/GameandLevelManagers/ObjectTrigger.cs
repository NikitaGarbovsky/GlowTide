using System;
using UnityEngine;

public class ObjectTrigger : MonoBehaviour
{
    public bool m_bOnlyTriggerableOnce = true;
    private bool m_bHasBeenTriggered = false;
    public string m_sTriggerObjectPurpose;
    private PlayerSlugManager playerSlugManager;

    [SerializeField] private AudioSource audioSource;
    // This method will be called when another collider enters this collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EelBoss"))
        {
            foreach (GameObject slug in playerSlugManager.m_lAssignedSlugs)
            {
                if (slug.GetComponent<SeaSlugBroFollower>().m_bIsInVulnerableZone)
                {
                    Destroy(slug);
                    playerSlugManager.m_lAssignedSlugs.Remove(slug);
                    audioSource.Play();
                }
            }
        }
        
        if (m_bOnlyTriggerableOnce && m_bHasBeenTriggered)
        {
            return;
        }
        if (m_bOnlyTriggerableOnce && !m_bHasBeenTriggered || !m_bOnlyTriggerableOnce)
        {
            // Check if the collider belongs to the player by tag 
            if (other.CompareTag("Player"))
            {
                //Debug.Log("Player collided with the " + m_sTriggerObjectPurpose);
                // Executes the correct corresponding functionality for the object that was triggered within 
                // the associated scenes level manager.
                ManageGameplay.Instance.ExecuteLevelManagerTrigger(m_sTriggerObjectPurpose);
                m_bHasBeenTriggered = true;
            }

            
        }
    }

    private void Start()
    {
        m_sTriggerObjectPurpose = gameObject.name;
        playerSlugManager = ManageGameplay.Instance.playerCharacter.GetComponent<PlayerSlugManager>();
    }
}
