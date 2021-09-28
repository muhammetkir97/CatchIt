using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Character : MonoBehaviour
{
    NavMeshAgent Agent;
    void Start()
    {
        Agent = transform.GetComponent<NavMeshAgent>();
        InvokeRepeating("SetNewDestination",Random.Range(0,3f),15f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetNewDestination()
    {
        Agent.destination = Globals.Instance.GetWaypoints().GetChild(Random.Range(0,5)).position; 
    }
}
