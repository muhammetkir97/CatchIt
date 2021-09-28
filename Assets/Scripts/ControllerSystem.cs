using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IControllerSystem
{
    void Init();

}

public class ControllerSystem : MonoBehaviour
{
    #region Singleton
    public static ControllerSystem  Instance {get; private set;}

    #endregion


    Dictionary<string,float> AxisList = new Dictionary<string, float>();


    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

            var objects = FindObjectsOfType<GameObject>();
            foreach(GameObject obj in objects)
            {
                if(obj.GetComponent<IControllerSystem>() != null)
                {
                    obj.GetComponent<IControllerSystem>().Init();
                }
            }
    
            //DontDestroyOnLoad(gameObject);

        }
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateAxis(string AxisName)
    {
        if(AxisList.ContainsKey(AxisName))
        {
            Debug.Log("Axis Already Exist!");
        }
        else
        {
            AxisList.Add(AxisName,0);
        }

    }

    public void SetAxisValue(string AxisName, float AxisValue)
    {
        if(AxisList.ContainsKey(AxisName))
        {
            AxisList[AxisName] = AxisValue;
        }
        else
        {
            Debug.Log("!Axis Not Exist!");
        }

    }



    public float GetAxis(string AxisName)
    {
        float axisValue = 0;
        if(AxisList.ContainsKey(AxisName))
        {
            axisValue = AxisList[AxisName];
        }
        else
        {
            Debug.Log("!Axis Not Exist!");
        }

        return axisValue;
    }
}
