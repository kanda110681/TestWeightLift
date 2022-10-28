using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLink 
{
    public Vector3 []points = new Vector3[2];
    public GameObject go;
    public Rigidbody rb;
    public HingeJoint joint;

    public RopeLink(Vector3 p1, Vector3 p2)
    {
        points[0] = p1; 
        points[1] = p2;

        CreateGameObjects();
    }

    void CreateGameObjects()
    {
        Vector3 center = (points[0] + points[1]) / 2.0f;
        go = new GameObject();
        go.transform.position = center;

        go.layer = LayerMask.NameToLayer("Rope");

        rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 0.1f;
        rb.drag = 0.2f;
        rb.angularDrag = 1f;

        var bc = go.AddComponent<BoxCollider>();
        bc.center = Vector3.zero;
        float size = Vector3.Distance(points[0], points[1]);
        bc.size = new Vector3(1, size * 0.9f, 1);
    }
}

public class Rope : MonoBehaviour
{
    public Rigidbody topAnchorPtRB;
    public Rigidbody bottomAnchorPtRB; // weigth joint pos
 
    public Vector3 ropeCreationDirection = Vector3.down;
    public float length;
    public float linkLength;
    public RopeLink ropeHead = null;
    public RopeLink ropeTailEnd = null;
  //  public HingeJoint ropeTailJoint;

    LineRenderer lr;
    public float linkDispSize = 1.0f;
    public float spring = 0.2f;
    public float damper = 0.2f;

    public float currentDamper = 1f;
    Coroutine crDamper;

    public Material material;
    public bool bUseLineRenderer;

    public GameObject goLinks;
    List<RopeLink> links = new List<RopeLink>();
    public int nLinks;
    public LayerMask layer;

    public bool bValid = false;

   

    public void Init()
    {
        //debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //debugSphere.GetComponent<Renderer>().material.color = Color.cyan;
        //Destroy(debugSphere.GetComponent<SphereCollider>());
        //debugSphere.transform.localScale = Vector3.one * 3f;
        //debugSphere.SetActive(false);

        var lm = LayerMask.NameToLayer("Rope");

        lr = GetComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = linkDispSize;
        lr.positionCount = 0;

        goLinks = new GameObject("LINKS");
        goLinks.layer = lm;
        goLinks.transform.SetParent(this.transform);

        if ( bottomAnchorPtRB != null )
        {
            length = Vector3.Distance(topAnchorPtRB.transform.position, bottomAnchorPtRB.transform.position) ;
        }
        
        nLinks = ((int)(length / linkLength)) + 1;

        Generate();

        AttachJoints();

        bValid = true;
    }

    private void OnDisable2()
    {
       // Debug.Log("Rope Destroyed");
        DetachHeadJoint();
        DetachTailJoint();
    }

    public void Generate()
    {
        if (nLinks <= 0)
            return ;

        Vector3 dir = ropeCreationDirection;
        dir = dir.normalized * linkLength;

        Vector3 prev = topAnchorPtRB.transform.position;
        Vector3 next;

        for (int i = 0; i < nLinks; i++)
        {
            next = prev + dir;

            RopeLink newLink = new RopeLink(prev, next);
            links.Add(newLink);
            newLink.go.name = i.ToString();
            newLink.go.transform.SetParent(goLinks.transform);

            prev = next;
        }

        ropeHead = links[0];
        ropeTailEnd = links[nLinks-1];
    }

    void CreateHingeJoint(RopeLink lnk, Rigidbody anchorRB)
    {
        HingeJoint joint = lnk.go.AddComponent<HingeJoint>();
        lnk.joint = joint;
        joint.connectedBody = anchorRB;

        joint.useSpring = true;
        joint.spring = new JointSpring { spring = this.spring, damper = this.damper };
    }

    public void AttachJoints()
    {
        CreateHingeJoint(ropeHead, topAnchorPtRB);

        for (int i = 1; i < nLinks; i++)
        {
            CreateHingeJoint(links[i], links[i-1].rb);            
        }

        if( bottomAnchorPtRB != null )
        {
            var fjt = ropeTailEnd.go.AddComponent<FixedJoint>();
            fjt.connectedBody = bottomAnchorPtRB;
        }
    }

    public void DetachTailJoint()
    {
        var fjt = ropeTailEnd.go.AddComponent<FixedJoint>();
        fjt.connectedBody = null;

        Destroy(fjt);
    }

    public void DetachHeadJoint()
    {
        if (topAnchorPtRB == null)
            return;
        var fjt = topAnchorPtRB.gameObject.AddComponent<FixedJoint>();
        fjt.connectedBody = null;
        Destroy(fjt);
    }

    public void WeightDistribute(float linkWeight)
    {
        if (!bValid)
            return;

        if( crDamper == null)
            crDamper = StartCoroutine(ActivateDamper());

        //float tlw = linkWeight * nLinks;

        foreach (var lnk in links)
            lnk.rb.mass = linkWeight;

    }

    private void FixedUpdate()
    {
        if (!bValid)
            return;

        //float MaxVelocityChange = 5f;
        //bool DampRopes = false;
        //foreach (var lnk in links)
        //{
        //    if( lnk.rb.velocity.magnitude > MaxVelocityChange )
        //    {
        //        DampRopes = true;
        //        break;
        //    }
        //}

        //float d = 1;
        //if (DampRopes)
        //    d = 10000;

        float s = spring;
        if (currentDamper > 100f)
            s = 76543;

        foreach (var lnk in links)
        {
            lnk.rb.ResetCenterOfMass();

            lnk.joint.spring = new JointSpring { spring = s, damper = currentDamper };
            lnk.rb.drag = currentDamper;
        }
    }


    public bool bDrawToMainLift = false;

    public void Update()
    {
        if (!bValid || lr == null)
            return;

        lr.positionCount = 0;

        if (bDrawToMainLift)
        {
            Vector3 tpos = MainLiftController.Instance.GetTailEndPos();
           
           // Debug.DrawLine(topAnchorPtRB.position, tpos, Color.red);

            lr.SetPosition(lr.positionCount++, tpos);
        }

        lr.SetPosition(lr.positionCount++, topAnchorPtRB.position);

        foreach (var lnk in links)
        {
            lr.SetPosition(lr.positionCount++, lnk.rb.position);
        }

        Vector3 pos = ropeTailEnd.rb.position;
        if (bottomAnchorPtRB != null)
        {
            pos = bottomAnchorPtRB.position;

           // debugSphere.transform.position = pos;
           // debugSphere.SetActive(true);
        }
        else
            pos = ropeTailEnd.rb.position + (Vector3.down * (linkLength * 0.6f));
        lr.SetPosition(lr.positionCount++, pos);
    }

    IEnumerator ActivateDamper()
    {
        currentDamper = 10000;
        yield return new WaitForSeconds(1f);
        currentDamper = damper;
        
       // Debug.Log("Damper changed to : " + damper);

        StopCoroutine(crDamper); crDamper = null;
    }

}
