using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using UnityEngine.AI;
public class GameSystem : MonoBehaviour
{
    #region Singleton
    public static GameSystem  Instance {get; private set;}

    #endregion

    int ArrestedThiefCount = 0;
    int ActiveCharacter = 0;
    int ThiefCount = 0;
    int TotalThiefCount;
    bool GameStatus = false;
    Transform CharacterParent;

    [Header("Arrest")]
    [SerializeField] private Character PoliceCharacter;
    [SerializeField] private ParticleSystem PoliceSendEffect;
    [SerializeField] private Transform ArrestPosition;
    [SerializeField] private Camera ArrestCamera;
    [SerializeField] private Camera NormalCamera;
    [SerializeField] private Camera ThiefCamera;
    [SerializeField] private GameObject ArrestBackground;
    [SerializeField] private GameObject ControlPanel;

    [SerializeField] private GameObject RedLight;
    [SerializeField] private GameObject BlueLight;

    [Header("Top Bar")]

    [SerializeField] private Image UiMask;

    [SerializeField] private Image[] LevelStars;
    [SerializeField] private Image ProgressBar;

    [Header("End Popup")]

    [SerializeField] private GameObject EndPopup;
    [SerializeField] private Image[] EndLevelStars;
    [SerializeField] private GameObject RetryButton;
    [SerializeField] private  GameObject NextButton;

    [Header("Pause Popup")]

    [SerializeField] private  GameObject PausePopup;
    [SerializeField] private  GameObject PauseBackground;

    [Header("Navigation")]
    [SerializeField] private NavMeshSurface OuterPlane;
    [SerializeField] private NavMeshSurface[] LevelSurfaces;


    [Header("Thief Camera")]
    [SerializeField] private  GameObject ThiefCamSprite;
    [SerializeField] private  Image ThiefCamImage;

