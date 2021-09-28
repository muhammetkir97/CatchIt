using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IControllerSystem
{
    public float Range = 50;

    [Header("Movement Direction")]
    public bool IsVertical = true;
    public bool IsHorizontal = true;

    [Header("Axis Names")]
    public string VerticalAxis = "Vertical";
    public string HorizontalAxis = "Horizontal";

    [Header("Hiding When Touch Ended")]
    public bool HideFrame = false;
    public bool HideHandle = false;

    [Header("Joystick Handle")]
    public RectTransform JoystickHandle;
    public Color JoystickHandleHideColor = Color.clear;
    Image JoystickHandleImage;
    Color JoystickHandleFirstColor;

    [Header("Joystick Frame")]
    public RectTransform JoystickFrame;
    public Color JoystickFrameHideColor = Color.clear;
    Image JoystickFrameImage;
    Color JoystickFrameFirstColor;

    RectTransform Canvas;

    float ScreenWidth;
    float ScreenHeight;


    void Awake()
    {


    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Init()
    {
        Canvas = transform.GetComponent<RectTransform>();
        JoystickHandleImage = JoystickHandle.transform.GetComponent<Image>();
        JoystickHandleFirstColor = JoystickHandleImage.color;
        if(HideHandle)
        {
            JoystickHandleImage.color = JoystickHandleHideColor;
        }

        JoystickFrameImage = JoystickFrame.transform.GetComponent<Image>();
        JoystickFrameFirstColor = JoystickFrameImage.color;
        if(HideFrame)
        {
            JoystickFrameImage.color = JoystickFrameHideColor;
        }

        ScreenHeight = Canvas.rect.height;
        ScreenWidth = Canvas.rect.width;
        ControllerSystem.Instance.CreateAxis(VerticalAxis);
        ControllerSystem.Instance.CreateAxis(HorizontalAxis);
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        dragPosition = Vector2.zero;
        JoystickHandle.anchoredPosition = dragPosition;
        
        ControllerSystem.Instance.SetAxisValue(HorizontalAxis,dragPosition.x / Range);
        ControllerSystem.Instance.SetAxisValue(VerticalAxis,dragPosition.y / Range);
        SetColors(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetColors(true);
    }

    Vector2 dragPosition;
    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickFrame,eventData.position,null,out dragPosition);

        if(!IsHorizontal)
        {
            dragPosition.x = 0;
        }

        if(!IsVertical)
        {
            dragPosition.y = 0;
        }

        if(dragPosition.magnitude > Range)
        {
            dragPosition = dragPosition.normalized * Range;
        }
        JoystickHandle.anchoredPosition = dragPosition;

        ControllerSystem.Instance.SetAxisValue(HorizontalAxis,dragPosition.x / Range);
        ControllerSystem.Instance.SetAxisValue(VerticalAxis,dragPosition.y / Range);
        
    }


    void SetColors(bool IsDrag)
    {
        Color handleColor = JoystickHandleFirstColor;
        Color frameColor = JoystickFrameFirstColor;

        if(!IsDrag)
        {
            if(HideFrame)
            {
                frameColor = JoystickFrameHideColor;
            }
            if(HideHandle)
            {
                handleColor = JoystickHandleHideColor;
            }
        }

        JoystickFrameImage.color = frameColor;
        JoystickHandleImage.color = handleColor;

    }


}

