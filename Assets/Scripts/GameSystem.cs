using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;

public class GameSystem : MonoBehaviour
{
    #region Singleton
    public static GameSystem  Instance {get; private set;}

    #endregion

    int ActiveCharacter = 0;
    int ThiefCount = 0;
    bool GameStatus = false;
    Transform CharacterParent;
    [SerializeField] private Character PoliceCharacter;
    [SerializeField] private ParticleSystem PoliceSendEffect;
    [SerializeField] private Transform ArrestPosition;
    [SerializeField] private Camera ArrestCamera;
    [SerializeField] private Camera NormalCamera;
    [SerializeField] private GameObject ArrestBackground;
    [SerializeField] private GameObject ControlPanel;

    [SerializeField] private GameObject RedLight;
    [SerializeField] private GameObject BlueLight;


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

        PoliceCharacter.Init(false,true);
        
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
                    tf.GetComponent<Character>().Init(thiefStatus,false);
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
                    SendPolice(selectedCharacter);
                }
                
            }
        }
    }

    bool IsVisibleOnCamera(GameObject Object) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        if (GeometryUtility.TestPlanesAABB(planes , Object.GetComponent<Collider>().bounds)) return true;

        return false;
    }

    void SendPolice(Character thief)
    {
        ArrestCamera.enabled = true;
        NormalCamera.enabled = false;
        
        thief.transform.position = ArrestPosition.position;
        PoliceSendEffect.transform.position = thief.transform.position;
        thief.transform.rotation = Quaternion.Euler(0,90,0);

        iTween.ScaleTo(ControlPanel,Vector3.zero,0.3f);
        iTween.RotateBy(RedLight,iTween.Hash("z",3,"time",5,"looptype",iTween.LoopType.loop,"easetype",iTween.EaseType.linear));
        iTween.RotateBy(BlueLight,iTween.Hash("z",-3,"time",5,"looptype",iTween.LoopType.loop,"easetype",iTween.EaseType.linear));

        iTween.RotateBy(ArrestBackground,iTween.Hash("z",1,"time",5,"looptype",iTween.LoopType.loop,"easetype",iTween.EaseType.linear));
        iTween.MoveBy(ArrestBackground,iTween.Hash("z",0.5,"time",0.3,"looptype",iTween.LoopType.pingPong,"easetype",iTween.EaseType.linear));
        PoliceSendEffect.Play();
        PoliceCharacter.transform.position = thief.transform.position - (thief.transform.forward * 1.6f);
        PoliceCharacter.transform.LookAt(thief.transform,Vector3.up);
        PoliceCharacter.ArrestThief();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
