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
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseAngle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x);
        Debug.Log(mouseAngle);
        //Vector3 pos = new Vector3(Mathf.Cos(mouseAngle) * 3, Mathf.Sin(mouseAngle) * 3, 0);
        if (Input.GetMouseButtonDown(0) && m_slugCount > 0)
        {
            m_slugCount--;
            GameObject newSlug = Instantiate(m_slugObject, transform.position, transform.rotation);
            TEST_SlugController slugController = newSlug.GetComponent<TEST_SlugController>();
            if (slugController != null)
            {
                slugController.SetVelocity(Mathf.Cos(mouseAngle), Mathf.Sin(mouseAngle));
            }
        }

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
