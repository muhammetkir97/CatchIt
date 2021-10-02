﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    #region Singleton
    public static GameSystem  Instance {get; private set;}

    #endregion

    int ActiveCharacter = 0;
    int ThiefCount = 0;
    bool GameStatus = false;
    Transform CharacterParent;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        CharacterParent = GameObject.Find("Characters").transform;

        StartGame();
    }

    void StartGame()
    {
        int firstCharCount = Mathf.RoundToInt(Globals.Instance.GetLevelCharacterLimit() * 0.75f);
        float waitTime = 0;
        for(int i=0; i<firstCharCount; i++)
        {
            waitTime = (1f + (i * 0.4f) )* i;
            Invoke("AddNewCharacter",waitTime + 0.1f);
        }

        InvokeRepeating("CharacterCountControl",waitTime + 1,10);
        InvokeRepeating("OutCharacter",waitTime + 5,20);

    }

    void OutCharacter()
    {
        for(int i=0; i< CharacterParent.childCount; i++)
        {
            int randomChar = Random.Range(0,CharacterParent.childCount);
            
            Character charControl = CharacterParent.GetChild(randomChar).GetComponent<Character>();
            if(charControl.GetStatus())
            {
                if(!charControl.GetThiefStatus())
                {
                    charControl.CharacterOut();
                    ActiveCharacter--;
                    break;
                }
            }
        }
      
    }


    void CharacterCountControl()
    {
        AddNewCharacter();
    }

    void AddNewCharacter()
    {
        if(ActiveCharacter < Globals.Instance.GetLevelCharacterLimit())
        {
            List<Transform> randomCharacters = RandomParentList(CharacterParent);

            foreach(Transform tf in randomCharacters)
            {
                if(!tf.GetComponent<Character>().GetStatus())
                {
                    bool thiefStatus = false;
                    if(ThiefCount < Globals.Instance.GetLevelThiefLimit())
                    {
                        thiefStatus = true;
                        ThiefCount++;
                    }
                    tf.GetComponent<Character>().Init(thiefStatus);
                    ActiveCharacter++;
                    break;
                }
            }
        }
    }

    List<Transform> RandomParentList(Transform parent)
    {
        List<Transform> randomCharacters = new List<Transform>();

        foreach(Transform tf in parent)
        {
            randomCharacters.Add(tf);
        }

        int n = randomCharacters.Count;  
        while (n > 1) 
        {  
            n--;  
            int k = Random.Range(0,n + 1);  
            Transform tf = randomCharacters[k];  
            randomCharacters[k] = randomCharacters[n];  
            randomCharacters[n] = tf;  
        } 
            
        return randomCharacters;
    }

    public void OutThief()
    {
        ThiefCount--;
        ActiveCharacter--;
    }

    public void StartAlarm()
    {
        foreach(Transform tf in CharacterParent)
        {
            Character selectedCharacter = tf.GetComponent<Character>();
            if(selectedCharacter.GetStealingStatus())
            {
                if(IsVisibleOnCamera(selectedCharacter.GetStealObject()))
                {
                    selectedCharacter.Busted();
                }
                
            }
        }
    }

    bool IsVisibleOnCamera(GameObject Object) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes , Object.GetComponent<Collider>().bounds)) return true;

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
