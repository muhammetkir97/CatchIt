using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandFollow : MonoBehaviour
{
    Transform FollowObject;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.position = FollowObject.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetFollowObject(Transform tf)
    {
        FollowObject = tf;
    }
}