    Character LastThief;

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
        CreateCurrentLevel();
        StartGame();
    }

    void StartGame()
    {
        
        ControlProgress();
        int firstCharCount = Mathf.RoundToInt(Globals.Instance.GetLevelCharacterLimit() * 0.75f);
        float waitTime = 0;
        for(int i=0; i<firstCharCount; i++)
        {
            waitTime = (1f + (i * 0.4f) )* i;
            Invoke("AddNewCharacter",waitTime + 0.1f);
        }

        InvokeRepeating("CharacterCountControl",waitTime + 1,3);
        InvokeRepeating("OutCharacter",waitTime + 5,20);

        PoliceCharacter.Init(false,true);
        
    }

    void CreateCurrentLevel()
    {
        Debug.Log(Globals.Instance.GetCurrentLevel());
        Transform levelParent = GameObject.Find("Levels").transform;

        foreach(Transform level in levelParent)
        {
            if(level.GetSiblingIndex() == Globals.Instance.GetCurrentLevel())
            {
                level.gameObject.SetActive(true);
            }
            else
            {
                level.gameObject.SetActive(false);
            }
            
        }
        OuterPlane.BuildNavMesh();
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
        Debug.Log(ActiveCharacter);
        AddNewCharacter();
    }

    void AddNewCharacter()
    {
        if(ActiveCharacter < Globals.Instance.GetLevelCharacterLimit())
        {
            List<Transform> randomCharacters = RandomParentList(CharacterParent);

            foreach(Transform tf in randomCharacters)
            {
                if(!tf.GetComponent<Character>().GetStatus() && tf.GetComponent<Character>().GetSelectableStatus())
                {
                    bool thiefStatus = false;
                    if(ThiefCount < Globals.Instance.GetLevelThiefLimit() && TotalThiefCount < Globals.Instance.GetLevelThiefCount())
                    {
                        thiefStatus = true;
                        ThiefCount++;
                        TotalThiefCount++;
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
        if(TotalThiefCount >= Globals.Instance.GetLevelThiefCount())
        {
            EndGame();
        }
        else
        {
            ThiefCount--;
            ActiveCharacter--;
        }

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
                    ArrestedThiefCount++;
                    
                    ControlProgress();
                }
                
            }
        }
    }

    bool IsVisibleOnCamera(GameObject Object) {
        if(Object != null || Camera.main != null)
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
            if(planes != null)
            {
                if (GeometryUtility.TestPlanesAABB(planes , Object.GetComponent<Collider>().bounds)) return true;
            }


        }

        return false;
    }

    void SendPolice(Character thief)
    {
        SetArrestEffects(true);
        SetArrestCamera(true);
        
        PoliceCharacter.transform.GetComponent<NavMeshAgent>().enabled = false;
        LastThief = thief;

        thief.transform.position = ArrestPosition.position;
        PoliceSendEffect.transform.position = thief.transform.position;
        thief.transform.rotation = Quaternion.Euler(0,90,0);
        
        PoliceSendEffect.Play();
        PoliceCharacter.transform.position = thief.transform.position - (thief.transform.forward * 1.6f);
        PoliceCharacter.transform.LookAt(thief.transform,Vector3.up);
        PoliceCharacter.ArrestThief();
        Invoke("BackToMarket",2.5f);
    }

    void SetArrestCamera(bool status)
    {
        ArrestCamera.enabled = status;
        NormalCamera.enabled = !status;
    }

    void BackToMarket()
    {
        SetArrestCamera(false);
        SetArrestEffects(false);
        
        LastThief.SetDeactiveFast();
        PoliceCharacter.SetDeactiveFast();
    }

    void SetArrestEffects(bool status)
    {
        if(status)
        {
            UiMask.color = Color.clear;
            iTween.ScaleTo(ControlPanel,Vector3.zero,0.3f);
            iTween.RotateBy(RedLight,iTween.Hash("z",3,"time",5,"looptype",iTween.LoopType.loop,"easetype",iTween.EaseType.linear));
            iTween.RotateBy(BlueLight,iTween.Hash("z",-3,"time",5,"looptype",iTween.LoopType.loop,"easetype",iTween.EaseType.linear));

            iTween.RotateBy(ArrestBackground,iTween.Hash("z",1,"time",5,"looptype",iTween.LoopType.loop,"easetype",iTween.EaseType.linear));
            iTween.MoveBy(ArrestBackground,iTween.Hash("z",0.5,"time",0.3,"looptype",iTween.LoopType.pingPong,"easetype",iTween.EaseType.linear));
        }
        else
        {
            UiMask.color = Color.white;
            iTween.Stop(ControlPanel);
            iTween.Stop(RedLight);
            iTween.Stop(BlueLight);
            iTween.Stop(ArrestBackground);
        }
    }

    void ControlProgress()
    {
        ProgressBar.fillAmount = ArrestedThiefCount/(float)Globals.Instance.GetLevelThiefCount();
        if(ArrestedThiefCount == 6)
        {
            SetLevelStar(3);
        }
        else if(ArrestedThiefCount >= 4)
        {
            SetLevelStar(2);
        }
        else if(ArrestedThiefCount >= 2)
        {
            SetLevelStar(1);
        }
        else
        {
            SetLevelStar(0);
        }
    }

    void SetLevelStar(int level)
    {
        if(level == 0)
        {
            for(int i=0; i<3; i++)
            {
                LevelStars[i].color = Color.clear;
            }
        }
        else
        {
            for(int i=1; i<4; i++)
            {
                if(i <= level)
                {
                    LevelStars[i-1].color = Color.white;
                }
                else
                {
                    LevelStars[i-1].color = Color.clear;
                }
            }
        }

    }

    void EndGame()
    {
        iTween.ScaleTo(EndPopup,Vector3.one,0.5f);
        if(ArrestedThiefCount >= 2)
        {
            RetryButton.SetActive(false);
            NextButton.SetActive(true);

            if(ArrestedThiefCount >= 2) iTween.ScaleTo(EndLevelStars[0].gameObject,iTween.Hash("scale",Vector3.one,"time",0.3f,"delay",0.3f,"easetype",iTween.EaseType.easeOutBounce));
            if(ArrestedThiefCount >= 4) iTween.ScaleTo(EndLevelStars[1].gameObject,iTween.Hash("scale",Vector3.one,"time",0.3f,"delay",0.5f,"easetype",iTween.EaseType.easeOutBounce));
            if(ArrestedThiefCount >= 6) iTween.ScaleTo(EndLevelStars[2].gameObject,iTween.Hash("scale",Vector3.one,"time",0.3f,"delay",0.8f,"easetype",iTween.EaseType.easeOutBounce));
        }
        else
        {
            RetryButton.SetActive(true);
            NextButton.SetActive(false);
        }
    }

    public void ShowPauseMenu()
    {
        Time.timeScale = 0;
        SetPausePopupVisible(true);
        PauseBackground.transform.localScale = Vector3.zero;
        iTween.ScaleTo(PauseBackground,iTween.Hash("scale",Vector3.one,"time",0.25f,"easetype",iTween.EaseType.spring,"ignoretimescale",true));
    }

    public void HidePauseMenu()
    {
        Time.timeScale = 1;
        Invoke("HidePauseLate",0.25f);
        iTween.ScaleTo(PauseBackground,iTween.Hash("scale",Vector3.zero,"time",0.25f,"easetype",iTween.EaseType.spring,"ignoretimescale",true));
    }

    void HidePauseLate()
    {
        SetPausePopupVisible(false);
    }

    void SetPausePopupVisible(bool status)
    {
        PausePopup.SetActive(status);
    }

    void ShowThiefCam()
    {
        iTween.ValueTo(gameObject,iTween.Hash("name","ThiefCam","from",0.7f,"to",1f,"time",0.3f,"onupdate","ThiefCamAnim","looptype",iTween.LoopType.pingPong));
        iTween.ScaleTo(ThiefCamSprite,Vector3.one,0.3f);

    }

    void HideThiefCam()
    {
        iTween.StopByName("ThiefCam");
        iTween.ScaleTo(ThiefCamSprite,Vector3.zero,0.3f);
    }

    public void SetThiefCamera(Transform thief,bool status)
    {
        ThiefCamera.enabled = status;
        if(status)
        {
            ShowThiefCam();
        }
        else
        {
            HideThiefCam();
        }
        
        ThiefCamera.transform.position = thief.position + (Vector3.up * 3) + (transform.forward * 1f);
        ThiefCamera.transform.LookAt(thief);
        
        
    }

    void ThiefCamAnim(float val)
    {
        Color tempColor = ThiefCamImage.color;
        tempColor.a = val;
        ThiefCamImage.color = tempColor;
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
