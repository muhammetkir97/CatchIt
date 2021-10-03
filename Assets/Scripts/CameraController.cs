using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform CamParent,CamDummy;
    Transform CurrentCamParent;
    [SerializeField] private Transform Cam;
    [SerializeField] private TextMeshProUGUI TxtCamNumber;
    [SerializeField] private GameObject RecCircle;
    int CurrentCamNumber = 0;
    void Start()
    {
        iTween.ScaleTo(RecCircle,iTween.Hash("scale",Vector3.zero,"time",0.02f,"delay",0.5f,"looptype",iTween.LoopType.pingPong,"easetype",iTween.EaseType.linear));
        CurrentCamParent = CamParent.GetChild(Globals.Instance.GetCurrentLevel());
        CurrentCamParent.gameObject.SetActive(true);
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

    float HAngle = 0,VAngle = 0;
    void ChangeCameraAngle()
    {
        float hAxis = ControllerSystem.Instance.GetAxis("HorizontalDrag");
        float vAxis = ControllerSystem.Instance.GetAxis("VerticalDrag");

        float xMax = Globals.Instance.GetCameraMaxAngle().x;
        float yMax = Globals.Instance.GetCameraMaxAngle().y;

        HAngle += hAxis;
        VAngle += -vAxis;

        if(HAngle > yMax) HAngle = yMax;
        if(HAngle < -yMax) HAngle = -yMax;

        if(VAngle > xMax) VAngle = xMax;
        if(VAngle < -xMax) VAngle = -xMax;

        Quaternion targetQuaternion = Quaternion.Euler(VAngle,HAngle,-HAngle/3f);



    
        Cam.localRotation = SmoothDampQuaternion(Cam.localRotation,targetQuaternion,ref SmoothVel, 0.4f);
        //Cam.rotation = Quaternion.Slerp(Cam.rotation,CamDummy.rotation,Time.time * 0.1f);


    }

    Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
    {
        Vector3 c = current.eulerAngles;
        Vector3 t = target.eulerAngles;
        return Quaternion.Euler(
            Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
            Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
            Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
        );
    }

    public float ConvertToAngle180(float input)
    {       
        while (input > 360)
        {
            input = input - 360;
        } 
        while (input < -360)
        {
            input = input + 360;
        }
        if (input > 180)
        {
            input = input - 360;        
        }
        if (input < -180)
            input = 360+ input;
        return input;
    }

    public void ChangeCam(int direction)
    {

        int camNumber = CurrentCamNumber + direction;
        if(camNumber < 0) camNumber = CurrentCamParent.childCount-1;

        if(camNumber >= CurrentCamParent.childCount) camNumber = 0;

        CurrentCamNumber = camNumber;

        Debug.Log(camNumber);
        ChangeCurrentCam();
        HAngle = 0;
        VAngle = 0;
    }

    void ChangeCurrentCam()
    {
        TxtCamNumber.text = $"Camera {CurrentCamNumber + 1}";
        CurrentCamParent.GetChild(CurrentCamNumber).GetChild(0).localEulerAngles = Vector3.zero;
        Cam.parent = CurrentCamParent.GetChild(CurrentCamNumber).transform;
        CamDummy.parent = CurrentCamParent.GetChild(CurrentCamNumber).GetChild(0).transform;

        Cam.localPosition = Vector3.zero;
        CamDummy.localPosition = Vector3.zero;

        Cam.localEulerAngles =  Vector3.zero;
        CamDummy.localEulerAngles = Vector3.zero;
    }


}
