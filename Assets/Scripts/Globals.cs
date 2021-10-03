using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    #region Singleton
    public static Globals  Instance {get; private set;}

    #endregion

    public Dictionary<int,bool> SelectedWaypoints = new Dictionary<int, bool>();

    int CameraMaxV = 45;
    int CameraMaxH = 45;
    int CurrentLevel = 0;

    int[] CharacterLimits = {6,6,6,6,6,6};
    int[] ThiefLimits = {1,1,1,2,2,2};
    int[] LevelThiefCount = {6,6,6,6,6};

    float AlarmCooldown = 2;

    void Awake()
    {
        if(Instance == null)
        {

            Instance = this;
        }
    }

    void ReadPlayerValues()
    {
        CurrentLevel = PlayerPrefs.GetInt("Level",0);
    }

    public int GetLevelThiefLimit()
    {
        return ThiefLimits[GetCurrentLevel()];
    }

    public int GetLevelCharacterLimit()
    {
        return CharacterLimits[GetCurrentLevel()];
    }


    ///////////////////////////
    //////////////////////////
    //LEVEL AYARI
    ///////////////////////
    public int GetCurrentLevel()
    {
        return CurrentLevel;
        //return 1
    }

    public GameObject GetCurrentLevelObject()
    {
        return GameObject.Find($"Levels/{GetCurrentLevel()}");
    }

    public Transform GetWaypoints()
    {

        return GetCurrentLevelObject().transform.GetChild(0);
    }

    public Vector2 GetCameraMaxAngle()
    {
        Vector2 maxAngle = new Vector2(CameraMaxH,CameraMaxV);
        return maxAngle;
    }

    public float GetAlarmCooldown()
    {
        return AlarmCooldown;
    }

    public int GetLevelThiefCount()
    {
        return LevelThiefCount[GetCurrentLevel()];
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
