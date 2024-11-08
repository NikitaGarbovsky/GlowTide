using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is a spawner for scared bro sea slugs, how it works:
/// It has two states: Occupied and Unoccupied,
/// 1. During the Occupied state, the player is able to bring a bro snack to the bush,
/// 2. When a snack is brought to the bush, the snack is consumed,
/// 3. A scared sea slug spawns on the SlugSpawnPoint,
///     3.1. The scared sea slug is set to "FollowingPlayer" state,
/// 4. The state of this bush is set to Unoccupied.
/// </summary>
public class ScaredSlugBushManager : MonoBehaviour
{
    // Enum to represent states of the bush
    public enum BushState
    {
        Occupied,
        Unoccupied
    }
    // Holds the location the scared bro will spawn when this bush is brought a bro snack. 
    [SerializeField] public GameObject SlugSpawnPoint;
    [SerializeField] public GameObject SeaSlugPrefabToSpawn;
    public BushState _bushState = BushState.Occupied;

    float shakeAngle = 5f; // Maximum rotation angle for shaking
    float shakeSpeed = 10f; // Speed of shake
    float shakeDuration = 1f; // Duration to shake in seconds
    float pauseDuration = 4f; // Duration to pause in seconds

    bool isShaking = false;

    AudioSource m_audioSource;

    void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        // Start the main coroutine that alternates between shaking and pausing
        StartCoroutine(ShakeAndPause());
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player triggered this event and the bush is occupied
        if (other.CompareTag("Player") && _bushState == BushState.Occupied && other.GetComponent<PlayerSlugManager>().m_bHasBroSnack)
        {
            // Spawn the sea slug at the spawn point's position and rotation
            Instantiate(SeaSlugPrefabToSpawn, SlugSpawnPoint.transform.position, SlugSpawnPoint.transform.rotation);
            
            // Change the bush state to Unoccupied after spawning the sea slug
            _bushState = BushState.Unoccupied;
            m_audioSource.Play();
        }
    }

    IEnumerator ShakeAndPause()
    {
        while (_bushState == BushState.Occupied)
        {
            // Start shaking
            isShaking = true;
            float shakeEndTime = Time.time + shakeDuration;

            while (Time.time < shakeEndTime)
            {
                if (isShaking)
                {
                    // Rotate back and forth
                    float shake = Mathf.Sin(Time.time * shakeSpeed) * shakeAngle;
                    transform.rotation = Quaternion.Euler(0, 0, shake);
                }
                yield return null; // Wait for the next frame
            }

            // Stop shaking
            transform.rotation = Quaternion.identity;
            isShaking = false;

            // Wait for pause duration
            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
