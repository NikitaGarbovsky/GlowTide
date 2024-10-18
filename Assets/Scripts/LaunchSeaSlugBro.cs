using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchSeaSlugBro : MonoBehaviour
{
    [SerializeField]
    List<GameObject> seaSlugBros; // List of sea slugs currently following the player

    Vector3 mousePos;

    // Update is called once per frame
    void Update()
    {
        // Calculate Mouse angle from Player
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = (mousePos - transform.position).normalized; // Get the direction towards the mouse

        // Launch the first available slug when pressing 'E'
        if (Input.GetKeyDown(KeyCode.E) && seaSlugBros.Count > 0)
        {
            GameObject slugToLaunch = seaSlugBros[0]; // Get the first sea slug from the list
            seaSlugBros.RemoveAt(0); // Remove it from the list, so it's no longer following

            SeaSlugBroFollower slugController = slugToLaunch.GetComponent<SeaSlugBroFollower>();
            if (slugController != null)
            {
                // Launch the slug in the direction of the mouse
                //slugController.Launch(direction); // ========================
            }
        }
    }
    
    // Adds the slug to the seaSlugBros list 
    public void AddSlug(GameObject slug)
    {
        seaSlugBros.Add(slug);
    }
}