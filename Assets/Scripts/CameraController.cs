using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform CamParent;
    [SerializeField] private Transform Cam;
    int CurrentCamNumber = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        ChangeCameraAngle();
    }


    Vector3 SmoothVel = Vector3.zero;
    float VAngle = 0,HAngle = 0,VSmooth,HSmooth;
    void ChangeCameraAngle()
    {
        
        float vAxis = ControllerSystem.Instance.GetAxis("Vertical");
        float hAxis = ControllerSystem.Instance.GetAxis("Horizontal");

        VAngle = Mathf.SmoothDamp(VAngle,-vAxis * 45,ref VSmooth,0.1f);
        HAngle = Mathf.SmoothDamp(HAngle,hAxis * 45,ref HSmooth,0.1f);

        Vector3 defaultAngle = CamParent.GetChild(0).GetChild(CurrentCamNumber).eulerAngles;
        defaultAngle.x += VAngle;
        defaultAngle.y += HAngle;
        
        
        Cam.eulerAngles = defaultAngle;

    }

    public void ChangeCam(int direction)
    {
        
        int camNumber = CurrentCamNumber + direction;
        if(camNumber < 0) camNumber = CamParent.GetChild(0).childCount-1;

        if(camNumber >= CamParent.GetChild(0).childCount) camNumber = 0;

        CurrentCamNumber = camNumber;
        Debug.Log(camNumber);
        ChangeCurrentCam();
    }

    void ChangeCurrentCam()
    {
        Cam.position = CamParent.GetChild(0).GetChild(CurrentCamNumber).position;
    }
}
