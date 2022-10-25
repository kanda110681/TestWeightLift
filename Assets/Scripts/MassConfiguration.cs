using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassConfiguration : MonoBehaviour
{
    public float MaxMassLimit = 3000;
    public LoadType loadType;
    public float Mass;
    public List<Transform> contactPts = new List<Transform>();

    public List<Rigidbody> connectionRigidBodies = new List<Rigidbody>();

    Rigidbody rb;

    public void Awake()
    {
        Mass = 0;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
    }

    public void UpdateMass(float Mass)
    {
        if( this.Mass+Mass > MaxMassLimit)
        {
            WallDisplay.Display("Max weight limit " + MaxMassLimit);
            return;
        }
        else if ( (this.Mass + Mass) < 1)
        {

            WallDisplay.Display("Weight is below 1 kg");
            return;
        }

        this.Mass += Mass;
        rb.mass = this.Mass;

        WallDisplay.DisplayWeight(this.Mass.ToString()+" kg");

        //float s = Mass / 200;
        //float y = 1;
        //if (s < 1)
        //    s = 1f;
        //else if (s > 50)
        //    s = 50f;

        //if (s > 1)
        //    y = y + (s / 3.0f);

        //if( y > 20 ) y = 20;

        //transform.localScale = new Vector3(s, y, s);
        //transform.position = new Vector3(transform.position.x, transform.position.y + 30, transform.position.z);
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

        foreach(var c in contacts)
        {
            var t = contactPts[c];

            var arb = t.gameObject.AddComponent<Rigidbody>();
            arb.useGravity = true;
            arb.mass = 0.3f;

            var fjt = t.gameObject.AddComponent<FixedJoint>();
            fjt.connectedBody = rb;
            fjt.anchor = Vector3.zero;
            fjt.axis = Vector3.up;

            connectionRigidBodies.Add(arb);
        }

        return connectionRigidBodies;
    }
}
