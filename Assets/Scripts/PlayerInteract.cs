using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerInteract : MonoBehaviour
{
    [Header("Camera Controll")]
    [SerializeField] private Button BtnRightCam,BtnLeftCam;
    [SerializeField] private CameraController CamController;

    

    void Start()
    {
        BtnRightCam.onClick.AddListener(() => ClickChangeCam(1));
        BtnLeftCam.onClick.AddListener(() => ClickChangeCam(-1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void ClickChangeCam(int direction)
    {
       CamController.ChangeCam(direction);
    }


}
