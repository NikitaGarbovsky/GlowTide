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
    [SerializeField]
    int m_slugCount;
    [SerializeField]
    int m_pickupRadius;
    [SerializeField]
    GameObject m_slugObject;

    public LayerMask m_slugMask;
    public bool m_pickup;
    Vector3 mousePos;
    CircleCollider2D playerCollider;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate Mouse angle from Player
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseAngle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        // Create Slugs
        if (Input.GetMouseButtonDown(0) && m_slugCount > 0)
        {
            m_slugCount--;
            GameObject newSlug = Instantiate(m_slugObject, transform.position, transform.rotation);
            SlugProjectile slugController = newSlug.GetComponent<SlugProjectile>();
            if (slugController != null)
            {
                // Set Slug Velocity
                slugController.SetVelocity(Mathf.Cos(mouseAngle), Mathf.Sin(mouseAngle));
            }
        }

        // Picking up Slugs [NOT FINISHED]
        if (m_pickup)
        {
            Collider2D[] slugColliders = Physics2D.OverlapCircleAll(transform.position, m_pickupRadius, m_slugMask);
            foreach (Collider2D slug in slugColliders)
            {
                Destroy(slug.gameObject);
                m_slugCount++;
            }
        }
    }
}
