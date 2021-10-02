using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] private bool IsShelf;
    [SerializeField] private GameObject StealObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetShelfStatus()
    {
        return IsShelf;
    }

    public GameObject GetStealObject()
    {
        return StealObject;
    }
}
