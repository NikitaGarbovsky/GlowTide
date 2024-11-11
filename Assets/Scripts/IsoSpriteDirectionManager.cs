using UnityEngine;

/// <summary>
/// This class manages the sprite directions & animations for objects that move around the scene and change directions.
/// </summary>
public class IsoSpriteDirectionManager : MonoBehaviour
{
    public Animator animator; // Reference to Animator for handling animations
    public Transform spriteTransform; // Reference to the child object holding the SpriteRenderer

    // TODO the bobbing effect is only really used by the jellyfish (player) so might
    // remove this in the future or change where it is
    public float bobbingAmplitude = 0.5f; // Height of the bobbing motion
    public float bobbingSpeed = 2f; // Speed of the bobbing motion

    private Vector3 spriteInitialLocalPosition;
    private int currentDirection = -1; // Track the current direction index (0-15)

    public void SetDirection(int _iDirection)
    {
        currentDirection = _iDirection;
        animator.SetInteger("Direction",_iDirection);
    }
    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (spriteTransform == null)
        {
            spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
        }
        spriteInitialLocalPosition = spriteTransform.localPosition; // Store initial local position of the sprite
    }

    public void UpdateSpriteDirection(Vector3 direction)
    {
        // Change animation based on movement direction
        if (direction.sqrMagnitude > 0.01f)
        {
            // Determine the angle of movement
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Set animation based on angle
            SetAnimationBasedOnAngle(angle);
        }

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
            animator.SetInteger("Direction", newDirection);

            // Get the current animation state and its normalized time
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            float currentNormalizedTime = currentState.normalizedTime % 1; // Keep it between 0 and 1

            // Update the Animator parameter with normalized time to continue from the same frame
            string animationName = GetDirectionNumber(newDirection) + "_JELLYFISH_" + GetDirectionName(newDirection);
            animator.Play(animationName, 0, currentNormalizedTime);

            // Update the current direction tracker
            currentDirection = newDirection;
        }
    }

    public int GetDirectionIndexForAngle(float angle)
    {
        // 16-direction handling - 22.5-degree increments
        if (angle >= 348.75f || angle < 11.25f)
            return 0;  // East
        else if (angle >= 11.25f && angle < 33.75f)
            return 1;  // North-East East
        else if (angle >= 33.75f && angle < 56.25f)
            return 2;  // North-East
        else if (angle >= 56.25f && angle < 78.75f)
            return 3;  // North North-East
        else if (angle >= 78.75f && angle < 101.25f)
            return 4;  // North
        else if (angle >= 101.25f && angle < 123.75f)
            return 5;  // North North-West
        else if (angle >= 123.75f && angle < 146.25f)
            return 6;  // North-West
        else if (angle >= 146.25f && angle < 168.75f)
            return 7;  // West North-West
        else if (angle >= 168.75f && angle < 191.25f)
            return 8;  // West
        else if (angle >= 191.25f && angle < 213.75f)
            return 9;  // South-West West
        else if (angle >= 213.75f && angle < 236.25f)
            return 10; // South-West
        else if (angle >= 236.25f && angle < 258.75f)
            return 11; // South South-West
        else if (angle >= 258.75f && angle < 281.25f)
            return 12; // South
        else if (angle >= 281.25f && angle < 303.75f)
            return 13; // South South-East
        else if (angle >= 303.75f && angle < 326.25f)
            return 14; // South-East
        else if (angle >= 326.25f && angle < 348.75f)
            return 15; // South-East East

        Debug.LogError("Failed to determine direction index for angle: " + angle);
        return 0; // Default to East if something goes wrong
    }

    public int UpdateAnimatorDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            // Determine the angle of the throw direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Normalize angle to be between 0 and 360 degrees
            if (angle < 0) angle += 360f;

            // Get the direction index based on the normalized angle
            int directionIndex = GetDirectionIndexForAngle(angle);

            // Update the Animator's Direction parameter
            gameObject.GetComponent<Animator>().SetInteger("Direction", directionIndex);

            // Return the direction index to use it in other logic
            return directionIndex;
        }

        // Return a default value if no direction is detected
        return 0;
    }


    public string GetDirectionName(int directionIndex)
    {
        switch (directionIndex)
        {
            case 0: return "E";
            case 1: return "NEE";
            case 2: return "NE";
            case 3: return "NNE";
            case 4: return "N";
            case 5: return "NNW";
            case 6: return "NW";
            case 7: return "NWW";
            case 8: return "W";
            case 9: return "SWW";
            case 10: return "SW";
            case 11: return "SSW";
            case 12: return "S";
            case 13: return "SSE";
            case 14: return "SE";
            case 15: return "SEE";
            default: return "E"; // Default to East
        }
    }

    private string GetDirectionNumber(int directionIndex)
    {
        return directionIndex.ToString();
    }

    public int GetCurrentDirection()
    {
        return currentDirection;
    }
}
