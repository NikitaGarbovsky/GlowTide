using UnityEngine;

public class SlugProjectile : MonoBehaviour
{
    private Vector2 targetPosition;
    private float throwDistance;
    private float speed;

    public void Initialize(Vector2 mousePosition, float distance, float throwSpeed)
    {
        // Calculate direction and final target position
        Vector2 playerPosition = transform.position;
        Vector2 direction = (mousePosition - playerPosition).normalized;
        targetPosition = playerPosition + direction * distance;

        // Set speed
        speed = throwSpeed;
    }

    void Update()
    {
        // Move the slug towards the target position
        if ((Vector2)transform.position != targetPosition)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            // Reached the target position
            OnReachTarget();
        }
    }

    void OnReachTarget()
    {
        // TODO: Implement what happens when the slug reaches the target
        // For now, we'll destroy the slug
        Destroy(gameObject);
    }
}



/*

***********************************************************************
    Bachelor of Software Engineering
    Media Design School
    Auckland
    New Zealand

    (c) 2024 Media Design School

    File Name   :   TEST_SlugController.cs
    Description :   Slug Controller - Allows the slugs to move with Velocity
    Author      :   Connor Maguigan
    Mail        :   connor.maguigan@mds.ac.nz
**************************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlugProjectile : MonoBehaviour
{
    public float m_MoveSpeed;
    public float m_Friction;
    public Vector3 m_Velocity;

    [SerializeField]
    GameObject m_slug;
    [SerializeField]
    GameObject m_player;
    private void Awake()
    {
        m_player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Move Slugs
        m_Velocity -= m_Velocity * m_Friction * Time.deltaTime;
        Vector2 move = m_Velocity * m_MoveSpeed * Time.deltaTime;
        transform.position += new Vector3(move.x, move.y, 0);

        if (m_Velocity.magnitude < 0.01)
        {
            GameObject slug = Instantiate(m_slug, transform.position, transform.rotation);
            SlugThrowing slugThrowing = m_player.GetComponent<SlugThrowing>();
            if (slugThrowing != null)
            {
                slugThrowing.m_slugs.Add(slug);
            }
            Destroy(gameObject);
        }

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.zero);
        if (hit)
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                m_Velocity *= -1;
            }

            if (hit.collider.gameObject.tag == "Door")
            {
                GameObject slug = Instantiate(m_slug, transform.position, transform.rotation);
                SlugThrowing slugThrowing = m_player.GetComponent<SlugThrowing>();
                if (slugThrowing != null)
                {
                    slugThrowing.m_slugs.Add(slug);
                }
                DoorInteractable doorscript = hit.collider.gameObject.GetComponent<DoorInteractable>();
                if (doorscript != null)
                {
                    doorscript.AddToSlugList(slug);
                    Debug.Log("kfgjldskfsgjhlsdkfjgl");
                    Destroy(gameObject);
                }
            }

            if (hit.collider.gameObject.tag == "Door")
            {
            }
        }
    }

    public void SetVelocity(float x, float y)
    {
        m_Velocity = new Vector3(x, y, 0);
    }

    
}
*/