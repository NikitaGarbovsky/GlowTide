using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Level6Triggers : LevelTriggers
{
    private Tilemap tilemap; // Reference to the Tilemap
    private Vector3 levelCenter; // Center position of the level
    [SerializeField] private GameObject EelBossPrefab;


    float threshold = 0.5f;
    private GameObject EelBossSpawnsObject;
    private bool spawnedInFirstPosition = false;
    private Vector3 eelBossTargetPosition1;
    private bool spawnedInSecondPosition = false;
    private Vector3 eelBossTargetPosition2;
    private bool spawnedInThirdPosition = false;
    private Vector3 eelBossTargetPosition3;
    private bool spawnedInFourthosition = false;
    private Vector3 eelBossTargetPosition4;
    private bool spawnedInFifthPosition = false;
    private Vector3 eelBossTargetPosition5;
    private bool spawnedInSixthPosition = false;
    private bool spawnedInSeventhPosition = false;

    private void Start()
    {
        EelBossSpawnsObject = GameObject.FindWithTag("EelBossSpawns");
        eelBossTargetPosition1 = EelBossSpawnsObject.GetComponentInChildren<Transform>().transform.Find("EelBossSpawn1")
            .position;
        eelBossTargetPosition2 = EelBossSpawnsObject.GetComponentInChildren<Transform>().transform.Find("EelBossSpawn2")
            .position;
        eelBossTargetPosition3 = EelBossSpawnsObject.GetComponentInChildren<Transform>().transform.Find("EelBossSpawn3")
            .position;
        eelBossTargetPosition4 = EelBossSpawnsObject.GetComponentInChildren<Transform>().transform.Find("EelBossSpawn4")
            .position;
        eelBossTargetPosition5 = EelBossSpawnsObject.GetComponentInChildren<Transform>().transform.Find("EelBossSpawn5")
            .position;

    }

    private GameObject EelBossInstance;

    public override void ExecuteLevelTrigger(string _sTriggerName)
    {
        if (_sTriggerName == "0_ZoomOutAndShowLevel") // This is the name of the gameobject that is being triggered.
        {
            StartCoroutine(ZoomOutAndShowLevel());
        }

        if (_sTriggerName == "1_EelBoss") // First spawning position for the Eel Boss
        {
            spawnedInFirstPosition = true;
            // Spawn at the first position,
            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition1, Quaternion.identity);

            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();

            eelBossController.SetDirectionForMovement(1, "NEE");
            // Then move to the second position.
            eelBossController.MovetoPoint(eelBossTargetPosition2);
            eelBossController.m_speed = 12;
        }

        if (_sTriggerName == "2_EelBoss") // Second spawning position for the Eel Boss
        {
            spawnedInSecondPosition = true;
            // Spawn at the second position,
            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition2, Quaternion.identity);

            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
            eelBossController.m_speed = 12;
            eelBossController.SetDirectionForMovement(9, "SWW");
            // Then move to the first position.
            eelBossController.MovetoPoint(eelBossTargetPosition1);
        }

        if (_sTriggerName == "3_EelBoss") // Third spawning position for the Eel Boss
        {
            spawnedInThirdPosition = true;

            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition3, Quaternion.identity);


            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
            eelBossController.m_speed = 12;
            eelBossController.SetDirectionForMovement(15, "SEE");
            eelBossController.MovetoPoint(eelBossTargetPosition4);
        }

        if (_sTriggerName == "4_EelBoss") // Fourth spawning position for the Eel Boss
        {
            spawnedInFourthosition = true;
            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition4, Quaternion.identity);

            // 2. Command the Eel Boss to move to the target point
            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
            eelBossController.m_speed = 12;
            eelBossController.SetDirectionForMovement(7, "NWW");
            eelBossController.MovetoPoint(eelBossTargetPosition3);
        }

        if (_sTriggerName == "5_EelBoss") // Fifth spawning position for the Eel Boss
        {
            spawnedInFifthPosition = true;
            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition3, Quaternion.identity);

            // 2. Command the Eel Boss to move to the target point
            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
            eelBossController.m_speed = 12;
            eelBossController.SetDirectionForMovement(0, "E");
            eelBossController.MovetoPoint(eelBossTargetPosition5);
        }

        if (_sTriggerName == "6_EelBoss") // Sixth spawning position for the Eel Boss
        {
            spawnedInSixthPosition = true;
            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition5, Quaternion.identity);

            // 2. Command the Eel Boss to move to the target point
            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
            eelBossController.m_speed = 12;
            eelBossController.SetDirectionForMovement(8, "W");
            eelBossController.MovetoPoint(eelBossTargetPosition1);
        }

        if (_sTriggerName == "7_EelBoss") // Seventh spawning position for the Eel Boss
        {
            spawnedInSeventhPosition = true;
            EelBossInstance = Instantiate(EelBossPrefab, eelBossTargetPosition2, Quaternion.identity);

            // 2. Command the Eel Boss to move to the target point
            BigEelController eelBossController = EelBossInstance.GetComponent<BigEelController>();
            eelBossController.m_speed = 12;
            eelBossController.SetDirectionForMovement(9, "SWW");
            eelBossController.MovetoPoint(eelBossTargetPosition3);
        }

        if (_sTriggerName == "8_LevelTransition")
        {

        }
    }

    private void Update()
    {
        if (EelBossInstance)
        {
            switch (ReturnWhichEelSpawnCount())
            {
                case 1:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition2);
                    break;
                }
                case 2:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition1);
                    break;
                }
                case 3:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition4);
                    break;
                }
                case 4:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition3);
                    break;
                }
                case 5:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition5);
                    break;
                }
                case 6:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition1);
                    break;
                }
                case 7:
                {
                    CheckIfEelBossHasReachedEnd(eelBossTargetPosition3);
                    break;
                }
            }
        }
    }

    private IEnumerator ZoomOutAndShowLevel()
    {
        // 1. Remove all controls from the player
        ManageGameplay.Instance.RemovePlayerControl();
        ManageGameplay.Instance.PlayerCanCallBros = false;
        ManageGameplay.Instance.PlayerCanThrowBros = false;

        // 2. Calculate the offset between the level center and the player
        Vector2 targetOffset = levelCenter - ManageGameplay.Instance.playerCharacter.transform.position;

        // 3. Camera pans to the level center while zooming out
        float zoomOutSize = 10f; // Adjust this value based on the levels size
        float panDuration = 2f;
        yield return StartCoroutine(ManageGameplay.Instance.PanAndZoomCamera(targetOffset, zoomOutSize, panDuration));

        // 4. Hold the camera at the zoomed-out view for a specified duration
        float holdDuration = 2f; // TODO maybe add this as a variable?
        yield return new WaitForSeconds(holdDuration);

        // 5. Camera returns to default size and position following the player
        yield return StartCoroutine(ManageGameplay.Instance.PanAndZoomCamera(Vector2.zero, 2.75f, panDuration));

        // 6. Return all controls back to the player
        ManageGameplay.Instance.ReturnPlayerControl();
        ManageGameplay.Instance.PlayerCanCallBros = true;
        ManageGameplay.Instance.PlayerCanThrowBros = true;
    }

    private int ReturnWhichEelSpawnCount()
    {
        int iSpawnedCount = 0;
        if (spawnedInFirstPosition)
        {
            iSpawnedCount++;
        }

        if (spawnedInSecondPosition)
        {
            iSpawnedCount++;
        }

        if (spawnedInThirdPosition)
        {
            iSpawnedCount++;
        }

        if (spawnedInFourthosition)
        {
            iSpawnedCount++;
        }

        if (spawnedInFifthPosition)
        {
            iSpawnedCount++;
        }

        if (spawnedInSixthPosition)
        {
            iSpawnedCount++;
        }

        if (spawnedInSeventhPosition)
        {
            iSpawnedCount++;
        }

        return iSpawnedCount;
    }

    private void CheckIfEelBossHasReachedEnd(Vector2 _endPosition)
    {
        if (Vector2.Distance(EelBossInstance.transform.position, _endPosition) < 0.1)
        {
            Destroy(EelBossInstance);
        }
    }
}
