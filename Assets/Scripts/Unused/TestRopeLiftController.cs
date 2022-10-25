using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRopeLiftController : MonoBehaviour
{
    public Rope rope;
    public LiftSettings ls;
    public float MovementSpeed = 1;

    public Transform AnchorPtTransform;
    public float AnchorAwayDist=0;
    public float HeightDist = 0;


    public float AnchorAwayDistUPLimit = -65f;
    public float AnchorAwayDistDownLimit = -5;

    private void Awake()
    {
        rope = GetComponent<Rope>();

        ls.cbDown = LiftDown;
        ls.cbUp = LiftUP;
        ls.cbAddMass = AddMass;
    }

    public void Update()
    {
        AnchorAwayDist = AnchorPtTransform.transform.position.y - transform.position.y;
        HeightDist = AnchorPtTransform.transform.position.y - rope.ropeTailEnd.go.transform.position.y;

    }

    public void LiftUP()
    {
        if(AnchorAwayDist < AnchorAwayDistUPLimit)
        {
            Debug.Log("Up Limit reached");
            return;
        }

       // Debug.Log("Up");
        transform.position = transform.position + Vector3.up * MovementSpeed;

        // rope.LiftUp();
    }

    public void LiftDown()
    {
        if (AnchorAwayDist > AnchorAwayDistDownLimit)
        {
            Debug.Log("Down Limit reached");
            return;
        }

        transform.position = transform.position + Vector3.down * MovementSpeed;

        //rope.LiftDown();
    }


    public void AddMass()
    {
        
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = "Add MASS: " + ls.Weight;
        go.transform.SetParent(rope.ropeTailEnd.go.transform, false);

        Vector3 pos = rope.ropeTailEnd.go.transform.position + Vector3.up * (0.5f);
       // pos.x = Random.Range(-spawnRange, spawnRange);
      //  pos.z = Random.Range(-spawnRange, spawnRange);

       // if (pos.y < 2)
        //    pos.y = 2;

        go.transform.position = pos;

        float scale = 5f;
        go.transform.localScale = new Vector3(scale, scale, scale);

        var arb = go.AddComponent<Rigidbody>();
        arb.useGravity = true;
        arb.mass = ls.Weight;
        arb.Sleep();

        go.AddComponent<CollisionHandler>();

        var Parent = rope.ropeTailEnd.go;

       // if (bWeighCenterAttachment) // Fixed Joint
        {
            var fjt = Parent.AddComponent<FixedJoint>();
            fjt.connectedBody = arb;
            fjt.anchor = Vector3.zero;
            fjt.axis = Vector3.up;
        }

        //if (nWeighAddedTimes > ls.MAX_TIMES_WEIGH_ADD)
        //    return;

        //var go = Instantiate(ls.prefabAddMass);
        //weights.Add(go);
        //var am = go.GetComponent<AddMass>();
        //if (weightAttachRB != null)  // Fixed Joint
        //{
        //    am.bWeighCenterAttachment = true;
        //    am.connectedRB = weightAttachRB;
        //    am.topJoint = weightAttachRB.transform;
        //}
        //else
        //{
        //    am.bWeighCenterAttachment = false;
        //    am.connectedRB = SlingControllers[0].rb;
        //    am.topJoint = SlingControllers[0].bottomJoint;
        //}

        //am.Init();
        //nWeighAddedTimes++;
    }

    public void OnDisable()
    {
        ClearWeights();
    }

    public void ClearWeights()
    {
       
    }

    public void LiftControllerReset()
    {
        
    }
}
