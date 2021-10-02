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
    Vector3 OutPosition;

    bool IsActive = false;
    bool IsThief = false;
    int TryStealTime = 0;
    int TimeStep = 0;
    void Start()
    {
        OutPosition = GameObject.Find("OutPosition").transform.position;
        CharacterParent = transform.GetChild(0);
        SelectedCharacter = Random.Range(1,CharacterParent.childCount-1);
        AnimatorController = CharacterParent.GetComponent<Animator>();

        Agent = transform.GetComponent<NavMeshAgent>();

        Waypoints = Globals.Instance.GetWaypoints();
        WaypointCount = Waypoints.childCount;

        SetCharacterSkin();

       


    }

    public void Init(bool isThief)
    {
        IsThief = isThief;
        if(isThief) SetAsThief();
        IsActive = true;
        Status = CharacterStatus.Stop;

        CharacterParent.GetComponent<AnimEvent>().StealEnd += (() => StealEnded(1));

        InvokeRepeating("SetNewDestination",0,Random.Range(10,18));
        InvokeRepeating("PathControl",0,0.1f);
        
    }

    void SetAsThief()
    {
        TryStealTime = Random.Range(1,2);
     
        AnimatorController.SetFloat("IdleType",1f);
        AnimatorController.SetFloat("WalkType",1f);

    }
    void SetCharacterSkin()
    {
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
            TimeStep++;
            Status = CharacterStatus.Stop;
            AnimatorController.ResetTrigger("Walk");
            AnimatorController.SetTrigger("Idle");
            iTween.RotateTo(gameObject,Waypoints.GetChild(LastWaypoint).eulerAngles,0.5f);

            if(IsThief)
            {
                if(TimeStep == TryStealTime)
                {
                    StartStealing();
                }
            }
        }
    }

    void StartStealing()
    {
        AnimatorController.SetTrigger("Steal");

    }

    public void StealEnded( int i)
    {
        Debug.Log("steal end");
        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        CharacterOut();
        GameSystem.Instance.OutThief();
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

        Status = CharacterStatus.Stop;
        //AnimatorController.SetTrigger("Idle");

        
        AnimatorController.SetTrigger("Walk");

        Globals.Instance.SelectedWaypoints.Remove(LastWaypoint);
        Globals.Instance.SelectedWaypoints.Add(SelectedWaypoint,true);
        
        Agent.destination = Waypoints.GetChild(SelectedWaypoint).position; 
        Status = CharacterStatus.Walk;
        LastWaypoint = SelectedWaypoint;

        InvokeRepeating("PathControl",0.1f,0.1f);
    }

    public void CharacterOut()
    {
        //CancelInvoke("PathControl");
        CancelInvoke("SetNewDestination");
        Globals.Instance.SelectedWaypoints.Remove(LastWaypoint);
        AnimatorController.SetTrigger("Walk");
        Agent.destination = OutPosition; 
        Invoke("SetDeactive",15f);
    }

    public bool GetStatus()
    {
        return IsActive;
    }

    public bool GetThiefStatus()
    {
        return IsThief;
    }

    void SetDeactive()
    {
                AnimatorController.SetFloat("IdleType",0);
        AnimatorController.SetFloat("WalkType",0);
        IsActive = false;
    }
}
