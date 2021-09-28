using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealObject : MonoBehaviour
{
    ParticleSystem StealParticle;
    void Start()
    {
        GameObject particle = Resources.Load<GameObject>("Partricles/StealParticle");
        GameObject clone = Instantiate(particle,transform.position,Quaternion.identity);
        clone.transform.SetParent(transform);
        clone.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartStealing()
    {
        StealParticle.Play();
    }
}
