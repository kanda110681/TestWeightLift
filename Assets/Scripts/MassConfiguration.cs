using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassConfiguration : MonoBehaviour
{
    public LoadType loadType;
    public float Mass;
    public List<Transform> contactPts = new List<Transform>();
    public List<Rigidbody> connectionRigidBodies = new List<Rigidbody>();
    public Rigidbody rb;

    public void Awake()
    {
        Mass = 0;
        WallDisplay.DisplayWeight(this.Mass.ToString() + " kg");
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void MassReset()
    {
        this.Mass = 1;
        WallDisplay.DisplayWeight(this.Mass.ToString() + " kg");
        if (rb == null)
            return;

        rb.transform.position = Vector3.zero;

       // DestroyImmediate(rb);

       
       // Debug.Log(" Mass reset : ");

       // rb.Sleep();
        rb.mass = 0.5f;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.angularDrag = 0.1f;
       // rb.WakeUp();

       // Debug.Log("Mass reset Pos: " + rb.transform.position + " Vel: " + rb.velocity);
    }

    //private void Update()
    //{
    //   // Debug.Log("Pos: " + rb.transform.position + " Vel: " + rb.velocity);
    //}

    public void UpdateMass(float Mass)
    {
        this.Mass += Mass;
        if (this.Mass < 1)
            this.Mass = 1;

        rb.mass = this.Mass;
        WallDisplay.DisplayWeight(this.Mass.ToString()+" kg");
    }

    public List<Rigidbody> PrepareWeightConfigurationConnections(int nRopes)
    {
        if (nRopes <= 0)
            return null;

        List<int> contacts = new List<int>();

        if(nRopes == 1)
        {
            if (loadType == LoadType.LT_CUBE || loadType == LoadType.LT_DISC)
                contacts.Add(4);
            else if (loadType == LoadType.LT_CYLINDER)
                contacts.Add(2);
            else
                return null;

        }
        else if (nRopes == 2)
        {
            if (loadType == LoadType.LT_CUBE || loadType == LoadType.LT_DISC)
            {
                contacts.Add(3);
                contacts.Add(5);
            }
            else if (loadType == LoadType.LT_CYLINDER)
            {
                contacts.Add(0);
                contacts.Add(4);
            }
            else
                return null;
        }
        else if (nRopes == 3)
        {
            if (loadType == LoadType.LT_CUBE || loadType == LoadType.LT_DISC)
            {
                //contacts.Add(0);
                //contacts.Add(7);
                //contacts.Add(2);

                contacts.Add(6);
                contacts.Add(0);
                contacts.Add(5);
            }
            //else if(loadType == LoadType.LT_DISC)
            //{
            //    contacts.Add(3);
            //    contacts.Add(4);
            //    contacts.Add(5);
            //}
            else if (loadType == LoadType.LT_CYLINDER)
            {
                contacts.Add(0);
                contacts.Add(2);
                contacts.Add(4);
            }
            else
                return null;
        }
        else if (nRopes == 4)
        {
            if (loadType == LoadType.LT_CUBE || loadType == LoadType.LT_DISC)
            {
                contacts.Add(0);
                contacts.Add(6);
                contacts.Add(2);
                contacts.Add(8);
            }
            else if (loadType == LoadType.LT_CYLINDER)
            {
                contacts.Add(0);
                contacts.Add(1);
                contacts.Add(3);
                contacts.Add(4);
            }
            else
                return null;
        }
        else
            return null;

        connectionRigidBodies.Clear();
        foreach (var c in contacts)
        {
            var t = contactPts[c];

            var arb = t.gameObject.GetComponent<Rigidbody>();

            //var arb = t.gameObject.AddComponent<Rigidbody>();
            //arb.useGravity = true;
            //arb.mass = 0.3f;

            //var fjt = t.gameObject.AddComponent<FixedJoint>();
            //fjt.connectedBody = rb;
            //fjt.anchor = Vector3.zero;
            //fjt.axis = Vector3.up;

            connectionRigidBodies.Add(arb);
        }

        return connectionRigidBodies;
    }


    public float ExtraForce = 0.1f;
    private void Update()
    {
        rb.AddForce(Vector3.down * ExtraForce);
    }
}
