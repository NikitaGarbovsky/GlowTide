using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class CPlayerMovement : MonoBehaviour
{
    [SerializeField] public AIDestinationSetter aiDestinationSetter;
    private AIPath awef; 
    [SerializeField] public GameObject target;

    private Transform targetTransform;
    // Start is called before the first frame update
    void Start()
    {
        awef = GetComponent<AIPath>();
        targetTransform = target.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        awef.destination = targetTransform.position;
    }
}
