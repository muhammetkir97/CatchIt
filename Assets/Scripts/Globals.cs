﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    #region Singleton
    public static Globals  Instance {get; private set;}

    #endregion

    int CameraMaxV = 45;
    int CameraMaxH = 45;
    int CurrentLevel = 0;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }


    public int GetCurrentLevel()
    {
        return CurrentLevel;
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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}