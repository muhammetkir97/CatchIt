using System.Collections;
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
            Invoke("AddNewCharacter",waitTime);
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
            foreach(Transform tf in CharacterParent)
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

    public void OutThief()
    {
        ThiefCount--;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
