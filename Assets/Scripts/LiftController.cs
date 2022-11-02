using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : MonoBehaviour
{
    public float speed = 1f;
    public LiftSettings ls;
    public List<Rope> ropes = new List<Rope>();
    public WeightManager wm;

    public Transform AnchorPtTransform;
   
    public float TopHeightDistLimit = 68.0f;
    public float BottomHeightDistLimit = 41f;

    public Material ropeMaterial;
    public bool bRopesReady = false;

    float ropeLength;
    float ropeLinkLength;

    public void Awake()
    {
        
    }

    public void Init()
    {
        //  ls.cbDown = LiftDown;
        //  ls.cbUp = LiftUP;
        ls.cbAddMass += AddMass;
    }

    public void OnDisable()
    {
        ls.cbAddMass -= AddMass;
        SetInvalidLift();

        for (int i = 0; i < ropes.Count; i++)
            ropes[i] = null;
        ropes.Clear();
    }

    public void OnDestroy()
    {
       
    }

    public void SetInvalidLift()
    {
        foreach (Rope rope in ropes)
            rope.bValid = false;
    }

    public void AddRope(int rno)
    {
        var lm = LayerMask.NameToLayer("Rope");
        var go = new GameObject();
        go.name = "Rope-" + rno.ToString();
        go.layer = lm;
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = ropeMaterial;

        var rrb = go.AddComponent<Rigidbody>();
        rrb.isKinematic = true;
        var rope = go.AddComponent<Rope>();
        rope.topAnchorPtRB = rrb;
        rope.bUseLineRenderer = true;

        ropes.Add(rope);
    }

    public void InitRope(Rope rope, Vector3 direction)
    {
        rope.length = ropeLength;
        rope.linkLength = ropeLinkLength;
        rope.ropeCreationDirection = direction;
        rope.Init();
    }

    public void SetupRopes(int nRopes, float ropeLength, float ropeLinkLength, WeightManager wm)
    {
        if (nRopes <= 0 || wm == null)
            return;

        ropes.Clear();

        for (int i = 0; i < nRopes; i++)
            AddRope(i+1);

        this.ropeLength = ropeLength;
        this.ropeLinkLength = ropeLinkLength;

        var WeightRBs = wm.PrepareWeightConfigurationConnections(nRopes);
        for (int i = 0; i < nRopes; i++)
        {
            ropes[i].gameObject.transform.position = AnchorPtTransform.position; 

            ropes[i].bottomAnchorPtRB = WeightRBs[i];
            Vector3 dir = (WeightRBs[i].position - AnchorPtTransform.position);
            //if ( dir.magnitude > 100 )
            //{
            //   // Debug.Log("Problem in distance : rope: " + i + " dist: " + dir.magnitude + " WeigtPos: " + WeightRBs[i].position);
            //    continue;
            //}
            InitRope(ropes[i], dir.normalized);
        }

       
        bRopesReady = true;
    }

#if false
    public void LiftUP()
    {
        if (!bRopesReady) return;

        float h = (TopHeightDistLimit - ropes[0].ropeTailEnd.go.transform.position.y);

        if( h < 0.0 )
        {
            WallDisplay.Display("UP Limit reached");
            //Debug.Log("Up Limit reached");
            return;
        }

        foreach (Rope rope in ropes)
        {
            rope.gameObject.transform.position += (Vector3.up*speed);
        }
    }

    public void LiftDown()
    {
        if (!bRopesReady) return;

        float h = (BottomHeightDistLimit - ropes[0].ropeTailEnd.go.transform.position.y);
        if( h > 0.0 )
        {
            WallDisplay.Display("Down Limit reached");
            //Debug.Log("Down Limit reached");
            return;
        }

        foreach (Rope rope in ropes)
        {
            rope.gameObject.transform.position -= (Vector3.up * speed);
        }
    }
#endif  
  
    void AddMass()
    {
        if (ropes.Count <= 0)
            return;

        float mass = wm.CurrentWeight() + ls.Weight;
        float ropeWD = mass / (float)ropes.Count;
        if (ropeWD <= ropes.Count) ropeWD = ropes.Count;

        foreach (Rope rope in ropes)
        {
            float lnkWD = ropeWD / (float)rope.nLinks;
            lnkWD = lnkWD * 1.1f;
            if (lnkWD <= 0.05) lnkWD = 0.05f;

            rope.WeightDistribute(lnkWD);

           
           // Debug.Log("Weight Dist: " + mass + " RopeWD: " + ropeWD + " LinkWD: " + lnkWD);
        }
    }

    //IEnumerator IncreaseStiffness()
    //{
    //    foreach (Rope rope in ropes)
    //    {
    //        rope.spring = 50000f;
    //        rope.damper = 50000f;
    //    }

    //    yield return new WaitForSeconds(1f);

    //    foreach (Rope rope in ropes)
    //    {
    //        rope.spring = ls.ElasticityScaleFactor;
    //        rope.damper = ls.Damper;
    //    }
    //}

    //public void Update()
    //{
    //    if (ropes.Count <= 0 || !bRopesReady )
    //        return;


    //    //if( ls.Damper > 1 )
    //    {
    //        //foreach (Rope rope in ropes)
    //        //{
    //        //    rope.damper = ls.Damper;
    //        //    rope.spring = ls.SlingElasticity;
    //        //}
    //    }

    //}
}
