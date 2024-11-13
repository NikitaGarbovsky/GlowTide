using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigEelController : MonoBehaviour
{
    [SerializeField] bool m_canMove; // If the Eel can Move
    [SerializeField] Vector2 m_point; // Point to Move to
    [SerializeField] public float m_speed; // Move Speed
    private IsoSpriteDirectionManager isoSpriteDirectionManager;
    private Animator animator;
    private AudioSource audioSource;
    private void Update()
    {
        if (m_canMove)
        {
            // Moves the Eel to m_point
            transform.position = Vector2.MoveTowards(transform.position, m_point, m_speed * Time.deltaTime);
            if (transform.position.x == m_point.x
            &&  transform.position.y == m_point.y) 
            { 
                m_canMove = false; 
            }
        }
    }
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        animator = GetComponent<Animator>();
    }

    // Sets the Move to Point
    public void MovetoPoint(Vector2 _point)
    {
        m_canMove = true;
        m_point = _point;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Eel Boss Collided with Slug");
        if (other.CompareTag("Slug"))
        {
            Destroy(other.gameObject);
        }
    }

    public void SetDirectionForMovement(int _iDirection, string _DirectionName)
    {
        string animationName = _iDirection + "_Eel_" + _DirectionName;
        animator.SetInteger("Direction", _iDirection);
        animator.Play(animationName, 0, 0);
    }
}
