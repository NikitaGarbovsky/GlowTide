using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CharacterMovement : MonoBehaviour
{
    public AIPath aiPath; // Reference to A* pathfinding component
    [SerializeField] public Sprite N;
    [SerializeField] public Sprite NE;
    [SerializeField] public Sprite E;
    [SerializeField] public Sprite SE;
    [SerializeField] public Sprite S;
    [SerializeField] public Sprite SW;
    [SerializeField] public Sprite W;
    [SerializeField] public Sprite NW;

    public Transform spriteTransform; // Reference to the child object holding the SpriteRenderer

    private SpriteRenderer _spriteRenderer;
    private Vector3 targetPosition;
    private Vector3 spriteInitialLocalPosition;

    public float bobbingAmplitude = 0.5f; // Height of the bobbing motion
    public float bobbingSpeed = 2f; // Speed of the bobbing motion

    // Start is called before the first frame update
    private void Start()
    {
        aiPath = GetComponent<AIPath>(); // Grabs the AIPath component from this gameObject 
        _spriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
        spriteInitialLocalPosition = spriteTransform.localPosition; // Store initial local position of the sprite
        targetPosition = transform.position;  // Initialize the target position
    }

    // Update is called once per frame
    void Update()
    {
        // Right-click to set destination
        if (Input.GetMouseButtonDown(1))
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure z is 0 for 2D space

            // Set the target position for the A* pathfinding system
            targetPosition = mousePosition;
            aiPath.destination = targetPosition; // Tell A* where to move
        }

        // Change sprite based on movement direction
        Vector3 direction = (aiPath.steeringTarget - transform.position).normalized;

        // Determine the angle of movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set sprite based on angle
        SetSpriteBasedOnAngle(angle);

        // Bobbing effect on the Y-axis for the sprite child object
        float newLocalY = spriteInitialLocalPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        spriteTransform.localPosition = new Vector3(spriteTransform.localPosition.x, newLocalY, spriteTransform.localPosition.z);
    }

    private void SetSpriteBasedOnAngle(float angle)
    {
        // Normalize angle to be between 0 and 360 degrees
        if (angle < 0) angle += 360f;

        // Check the direction using angle thresholds
        if (angle >= 337.5f || angle < 22.5f)
        {
            _spriteRenderer.sprite = E; // East
        }
        else if (angle >= 22.5f && angle < 67.5f)
        {
            _spriteRenderer.sprite = NE; // North-East
        }
        else if (angle >= 67.5f && angle < 112.5f)
        {
            _spriteRenderer.sprite = N; // North
        }
        else if (angle >= 112.5f && angle < 157.5f)
        {
            _spriteRenderer.sprite = NW; // North-West
        }
        else if (angle >= 157.5f && angle < 202.5f)
        {
            _spriteRenderer.sprite = W; // West
        }
        else if (angle >= 202.5f && angle < 247.5f)
        {
            _spriteRenderer.sprite = SW; // South-West
        }
        else if (angle >= 247.5f && angle < 292.5f)
        {
            _spriteRenderer.sprite = S; // South
        }
        else if (angle >= 292.5f && angle < 337.5f)
        {
            _spriteRenderer.sprite = SE; // South-East
        }
    }
}
