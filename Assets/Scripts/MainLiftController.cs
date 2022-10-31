using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLiftController : MonoBehaviour
{
    public Material ropeMaterial;
    public Transform topAnchorPt;
    public Transform bottomAnchorPt;
    public LiftSettings ls;

    public float speed = 1f;
    public float TopHeightDistLimit = 68.0f;
    public float BottomHeightDistLimit = 20f;

    LiftSelector liftSelector;

    GameObject mainLiftGO;
    Vector3 mainLiftResetPos;

    public static MainLiftController Instance;

    Rope mainRope;
    bool bReady = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        liftSelector = GetComponent<LiftSelector>();
        ls.cbUp += LiftUP;
        ls.cbDown += LiftDown;
        bReady = false;

        liftSelector.cbNewLiftConfigured = NewLiftConfigured;
        liftSelector.cbResetRequired = LiftReset;
    }

    private void OnDisable()
    {
        ls.cbUp -= LiftUP;
        ls.cbDown -= LiftDown;

        if(liftSelector != null)
             liftSelector.cbResetRequired -= LiftReset;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupMainLift();
    }

    // Update is called once per frame
    void Update()
    {
        //{test
        //if (Input.GetKey(KeyCode.UpArrow))
        //    LiftUP();
        //else if (Input.GetKey(KeyCode.DownArrow))
        //    LiftDown();
        //}test

        var lc = liftSelector.GetCurrentLiftConotroller();
        if (lc == null)
            return;

        float spring = ls.SlingElasticity;
        float damper = ls.Damper;
        foreach (var rope in lc.ropes)
        {
            rope.spring = spring;
            rope.damper = damper;
        }
    }

    GameObject CreateGO(string name, Transform parent, out Rigidbody rb)
    {
        GameObject go = new GameObject();
        go.name = name;
        go.transform.parent = parent;

        rb = go.AddComponent<Rigidbody>();
        rb.mass = 200;
        rb.isKinematic = true;
        return go;
    }

    FixedJoint CreateFixedJoint(GameObject src, Rigidbody connectRB)
    {
        FixedJoint joint = src.AddComponent<FixedJoint>();
        joint.connectedBody = connectRB;
        return joint;
    }


    public GameObject CreateRope()
    {
        var lm = LayerMask.NameToLayer("Rope");
        var go = new GameObject();
        go.layer = lm;
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = ropeMaterial;

        var rrb = go.AddComponent<Rigidbody>();
        rrb.isKinematic = true;
        var rope = go.AddComponent<Rope>();
        rope.topAnchorPtRB = rrb;
        rope.bUseLineRenderer = true;

        return go;
    }

    void SetupMainLift()
    {
        bReady = false;

        Rigidbody mainRB, topRB;
        mainLiftGO = CreateGO("Main Lift", transform, out mainRB);
        var topGO = CreateGO("TopPt", mainLiftGO.transform, out topRB);

        mainLiftResetPos = mainLiftGO.transform.position;

        topGO.transform.position = topAnchorPt.position;

        var topFJ = CreateFixedJoint(mainLiftGO, topRB);

        //var ropeGO = Instantiate(ROPE, topAnchorPt);
        var ropeGO = CreateRope();
        ropeGO.transform.SetParent(topGO.transform);

        mainRope = ropeGO.GetComponent<Rope>();
        mainRope.topAnchorPtRB = topRB;
        
        mainRope.ropeCreationDirection = Vector3.down;
        mainRope.length = Vector3.Distance(topAnchorPt.position, bottomAnchorPt.position) ;
        mainRope.linkLength = 5f;
        mainRope.Init();

        float rad = 10;
        var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.layer = LayerMask.NameToLayer("Safety");

        Destroy(marker.GetComponent<SphereCollider>());
        var bc = marker.AddComponent<BoxCollider>();
        bc.size = new Vector3(7, 1.5f, 5);
        
        marker.transform.SetParent(mainRope.ropeTailEnd.go.transform);
        marker.transform.position = Vector3.zero;
        marker.transform.rotation = Quaternion.identity;
        marker.transform.localPosition = Vector3.zero;
       
        marker.transform.localScale = new Vector3(rad, rad, rad);
        marker.GetComponent<Renderer>().material.color = Color.black;
        //

        UpdateWeightForLinks(200);
    }

    public Vector3 GetTailEndPos()
    {
        return mainRope.ropeTailEnd.go.transform.position;
    }

    void UpdateWeightForLinks(float w)
    {
        if (mainRope == null)
            return;

        mainRope.WeightDistribute(w);
    }

    

    void LiftReset()
    {
        bReady = false;

        DestroyImmediate(mainLiftGO);
        mainLiftGO = null;

        SetupMainLift();
    }

    void NewLiftConfigured()
    {
        bReady = false;       

        var lc = liftSelector.GetCurrentLiftConotroller();
        if (lc == null)
            return;

        foreach(var rope in lc.ropes)
        {
            var fjt = rope.topAnchorPtRB.gameObject.AddComponent<FixedJoint>();
            fjt.connectedBody = mainRope.ropeTailEnd.rb;
        }

        foreach (var rope in lc.ropes)
        {
            var rb = rope.gameObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.mass = 1;
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        bReady = true;
    }

    public void LiftUP()
    {
        if (!bReady) return;

        float h = (TopHeightDistLimit - mainRope.ropeTailEnd.go.transform.position.y);

        if (h < 0.0)
        {
            WallDisplay.Display("UP Limit reached");
            //Debug.Log("Up Limit reached");
            return;
        }

        mainLiftGO.transform.position += (Vector3.up * speed);
       // UpdateRopeLinkPhysics(1);
    }

    public void LiftDown()
    {
        if (!bReady) return;

        float h = (BottomHeightDistLimit - mainRope.ropeTailEnd.go.transform.position.y);
        if (h > 0.0)
        {
            WallDisplay.Display("Down Limit reached");
            //Debug.Log("Down Limit reached");
            return;
        }
                
        mainLiftGO.transform.position -= (Vector3.up * speed);
       // UpdateRopeLinkPhysics(-1);
    }

    void UpdateRopeLinkPhysics(int move)
    {
        var lc = liftSelector.GetCurrentLiftConotroller();
        if (lc == null)
            return;

        float totalWeight = WeightManager.Instance.CurrentWeight();
        if (totalWeight <= 1.0)
            totalWeight = 1.0f;

        float wr = totalWeight / 3000f;

        float spring = ls.SlingElasticity;
        float damper = ls.Damper;

        // Debug.Log($"Weight-0: {totalWeight} Ratio: {wr} Spring: {spring} Damper: {damper}");
#if false
        //if (move < 0)
        //{
        //    damper += 0.1f;
        //}
        // else
        if (damper < 1)
        {
            damper += (wr * 1000);

            if (totalWeight < 10)
            {
                //  spring = 0.1f;
            }
            else if (totalWeight < 100)
            {
                spring = totalWeight * 0.07f;
            }
            else if (totalWeight < 200)
            {
                spring = totalWeight * 0.05f;
            }
            else if (totalWeight < 500)
            {
                spring = totalWeight * 0.01f;
            }
            else if (totalWeight < 1000)
            {
                spring = totalWeight * 0.05f;
            }
            else if (totalWeight < 1500)
            {
                spring = totalWeight * 0.07f;
            }
            else if (totalWeight < 2000)
            {
                spring = totalWeight * 0.1f;
            }
            else
                spring = totalWeight * 0.12f;
        }
        else if (damper > 1)
        {
            damper = damper * 5555;
        }

        spring = Mathf.Clamp(spring, 0.1f, 55555);
        damper = Mathf.Clamp(damper, 0.1f, 77777);

     //   Debug.Log($"Weight: {totalWeight} Ratio: {wr} Spring: {spring} Damper: {damper}");
#endif 
        foreach (var rope in lc.ropes)
        {
            rope.spring = spring;
            rope.damper = damper;
        }
    }
}
