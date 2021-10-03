using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInteract : MonoBehaviour
{
    [Header("Camera Control")]
    
    [SerializeField] private Button BtnRightCam;
    [SerializeField] private Button BtnLeftCam;
    [SerializeField] private CameraController CamController;


    [Header("Game Interface")]
    [SerializeField] private Button BtnAlarm;
    [SerializeField] private Button BtnExit;


    

    void Start()
    {
        BtnRightCam.onClick.AddListener(() => ClickChangeCam(1));
        BtnLeftCam.onClick.AddListener(() => ClickChangeCam(-1));
        
        BtnAlarm.onClick.AddListener(ClickAlarm);
        
        BtnExit.onClick.AddListener(ClickExit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void ClickChangeCam(int direction)
    {
       CamController.ChangeCam(direction);
    }

    void ClickAlarm()
    {
        BtnAlarm.interactable = false;
        GameSystem.Instance.StartAlarm();
        Invoke("ActivateAlarmButton",Globals.Instance.GetAlarmCooldown());
    }

    void ActivateAlarmButton()
    {
        BtnAlarm.interactable = true;
    }

    void ClickExit()
    {
        SceneManager.LoadScene("MainScene",LoadSceneMode.Single);

    }


}
