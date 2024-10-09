using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_UpKeyCode = KeyCode.W;

    public float m_MoveSpeed = 10.0f;

    public float m_Friction = 10.0f;
    public float m_Acceleration = 10.0f;

    [SerializeField]
    Vector3 m_Velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float x = 0.0f;
        float y = 0.0f;
        x -= Input.GetKey(m_LeftKeyCode) ? 1.0f : 0.0f;
        x += Input.GetKey(m_RightKeyCode) ? 1.0f : 0.0f;
        y -= Input.GetKey(m_DownKeyCode) ? 1.0f : 0.0f;
        y += Input.GetKey(m_UpKeyCode) ? 1.0f : 0.0f;

        Vector3 move = new Vector3(x, y, 0);
        if (move.magnitude > 1.0f)
        {
            move.Normalize();
        }

        m_Velocity -= m_Velocity * m_Friction * Time.deltaTime;
        m_Velocity += move * m_Acceleration * Time.deltaTime;

        Vector2 fullMove = m_Velocity * m_MoveSpeed * Time.deltaTime;

        float moveX = Mathf.Sign(fullMove.x) * Mathf.Max(Mathf.Abs(fullMove.x), 0.01f);
        RaycastHit2D hitX = Physics2D.BoxCast(transform.position, Vector2.one, 0.0f, Vector2.right, moveX);
        if (hitX)
        {
            fullMove.x = 0.0f;
        }

        float moveY = Mathf.Sign(fullMove.y) * Mathf.Max(Mathf.Abs(fullMove.y), 0.01f);
        RaycastHit2D hitY = Physics2D.BoxCast(transform.position, Vector2.one, 0.0f, Vector2.up, moveY);
        if (hitY)
        {
            fullMove.y = 0.0f;
        }

        transform.position += new Vector3(fullMove.x, fullMove.y, 0);
    }
}