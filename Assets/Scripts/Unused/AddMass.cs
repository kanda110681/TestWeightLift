using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMass : MonoBehaviour
{
    public LiftSettings ls;
    public Rigidbody connectedRB;
    public float scale = 1f;

    public Transform topJoint;
    public Transform bottomJoint;

    LineRenderer lr;
    public float chainDispSize = 0.3f;

    public float spawnRange = 3f;

    bool bInit = false;
    public bool bWeighCenterAttachment = false;

    public void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = chainDispSize;
        lr.positionCount = 0;
    }

    public void Init()
    {
        PrimitiveType type = PrimitiveType.Cube;
        if (ls.LType == LoadType.LT_CUBE)
            type = PrimitiveType.Cube;
        else if (ls.LType == LoadType.LT_CYLINDER)
            type = PrimitiveType.Cylinder;
       // else if (ls.LType == LoadType.LT_SPHERE)
        //    type = PrimitiveType.Sphere;

        var go = GameObject.CreatePrimitive(type);
        go.transform.SetParent(transform, false);

        Vector3 pos = topJoint.position + Vector3.down * (0.5f);
        pos.x = Random.Range(-spawnRange, spawnRange);
        pos.z = Random.Range(-spawnRange, spawnRange);

        if (pos.y < 2)
            pos.y = 2;

        go.transform.position = pos;

        bottomJoint = go.transform;


        go.transform.localScale = new Vector3(scale, scale, scale);

        var arb = go.AddComponent<Rigidbody>();
        arb.useGravity = true;
        arb.mass = ls.Weight;
        arb.Sleep();

        go.AddComponent<CollisionHandler>();

        var Parent = connectedRB.gameObject;

        if (bWeighCenterAttachment) // Fixed Joint
        {
            var fjt = Parent.AddComponent<FixedJoint>();
            fjt.connectedBody = arb;
            fjt.anchor = Vector3.zero;
            fjt.axis = Vector3.up;
        }
        else
        {
            //{ HingeJoint

            var ahjt = Parent.AddComponent<HingeJoint>();
            ahjt.connectedBody = arb;
            ahjt.anchor = Vector3.zero;
            ahjt.axis = Vector3.down;

            ahjt.useLimits = true;
            var jl = new JointLimits();
            jl.max = 90.0f;
            jl.min = -90.0f;
            ahjt.limits = jl;
            //}

        }

        bInit = true;
    }

    private void Update()
    {
        if(bInit)
            UpdateChainData();
    }

    void UpdateChainData()
    {
        lr.positionCount = 2;
        lr.SetPosition(0, topJoint.position);
        lr.SetPosition(1, bottomJoint.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.gameObject.tag);
    }
}
