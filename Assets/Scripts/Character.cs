using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


enum CharacterStatus
{
    Walk,
    Stop,
    Buy,
    Steal
}
public class Character : MonoBehaviour
{
    Animator AnimatorController;
    NavMeshAgent Agent;
    Transform Waypoints;
    int WaypointCount;
    int LastWaypoint;
    int SelectedCharacter;
    Transform CharacterParent;
    CharacterStatus  Status = CharacterStatus.Stop;
    void Start()
    {
        CharacterParent = transform.GetChild(0);
        SelectedCharacter = Random.Range(1,CharacterParent.childCount-1);
        AnimatorController = CharacterParent.GetComponent<Animator>();

        foreach(Transform character in CharacterParent)
        {
            if(character.GetSiblingIndex() != 0)
            {
                if(character.GetSiblingIndex() == SelectedCharacter)
                {
                    character.gameObject.SetActive(true);
                }
                else
                {
                    character.gameObject.SetActive(false);
                }
            }
        }
        Agent = transform.GetComponent<NavMeshAgent>();
        Waypoints = Globals.Instance.GetWaypoints();
        WaypointCount = Waypoints.childCount;
        InvokeRepeating("SetNewDestination",Random.Range(0f,2f),Random.Range(12,18));
        InvokeRepeating("PathControl",0,0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
   

    }

    void PathControl()
    {
        if(Agent.remainingDistance < 0.35f && Status != CharacterStatus.Stop)
        {
            Status = CharacterStatus.Stop;
            AnimatorController.SetTrigger("Idle");
        }
    }

    void SetNewDestination()
    {
        CancelInvoke("PathControl");
        int SelectedWaypoint = Random.Range(0,WaypointCount);
        bool isSelected = true;
        while(isSelected)
        {
            isSelected = false;

            if(Globals.Instance.SelectedWaypoints.ContainsKey(SelectedWaypoint))
            {
                isSelected = true;
                SelectedWaypoint = Random.Range(0,WaypointCount);
            }

        }

        
        AnimatorController.SetTrigger("Walk");
        Globals.Instance.SelectedWaypoints.Remove(LastWaypoint);
        Globals.Instance.SelectedWaypoints.Add(SelectedWaypoint,true);
        

        NavMeshHit hit;
        NavMesh.SamplePosition(Waypoints.GetChild(SelectedWaypoint).position,out hit,3,NavMesh.AllAreas);
        
        Agent.destination = hit.position; 
        Status = CharacterStatus.Walk;
        LastWaypoint = SelectedWaypoint;

        InvokeRepeating("PathControl",0.1f,0.1f);
    }
}
