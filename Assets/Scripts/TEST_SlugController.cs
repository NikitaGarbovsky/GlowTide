using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_SlugController : MonoBehaviour
{
    public float m_MoveSpeed;
    public float m_Friction;
    public Vector3 m_Velocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_Velocity -= m_Velocity * m_Friction * Time.deltaTime;
        Vector2 move = m_Velocity * m_MoveSpeed * Time.deltaTime;
        transform.position += new Vector3(move.x, move.y, 0);
    }

    public void SetVelocity(float x, float y)
    {
        m_Velocity = new Vector3(x, y, 0);
    }
}
