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

    [SerializeField] private Transform StealPositionTransform;
    Animator AnimatorController;
    NavMeshAgent Agent;
    Transform Waypoints;
    int WaypointCount;
    int LastWaypoint;
    int SelectedCharacter;
    Transform CharacterParent;
    CharacterStatus  Status = CharacterStatus.Stop;
    Vector3 OutPosition;
    GameObject StealedObject;

    bool IsActive = false;
    bool IsThief = false;
    bool IsPolice = false;
    bool IsStealing = false;
    int TryStealTime = 0;
    int TimeStep = 0;
    void Start()
    {
        OutPosition = GameObject.Find("OutPosition").transform.position;
        CharacterParent = transform.GetChild(0);
        
        AnimatorController = CharacterParent.GetComponent<Animator>();

        Agent = transform.GetComponent<NavMeshAgent>();

        Waypoints = Globals.Instance.GetWaypoints();
        WaypointCount = Waypoints.childCount;

        

       


    }

    public void Init(bool isThief,bool isPolice)
    {
        IsPolice = isPolice;
        IsThief = isThief;
        if(isThief) SetAsThief();
        IsActive = true;
        Status = CharacterStatus.Stop;

        if(!IsPolice)
        {
             CharacterParent.GetComponent<AnimEvent>().StealEnd += (() => StealEnded(1));
            InvokeRepeating("SetNewDestination",0,Random.Range(10,18));
            InvokeRepeating("PathControl",0,0.1f);
        }
       

        if(IsPolice)
        {
            SetPoliceSkin();
        }
        else 
        {
            SetCharacterSkin();
        }
        
    }

    void SetAsThief()
    {
        TryStealTime = Random.Range(1,2);
     
        AnimatorController.SetFloat("IdleType",1f);
        AnimatorController.SetFloat("WalkType",1f);

    }
    void SetCharacterSkin()
    {
        SelectedCharacter = Random.Range(1,CharacterParent.childCount-1);

        while(SelectedCharacter == 22 || SelectedCharacter == 23)
        {
            SelectedCharacter = Random.Range(1,CharacterParent.childCount-1);
        }
        
        SetSkin(SelectedCharacter);
    }


    void SetPoliceSkin()
    {
        SelectedCharacter = Random.Range(22,24);
        SetSkin(SelectedCharacter);
    }

    void SetSkin(int selectedSkin)
    {
        foreach(Transform character in CharacterParent)
        {
            if(character.GetSiblingIndex() != 0)
            {
                if(character.GetSiblingIndex() == selectedSkin)
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
        IsStealing = true;
        AnimatorController.SetTrigger("Steal");

        Invoke("CreateStealObject",0.4f);
    }

    void CreateStealObject()
    {
        GameObject stealObject = Waypoints.GetChild(LastWaypoint).GetComponent<Waypoint>().GetStealObject();
        GameObject cloneStealObject = Instantiate(stealObject,GetStealPosition(),stealObject.transform.rotation);
        cloneStealObject.AddComponent<HandFollow>().SetFollowObject(StealPositionTransform);
        StealedObject = cloneStealObject;
    }

    Vector3 GetStealPosition()
    {
        return StealPositionTransform.position;
    }

    public void StealEnded( int i)
    {
        IsStealing = false;

        transform.GetChild(1).GetComponent<ParticleSystem>().Play();
        Destroy(StealedObject);
        CharacterOut();
        
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

    public bool GetStealingStatus()
    {
        return IsStealing;
    }

    public GameObject GetStealObject()
    {
        return StealedObject;
    }

    public void Busted()
    {
        Agent.enabled = false;
        AnimatorController.SetTrigger("Busted");
        
        CancelInvoke("SetNewDestination");
        CancelInvoke("PathControl");
        Invoke("DropObject",1.2f);
    }

    public void DropObject()
    {
        StealedObject.GetComponent<HandFollow>().enabled = false;
        iTween.MoveBy(StealedObject,Vector3.forward * -3f,1.7f);
    }

    public void ArrestThief()
    {
        AnimatorController.SetTrigger("Police");
    }

    void SetDeactive()
    {
        if(IsThief) GameSystem.Instance.OutThief();
        AnimatorController.SetFloat("IdleType",0);
        AnimatorController.SetFloat("WalkType",0);
        IsActive = false;
        IsThief = false;
        TimeStep = 0;
    }
}
