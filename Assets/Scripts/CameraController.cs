using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform CamParent;
    Transform CurrentCamParent;
    [SerializeField] private Transform Cam;
    int CurrentCamNumber = 0;
    void Start()
    {
        CurrentCamParent = CamParent.GetChild(Globals.Instance.GetCurrentLevel());
        ChangeCurrentCam();
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
        float hAxis = ControllerSystem.Instance.GetAxis("HorizontalDrag");
        float vAxis = ControllerSystem.Instance.GetAxis("VerticalDrag");

        Vector3 targetAngle = Cam.eulerAngles;

        targetAngle.x += -vAxis;
        targetAngle.y += hAxis;

        

        Cam.eulerAngles = Vector3.SmoothDamp(Cam.eulerAngles,targetAngle,ref SmoothVel,0.2f);

        /*
        
        HAngle = Mathf.SmoothDamp(HAngle,hAxis * Globals.Instance.GetCameraMaxAngle().x,ref HSmooth,0.1f);
        VAngle = Mathf.SmoothDamp(VAngle,-vAxis * Globals.Instance.GetCameraMaxAngle().y,ref VSmooth,0.1f);
        

        Vector3 defaultAngle = CurrentCamParent.GetChild(CurrentCamNumber).eulerAngles;
        defaultAngle.x += VAngle;
        defaultAngle.y += HAngle;
        
        
        Cam.eulerAngles = defaultAngle;
        */

    }

    public void ChangeCam(int direction)
    {
        
        int camNumber = CurrentCamNumber + direction;
        if(camNumber < 0) camNumber = CurrentCamParent.childCount-1;

        if(camNumber >= CurrentCamParent.childCount) camNumber = 0;

        CurrentCamNumber = camNumber;
        Debug.Log(camNumber);
        ChangeCurrentCam();
    }

    void ChangeCurrentCam()
    {
        Cam.position = CurrentCamParent.GetChild(CurrentCamNumber).position;
    }
}
