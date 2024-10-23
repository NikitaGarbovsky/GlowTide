/***********************************************************************
    Bachelor of Software Engineering
    Media Design School
    Auckland
    New Zealand

    (c) 2024 Media Design School

    File Name   :   SlugThrowing.cs
    Description :   Allows the Player to Throw and Pickup slugs
    Author      :   Connor Maguigan
    Mail        :   connor.maguigan@mds.ac.nz
**************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugThrowing : MonoBehaviour
{
    public int m_slugCount;
    [SerializeField]
    int m_pickupRadius;
    [SerializeField]
    GameObject m_slugObject;

    public LayerMask m_slugMask;
    public bool m_pickup;
    Vector3 mousePos;
    public List<GameObject> m_slugs;
    [SerializeField]
    List<GameObject> m_throwableSlugs;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Calculate Mouse angle from Player
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseAngle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        // Handle slug throwing
        if (Input.GetMouseButtonDown(0) && m_throwableSlugs.Count > 0)
        {
            m_slugs.Remove(m_throwableSlugs[0]);
            Destroy(m_throwableSlugs[0]);
            m_throwableSlugs.RemoveAt(0);
            GameObject newSlug = Instantiate(m_slugObject, transform.position, transform.rotation);
            SlugProjectile slugController = newSlug.GetComponent<SlugProjectile>();
            if (slugController != null)
            {
                // Set Slug Velocity
                slugController.SetVelocity(Mathf.Cos(mouseAngle), Mathf.Sin(mouseAngle));

            }
            /*// Get the mouse position in world coordinates
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Perform a raycast to check if the mouse is over an interactive object
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);
            if (hit.collider != null)
            {
                InteractiveObject interactiveObject = hit.collider.GetComponent<InteractiveObject>();
                if (interactiveObject != null)
                {
                    // Mouse is over an interactive object; proceed to assign slug
                    AssignSlugToInteractiveObject(interactiveObject);
                }
            }*/
        }

        // Call slugs on mousedown e
        if (Input.GetKey(KeyCode.E))
        {
            foreach (GameObject slug in m_slugs)
            {
                SeaSlugBroFollower slugController = slug.GetComponent<SeaSlugBroFollower>();
                if (slugController != null)
                {
                    slugController.StartFollowingPlayer();
                }
            }
        }

        // Picking up Slugs
        if (m_pickup)
        {
            Collider2D[] slugColliders = Physics2D.OverlapCircleAll(transform.position, m_pickupRadius, m_slugMask);
            foreach (Collider2D slug in slugColliders)
            {
                Debug.Log(slug.gameObject.name);
                if (m_throwableSlugs.Contains(slug.gameObject) == false)
                {
                    m_throwableSlugs.Add(slug.gameObject);
                }
                
                if (m_slugs.Contains(slug.gameObject) == false)
                {
                    m_slugs.Add(slug.gameObject);
                }

            }
        }
    }
    private void AssignSlugToInteractiveObject(InteractiveObject interactiveObject)
    {
        // Remove the slug from your lists
        m_slugs.Remove(m_throwableSlugs[0]);
        Destroy(m_throwableSlugs[0]);
        m_throwableSlugs.RemoveAt(0);

        // Instantiate a new slug and assign it to the interactive object
        GameObject newSlug = Instantiate(m_slugObject, transform.position, transform.rotation);
        interactiveObject.AddSlugToSlugList(newSlug);
    }
}
