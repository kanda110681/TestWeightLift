using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlingController : MonoBehaviour
{
    public LiftSettings ls;

    public SpringJoint joint;
    public Rigidbody rb;

    public Transform topJoint;
    public Transform bottomJoint;
    LineRenderer lr;

    public void Awake()
    {
        lr = GetComponent<LineRenderer>();
        joint = GetComponent<SpringJoint>();
        rb = GetComponent<Rigidbody>();

        lr.startWidth = lr.endWidth = ls.chainDispSize;
        lr.positionCount = 0;
    }

    public void ActivateRigidBody()
    {
        if (rb.IsSleeping())
        {
            rb.WakeUp();
        }
    }

    public void DeactivateRigidBody()
    {
        if (rb == null)
            return;
        rb.Sleep();
    }
      

    private void FixedUpdate()
    {
        joint.damper = ls.Damper;
        joint.spring = ls.spring;
        UpdateChainData();
    }

    // Update is called once per frame
    void Update()
    {
        lr.startWidth = lr.endWidth = ls.chainDispSize;
    //    Debug.Log("Force: " + joint.currentForce.magnitude + " Torque: " + joint.currentTorque.magnitude);
        UpdateChainData();
    }

    void UpdateChainData()
    {
        lr.positionCount = 2;
        lr.SetPosition(0, topJoint.position);
        lr.SetPosition(1, bottomJoint.position);
    }

    public void SlingReset()
    {
        if (joint == null)
            return;
        joint.spring = 1;
        DeactivateRigidBody();
    }
}
