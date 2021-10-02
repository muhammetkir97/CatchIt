using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimEvent : MonoBehaviour
{
    public UnityAction StealEnd;
    void Start()
    {
        Animator AnimatorController = transform.GetComponent<Animator>();
        AnimationEvent evt;
        evt = new AnimationEvent();

        evt.intParameter = 1;
        evt.time = 1.3f;
        evt.functionName = "StealEnded";
        evt.objectReferenceParameter = this;

        AnimatorController.runtimeAnimatorController.animationClips[4].AddEvent(evt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StealEnded(int i)
    {
        if(StealEnd != null) StealEnd();
    }
}
