using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IControllerSystem
{
    public float Sensitivity = 1;
    [Header("Movement Direction")]
    [SerializeField] private bool IsVertical = true;
    [SerializeField] private bool IsHorizontal = true;
    
    [Header("Axis Names")]
    [SerializeField] private string VerticalAxis = "Vertical";
    [SerializeField] private string HorizontalAxis = "Horizontal";


    private Vector2 AxisValues;
    public void Init()
    {
        ControllerSystem.Instance.CreateAxis(VerticalAxis);
        ControllerSystem.Instance.CreateAxis(HorizontalAxis);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        CancelInvoke("ResetDrag");
        AxisValues = eventData.delta;

        if(!IsHorizontal)
        {
            AxisValues.x = 0;
        }

        if(!IsVertical)
        {
            AxisValues.y = 0;
        }

        ControllerSystem.Instance.SetAxisValue(HorizontalAxis,AxisValues.x * Sensitivity);
        ControllerSystem.Instance.SetAxisValue(VerticalAxis,AxisValues.y * Sensitivity);

        Invoke("ResetDrag",0.1f);
    }

    void ResetDrag()
    {
        ControllerSystem.Instance.SetAxisValue(HorizontalAxis,0);
        ControllerSystem.Instance.SetAxisValue(VerticalAxis,0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ControllerSystem.Instance.SetAxisValue(HorizontalAxis,0);
        ControllerSystem.Instance.SetAxisValue(VerticalAxis,0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
