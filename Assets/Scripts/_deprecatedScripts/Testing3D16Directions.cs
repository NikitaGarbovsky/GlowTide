using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Testing3D16Directions : MonoBehaviour
{
    public AIPath aiPath; // Reference to A* pathfinding component
    public Animator animator; // Reference to Animator for handling animations

    public Transform spriteTransform; // Reference to the child object holding the SpriteRenderer

    private Vector3 targetPosition;
    private Vector3 spriteInitialLocalPosition;

    public float bobbingAmplitude = 0.5f; // Height of the bobbing motion
    public float bobbingSpeed = 2f; // Speed of the bobbing motion

    private int currentDirection; // Track the current direction index (0-15)

    // Start is called before the first frame update
    private void Start()
    {
        aiPath = GetComponent<AIPath>(); // Get the AIPath component
        animator = GetComponent<Animator>();
        spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
        spriteInitialLocalPosition = spriteTransform.localPosition; // Store initial local position of the sprite
        targetPosition = transform.position;  // Initialize the target position
        
        currentDirection = -1; // Initialize direction as invalid (-1 means no valid direction yet)
    }

    // Update is called once per frame
    void Update()
    {
        // Right-click to set destination
        if (Input.GetMouseButtonDown(1) && ManageGameplay.Instance.PlayerCanIssueMoveCommands)
        {
            // Get the mouse position in world space
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure z is 0 for 2D space

            // Set the target position for the A* pathfinding system
            targetPosition = mousePosition;
            aiPath.destination = targetPosition; // Tell A* where to move
        }

        // Change animation based on movement direction
        Vector3 direction = (aiPath.steeringTarget - transform.position).normalized;

        // Determine the angle of movement
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Set animation based on angle
        SetAnimationBasedOnAngle(angle);

        // Bobbing effect on the Y-axis for the sprite child object
        float newLocalY = spriteInitialLocalPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmplitude;
        spriteTransform.localPosition = new Vector3(spriteTransform.localPosition.x, newLocalY, spriteTransform.localPosition.z);
    }

    private void SetAnimationBasedOnAngle(float angle)
{
    // Normalize angle to be between 0 and 360 degrees
    if (angle < 0) angle += 360f;

    int newDirection = GetDirectionIndexForAngle(angle);

    // Only update the animator if the direction has changed
    if (newDirection != currentDirection)
    {
        //Debug.Log(newDirection);
        animator.SetInteger("Direction", newDirection);
        // Get the current animation state and its normalized time
        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        float currentNormalizedTime = currentState.normalizedTime; // This keeps the animation's progress
        //Debug.Log("Normalized Time: " + currentNormalizedTime);
        
        //Debug.Log("Direction animation change: " + GetDirectionNumber(newDirection) + "_JELLYFISH_" + GetDirectionName(newDirection));
        // Update the Animator parameter with normalized time to continue from the same frame
        string animationName = GetDirectionNumber(newDirection) + "_JELLYFISH_" + GetDirectionName(newDirection);
        animator.Play(animationName, 0, currentNormalizedTime);
        // Update the current direction tracker
        currentDirection = newDirection;
    }
}

private string GetDirectionName(int directionIndex)
{
    switch (directionIndex)
    {
        case 0: return "E"; // East
        case 1: return "NEE"; // North-East East
        case 2: return "NE"; // North-East
        case 3: return "NNE"; // North North-East
        case 4: return "N"; // North
        case 5: return "NNW"; // North North-West
        case 6: return "NW"; // North-West
        case 7: return "NWW"; // West North-West
        case 8: return "W"; // West
        case 9: return "SWW"; // South-West West
        case 10: return "SW"; // South-West
        case 11: return "SSW"; // South South-West
        case 12: return "S"; // South
        case 13: return "SSE"; // South South-East
        case 14: return "SE"; // South-East
        case 15: return "SEE"; // South-East East
        default: return "E"; // Default to East
    }
}

private string GetDirectionNumber(int directionIndex)
{
    // Return the corresponding number for the direction based on the index
    switch (directionIndex)
    {
        case 0: return "0";  // East
        case 1: return "1";  // North-East East
        case 2: return "2";  // North-East
        case 3: return "3";  // North North-East
        case 4: return "4";  // North
        case 5: return "5";  // North North-West
        case 6: return "6";  // North-West
        case 7: return "7";  // West North-West
        case 8: return "8";  // West
        case 9: return "9";  // South-West West
        case 10: return "10"; // South-West
        case 11: return "11"; // South South-West
        case 12: return "12"; // South
        case 13: return "13"; // South South-East
        case 14: return "14"; // South-East
        case 15: return "15"; // South-East East
        default: return "0"; // Default to East
    }
}


    private int GetDirectionIndexForAngle(float _angle)
    {
        //Debug.Log("Angle: " + _angle);
        // 16 direction handling - 22.5 degree increments
        if (_angle >= 348.75f || _angle < 11.25f)
        {
            return 0; // East
        }
        else if (_angle >= 11.25f && _angle < 33.75f)
        {
            return 1; // North-East East
        }
        else if (_angle >= 33.75f && _angle < 56.25f)
        {
            return 2; // North-East
        }
        else if (_angle >= 56.25f && _angle < 78.75f)
        {
            return 3; // North North-East
        }
        else if (_angle >= 78.75f && _angle < 101.25f)
        {
            return 4; // North
        }
        else if (_angle >= 101.25f && _angle < 123.75f)
        {
            return 5; // North North-West
        }
        else if (_angle >= 123.75f && _angle < 146.25f)
        {
            return 6; // North-West
        }
        else if (_angle >= 146.25f && _angle < 168.75f)
        {
            return 7; // West North-West
        }
        else if (_angle >= 168.75f && _angle < 191.25f)
        {
            return 8; // West
        }
        else if (_angle >= 191.25f && _angle < 213.75f)
        {
            return 9; // South-West West
        }
        else if (_angle >= 213.75f && _angle < 236.25f)
        {
            return 10; // South-West
        }
        else if (_angle >= 236.25f && _angle < 258.75f)
        {
            return 11; // South South-West
        }
        else if (_angle >= 258.75f && _angle < 281.25f)
        {
            return 12; // South
        }
        else if (_angle >= 281.25f && _angle < 303.75f)
        {
            return 13; // South South-East
        }
        else if (_angle >= 303.75f && _angle < 326.25f)
        {
            return 14; // South-East
        }
        else if (_angle >= 326.25f && _angle < 348.75f)
        {
            return 15; // South-East East
        }
        
        Debug.Log("FAILED TO RETURN ANGLE");
        return 0; // Default to East if something goes wrong
    }
}
