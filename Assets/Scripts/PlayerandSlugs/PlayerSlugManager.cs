using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages the slugs that are assigned to the player. It is attached to the player object
/// and it mainly handles the throwing and calling functionality. 
/// </summary>

public class PlayerSlugManager : MonoBehaviour
{
    // List to hold currently assigned slugs
    public List<GameObject> m_lAssignedSlugs = new List<GameObject>();

    // Adjustable variables
    [Header("Calling Settings")]
    public float m_fCallRadius = 5f; // Radius around the mouse cursor for calling slugs
    public float m_fCallMaxDistance = 10f; // Maximum distance from the player to allow calling
    public LayerMask m_lSlugLayerMask; // Layer mask to identify slugs
    public GameObject m_goCallRadiusEffectPrefab; // Prefab for the call radius visual effect

    [Header("Throwing Settings")]
    public float m_fThrowDistance = 10f; // Fixed distance for throwing slugs
    public float m_fThrowSpeed = 10f; // Speed at which slugs are thrown
    
    [Header("Visual Effects")]
    // The note visual effect that is played when the player calls
    [SerializeField] private GameObject vfxNotesPrefab; 
    
    // Private variables
    private GameObject m_goCallRadiusEffectInstance; // Instance of the call radius visual effect
    private GameObject m_goPlayer; // Reference to the player GameObject

    // Start is called before the first frame update
    void Start()
    {
        m_goPlayer = gameObject; // assigns player gameobject
    }

    // Update is called once per frame
    void Update()
    {
        // Handle slug calling and throwing
        HandleCalling();
        HandleThrowing();
    }

    void HandleCalling()
    {
        if (ManageGameplay.Instance.PlayerCanCallBros) // Only call if the player is allowed to call
        {
            // Get the mouse position in world coordinates
            Vector3 v3MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v3MouseWorldPos.z = 0f; // Set Z to 0 (because 2D)

            // Calculate the distance between the mouse position and the player
            float fDistanceToMouse = Vector2.Distance(m_goPlayer.transform.position, v3MouseWorldPos);

            // If the 'E' key is being held down
            if (Input.GetKey(KeyCode.E))
            {
                // If the mouse is within the allowed distance
                if (fDistanceToMouse <= m_fCallMaxDistance)
                {
                    // Show the call effect radius
                    ShowCallRadiusEffect(v3MouseWorldPos);
                }
                else
                {
                    // Hide the call radius effect
                    HideCallRadiusEffect();
                }
            }
            else
            {
                // Hide the call radius effect
                HideCallRadiusEffect();

                // If the 'E' key was released, and the mouse is within the allowed distance
                if (Input.GetKeyUp(KeyCode.E) && fDistanceToMouse <= m_fCallMaxDistance)
                {
                    // Call slugs
                    CallSlugs(v3MouseWorldPos);
                }
            }
        }
    }


    // Shows the visual effect for the call radius
    void ShowCallRadiusEffect(Vector3 v3MouseWorldPos)
    {
        // Create the effect instance if it doesn't exist
        if (m_goCallRadiusEffectInstance == null)
        {
            m_goCallRadiusEffectInstance = Instantiate(m_goCallRadiusEffectPrefab);
        }
        m_goCallRadiusEffectInstance.SetActive(true);

        // Update the effect's position and scale based on the mouse position and call radius
        m_goCallRadiusEffectInstance.transform.position = v3MouseWorldPos;

        // Set the effect's size to match the call radius
        float fDiameter = m_fCallRadius * 2f;
        m_goCallRadiusEffectInstance.transform.localScale = new Vector3(fDiameter, fDiameter, 1f);
    }


    // Hides the call radius visual effect
    void HideCallRadiusEffect()
    {
        if (m_goCallRadiusEffectInstance != null)
        {
            m_goCallRadiusEffectInstance.SetActive(false);
        }
    }

    // Calls all slugs within the call radius to follow the player
    void CallSlugs(Vector3 v2MouseWorldPos)
    {
        // Instantiate the visual effect for calling at the player's position 
        Vector3 vfxPosition = m_goPlayer.transform.position; 
        GameObject vfxInstance = Instantiate(vfxNotesPrefab, vfxPosition, Quaternion.identity);
        // TODO change this visual effect when Esteri finds a better one
        // Destroy the visual effect after 1 second
        Destroy(vfxInstance, 1f);
        
        // Find all slugs within the call radius using the defined layer mask
        Collider2D[] aSlugsInRadius = Physics2D.OverlapCircleAll(v2MouseWorldPos, m_fCallRadius, m_lSlugLayerMask);

        // Iterate over all slugs found within the radius
        foreach (Collider2D cSlugCollider in aSlugsInRadius)
        {
            GameObject goSlug = cSlugCollider.gameObject;

            // Get the SeaSlugBroFollower component
            SeaSlugBroFollower slugFollower = goSlug.GetComponent<SeaSlugBroFollower>();
            if (slugFollower != null)
            {
                // Make the slug start following the player
                slugFollower.StartFollowingPlayer();

                // Add the slug to the list if it's not already there
                if (!m_lAssignedSlugs.Contains(goSlug))
                {
                    m_lAssignedSlugs.Add(goSlug);
                }
            }
        }
    }
    
    // Handles slug throwing logic
    void HandleThrowing()
    {
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0) && ManageGameplay.Instance.PlayerCanThrowBros)
        {
            ThrowSlug(); // ThrowSlug
        }
    }

    // Throws the nearest slug towards the mouse position
    void ThrowSlug()
    {
        // Return early if there are no slugs to throw
        if (m_lAssignedSlugs.Count == 0) return;

        // Get the mouse position in world coordinates
        Vector2 v2MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Find the nearest slug to the player
        GameObject goNearestSlug = null;
        float fMinDistance = Mathf.Infinity;

        // Iterate over all assigned slugs to find the nearest one
        foreach (GameObject goSlug in m_lAssignedSlugs)
        {
            float fDistance = Vector2.Distance(goSlug.transform.position, m_goPlayer.transform.position);
            if (fDistance < fMinDistance)
            {
                fMinDistance = fDistance;
                goNearestSlug = goSlug;
            }
        }

        // If a nearest slug was found, throw it towards the mouse position
        if (goNearestSlug != null)
        {
            // Remove the slug from the assigned list
            m_lAssignedSlugs.Remove(goNearestSlug);

            // Get the SeaSlugBroFollower component of the slug
            SeaSlugBroFollower slugFollower = goNearestSlug.GetComponent<SeaSlugBroFollower>();
            if (slugFollower != null)
            {
                // Set the throw distance and speed
                slugFollower.m_fThrowDistance = m_fThrowDistance;
                slugFollower.m_fThrowSpeed = m_fThrowSpeed;

                // Command the slug to throw towards the mouse position
                slugFollower.ThrowTowards(v2MouseWorldPos);
            }
        }

        // Draw a debug line from the player's position to the target mouse position
        StartCoroutine(DrawThrowLine(m_goPlayer.transform.position, v2MouseWorldPos, 1f));
    }

    // Coroutine to draw a debug line for visualizing the slug throw
    private IEnumerator DrawThrowLine(Vector3 v3Start, Vector3 v3End, float fDuration)
    {
        float fElapsedTime = 0f;
        while (fElapsedTime < fDuration)
        {
            fElapsedTime += Time.deltaTime;
            Debug.DrawLine(v3Start, v3End, Color.red);
            yield return null;
        }
    }
}
