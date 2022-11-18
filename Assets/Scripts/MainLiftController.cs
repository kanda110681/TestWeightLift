using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainLiftController : MonoBehaviour
{
    public Material ropeMaterial;
    public Transform topAnchorPt;     // main rope top anchor pt
    public Transform bottomAnchorPt;  // main rope bottom pt - all ropes attached here
    public LiftSettings ls; 

    public float speed = 1f;  // lift up and down speed control 

    // Up limit - main rope tail end position can not be greater than this
    public float TopHeightDistLimit = 68.0f;

    // Down limit - main rope tail end position can not be less than this
    public float BottomHeightDistLimit = 20f;

    // Ropes configuration (1 or 2 or 3 ... )
    LiftSelector liftSelector;
  

    public static MainLiftController Instance;

    GameObject mainLiftGO;
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
        bReady = false;
    }

    private void OnEnable()
    {
        ls.cbUp += LiftUP;
        ls.cbDown += LiftDown;

        if (liftSelector != null)
        {
            liftSelector.cbNewLiftConfigured += NewLiftConfigured;
            liftSelector.cbResetRequired += LiftReset;
        }
    }

    private void OnDisable()
    {
        ls.cbUp -= LiftUP;
        ls.cbDown -= LiftDown;

        if (liftSelector != null)
        {
            liftSelector.cbNewLiftConfigured -= NewLiftConfigured;
            liftSelector.cbResetRequired -= LiftReset;
        }
    }

    
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

        /*
         *  always check current lift controller obj before using it. Because it created dynamically. 
         */
        var lc = liftSelector.GetCurrentLiftConotroller();
        if (lc == null)
            return;

        /*
         *  Set Joint's spring and damper values from UI. 
         */
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
        go.name = "Rope";
        go.layer = lm;
        go.transform.SetParent(transform);
        var lr = go.AddComponent<LineRenderer>();
        lr.material = ropeMaterial;

        var rrb = go.AddComponent<Rigidbody>();
        rrb.isKinematic = true;
        var rope = go.AddComponent<Rope>();
        rope.topAnchorPtRB = rrb;

        return go;
    }

    void SetupMainLift()
    {
        bReady = false;

        Rigidbody mainRB, topRB;
        // Game objects creations - main lift and child object
        mainLiftGO = CreateGO("Main Lift", transform, out mainRB);  
        var topGO = CreateGO("TopPt", mainLiftGO.transform, out topRB);

        topGO.transform.position = topAnchorPt.position;
        // child obj attached to parent 
        var topFJ = CreateFixedJoint(mainLiftGO, topRB); 

        // Main rope
        var ropeGO = CreateRope(); 
        ropeGO.transform.SetParent(topGO.transform);

        mainRope = ropeGO.GetComponent<Rope>();
        // main rope attached to top pt 
        mainRope.topAnchorPtRB = topRB;
        
        mainRope.ropeCreationDirection = Vector3.down;
        mainRope.length = Vector3.Distance(topAnchorPt.position, bottomAnchorPt.position) ;

        // Its main rope - distance is too high, so used big chain length
        mainRope.linkLength = 5f; 
        mainRope.Init();  


        /*
         *   Marker: Centre of all ropes joints
         *   Big sphere to hide joints - for aesthetic purposeo only
         */
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

        // Main rope links updated with big weight to hold all ropes and load types
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
    
    /*
     *  New main lift created and destroyed old one 
     *  called if any changes in ropes or load type configuration
     *  Dependency: None (it can be called anytime)
     */
    void LiftReset()
    {
        bReady = false;

        DestroyImmediate(mainLiftGO);
        mainLiftGO = null;

        SetupMainLift();
    }

    /*
     *  Dependecy: It depends on the bottom ropes (if ropes changed and we need to reattached)
     *  All ropes head attached to main ropes tail
     *  Also make sure all ropes are NON-KINEMATIC and uses Gravity
     */
    void NewLiftConfigured()
    {
        bReady = false;       

        var lc = liftSelector.GetCurrentLiftConotroller();
        if (lc == null)
            return;

        // attach all ropes head to main rope tail
        foreach(var rope in lc.ropes)
        {
            var fjt = rope.topAnchorPtRB.gameObject.AddComponent<FixedJoint>();
            fjt.connectedBody = mainRope.ropeTailEnd.rb;
        }


        // make it Non-Kinematic ropes - so that we can move 
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

        mainLiftGO.transform.position += (Vector3.up * speed * Time.deltaTime);
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
                
        mainLiftGO.transform.position -= (Vector3.up * speed * Time.deltaTime);
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
