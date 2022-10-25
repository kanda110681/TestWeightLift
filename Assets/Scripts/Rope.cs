using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLink 
{
    public Rope rope;
    public Vector3 []points = new Vector3[2];
    public RopeLink prev, next;
    public bool bEndLink = false ;

    public GameObject go;
    public Rigidbody rb;
    public HingeJoint joint;
    public HingeJoint tailJoint;
    public float linkLength;

    public float spring = 2000f;
    public float damper = 1000f;
    public Material material;

    public bool bUseLineRenderer ;

    public RopeLink ()
    {
        prev = next = null;
    }

    public RopeLink(Vector3 p1, Vector3 p2, RopeLink prev=null, RopeLink next=null, bool bEndLink=false)
    {
        points[0] = p1; 
        points[1] = p2;
        this.prev = prev;
        this.next = next;
        if(prev != null)
            prev.next = this;
        this.bEndLink = bEndLink;
    }

    public void AttachPhysics()
    {
        Vector3 center = (points[0] + points[1]) / 2.0f;
        go = new GameObject();
        go.transform.position = center;
        go.transform.SetParent(rope.goLinks.transform);

        go.layer = LayerMask.NameToLayer("Rope");

        rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 10f;
        rb.drag = 1000f;
        rb.angularDrag = 100f;
        // rb.interpolation = RigidbodyInterpolation.Interpolate;

        GameObject marker = null;

       
        if (!bUseLineRenderer)
        {
            marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            marker.transform.SetParent(go.transform);
            marker.transform.localPosition = Vector3.zero;
            marker.transform.localScale = new Vector3(1, linkLength, 1);
            //marker.GetComponent<MeshRenderer>().material.color = Color.yellow;
            if (material != null)
                marker.GetComponent<MeshRenderer>().material = material;
            else
                marker.GetComponent<MeshRenderer>().material.color = Color.yellow;

            marker.GetComponent<CapsuleCollider>().height = linkLength * 0.8f;
        }
        else
        {
            //{ chain link colliders
            var bc = go.AddComponent<BoxCollider>();
            bc.center = Vector3.zero;
            bc.size = new Vector3(1, linkLength * 0.9f, 1);

            //var bc = go.AddComponent<CapsuleCollider>();
            //bc.center = Vector3.zero;
            //bc.radius = 0.2f;
            //bc.height = linkLength * 0.3f;
            //}
        }
        //}

        go.layer = LayerMask.NameToLayer("Rope");

        joint = go.AddComponent<HingeJoint>();

        if (prev == null && bEndLink) // start
        {
            joint.anchor = new Vector3(0, 0, 0);
        }
        else
        {
            joint.anchor = new Vector3(0, linkLength, 0);
            joint.connectedBody = prev.rb;

            joint.useSpring = true;
            joint.spring = new JointSpring { spring = spring, damper = damper };
        }

        // set end marker
        if(next==null && bEndLink)
        {
            if (bUseLineRenderer)
            {
                marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                marker.transform.SetParent(go.transform);
                marker.transform.localPosition = Vector3.zero;
                marker.transform.localScale = new Vector3(1, linkLength, 1);
            }
           
            marker.GetComponent<MeshRenderer>().material.color = Color.red;

            go.tag = "RopeTail";
            

            //{ attach weight lifting hinge joint  // Creates problem - flickering wihtou weight

            //tailJoint = go.AddComponent<HingeJoint>();
            //tailJoint.anchor = new Vector3(0, linkLength, 0);
            //tailJoint.useSpring = true;
            //tailJoint.spring = new JointSpring { spring = spring, damper = damper };
            //}
        }

        joint.axis = new Vector3(0, 1, 0);
    }

    public void AttachTailJoint()
    {
        if (next != null && bEndLink == false)
            return;

        tailJoint = go.AddComponent<HingeJoint>();
        tailJoint.anchor = new Vector3(0, linkLength, 0);
        tailJoint.useSpring = true;
        tailJoint.spring = new JointSpring { spring = spring, damper = damper };
    }

    public void UpdatePhysics()
    {
        float d = damper * 100;
        if (d <= 0.01f) d = 0.1f;
        if (d > 90000f)  d = 90000f;

        rb.drag = d;
        rb.angularDrag = 100;
        joint.spring = new JointSpring { spring = d, damper = d };

        // Debug.Log("Link vel: " + rb.velocity + " Angular: " + rb.angularVelocity);
    }

    //public void DisablePhysics()
    //{
    //   // rb.Sleep();
    //}

    //public void EnablePhysics()
    //{
    //   // go.SetActive(true);
    //  //  rb.WakeUp();
    //}
}

public class Rope : MonoBehaviour
{
    public float movementSpeed = 1f;
    public Transform topAnchorPt;
    public Rigidbody topAnchorPtRB;

    public float length;
    public int links;
    public RopeLink ropeHead = null;
    public RopeLink movementHead = null;
    public RopeLink ropeTailEnd = null;
    public float linkLength;

    LineRenderer lr;
    public float linkDispSize = 1.0f;
    public float spring = 2f;
    public float damper = 1f;

    public Material material;
    public bool bUseLineRenderer;

    public GameObject goLinks;
    public Vector3 ropeCreationDirection;

    public void Start()
    {
       
    }

    public void Init()
    {
        lr = GetComponent<LineRenderer>();
        lr.startWidth = lr.endWidth = linkDispSize;
        lr.positionCount = 0;

        goLinks = new GameObject("LINKS");
        goLinks.transform.SetParent(this.transform);

        topAnchorPt = this.transform;
        Generate(topAnchorPt, ropeCreationDirection, length, linkLength);

        AttachPhysics();
    }

    public RopeLink Generate(Transform topAnchorPt, Vector3 dir, float length, float linkLength=1.0f)
    {
        this.topAnchorPt = topAnchorPt;
        this.linkLength = linkLength;

        links = ((int)(length / linkLength));
        if (links <= 0)
            return null;

        this.length = links * linkLength;

        dir = dir.normalized * linkLength;

        RopeLink prevLink = null;
        ropeHead = null;

        Vector3 prev = topAnchorPt.position;
        Vector3 next = prev + dir;

        ropeHead = new RopeLink(prev, next);
        ropeHead.linkLength = linkLength;
        ropeHead.bEndLink = true;
        prev = next;
        prevLink = ropeHead;
        ropeHead.rope = this;


        for (int i = 1; i < links ; i++)
        {
            next = prev + dir;

            RopeLink newLink = new RopeLink(prev, next, prevLink);
            newLink.linkLength = linkLength;
            newLink.rope = this;

            prev = next;
            prevLink = newLink;
        }

        if (prevLink != null)
        {
            prevLink.bEndLink = true;
            ropeTailEnd = prevLink;
        }

        movementHead = ropeHead;
        return ropeHead;
    }

    public void AttachPhysics()
    {
        int i = 1;
        RopeLink lnk = ropeHead;
        while(lnk != null)
        {
            lnk.bUseLineRenderer = bUseLineRenderer;
            lnk.material = material;
            lnk.AttachPhysics();

            lnk.go.name = i.ToString();
            i++;
            lnk = lnk.next;
        }

        ropeHead.joint.connectedBody = topAnchorPtRB;
    }

    //public void FixedUpdate()
    //{
    //    float maxVelocity = 0.5f;

       
    //    RopeLink lnk = ropeHead;
    //    while (lnk != null)
    //    {
    //        if( lnk.rb.velocity.magnitude > maxVelocity )
    //        {
    //            lnk.rb.drag = 99999;
    //            //lnk.rb.velocity = Vector3.zero;
    //            lnk.rb.AddForce(-lnk.rb.velocity);
    //            //lnk.rb.Sleep();
    //            // lnk.rb.velocity = Vector3.ClampMagnitude(lnk.rb.velocity, maxVelocity);

    //            //Debug.Log("Rope Top Anchor vel: " + lnk.rb.velocity);
    //        }

    //        //if( lnk.rb.angularVelocity.magnitude > 0.2f )
    //        //{
    //        //    lnk.rb.angularVelocity = Vector3.zero;
    //        //   // lnk.rb.angularVelocity = Vector3.ClampMagnitude(lnk.rb.angularVelocity, 0.2f);
    //        //}


    //        lnk = lnk.next;
    //    }

    //}

    public void Update()
    {
        if (bUseLineRenderer)
        {
            lr.positionCount = 0;
            RopeLink lnk = movementHead;

            while (lnk != null)
            {
                if (lnk.rb == null)
                    break;

                lnk.spring = spring;
                lnk.damper = damper;
                lnk.UpdatePhysics();

                var pos = lnk.go.transform.position;
                var dir = lnk.go.transform.up * (linkLength / 2.0f);
                var pos1 = pos + dir;
                var pos2 = pos - dir;

                lr.SetPosition(lr.positionCount++, pos1);
                lr.SetPosition(lr.positionCount++, pos2);

                lnk = lnk.next;
            }
        }
    }


    // Not used and not working - can not  snap the chain using spirng joint
#if false
    public void ResetRope()
    {
        movementHead = ropeHead;
    }

    public void LiftUp()
    {
       // Time.timeScale = 0f;
        if (movementHead.next == null)
            return;

        UpdateMovementHeadPhysics();
        movementHead = movementHead.next;
        SetRopeAtAnchorPosition();
        // Time.timeScale = 1f;

        //test.gameObject.transform.position += Vector3.up;
    }

    public void LiftDown()
    {
        if (movementHead.prev == ropeHead)
            return;

        UpdateMovementHeadPhysics();
        movementHead = movementHead.prev;
        SetRopeAtAnchorPosition();

        //test.gameObject.transform.position -= Vector3.up;
    }

    public void UpdateMovementHeadPhysics()
    {
        if (movementHead == null || movementHead.joint == null)
            return;

        movementHead.joint.connectedBody = null;
        movementHead.DisablePhysics();
       
    }

    public void SetRopeAtAnchorPosition()
    {
        if (movementHead == null)
            return;

       // movementHead.DisablePhysics();
        //movementHead.go.transform.position = topAnchorPtRB.position;
        movementHead.joint.connectedBody = topAnchorPtRB;
        movementHead.EnablePhysics();
        //StartCoroutine( LinkTransition());
    }

    IEnumerator LinkTransition()
    {
        Vector3 target = topAnchorPt.position;

        while(true)
        {
            yield return null;
            Vector3 npos = target + topAnchorPt.up ;

            topAnchorPtRB.Sleep();
            topAnchorPt.position = new Vector3(npos.x, npos.y, npos.z);
            topAnchorPtRB.WakeUp();

            double d = Vector3.Distance(target, topAnchorPt.position);
            Debug.Log("" + d);
            if (d >= 0.9f)
                break;
        }
        


        /*
        RopeLink tmpLink = movementHead;
        SpringJoint sj = tmpLink.go.AddComponent<SpringJoint>();

        sj.spring = 1000;
        sj.damper = 10f;
        sj.connectedBody = topAnchorPtRB;
        sj.autoConfigureConnectedAnchor = false;
        sj.anchor = new Vector3(0, 0, 0);
        sj.connectedAnchor = new Vector3(0, 1f, 0);

        while(true)
        {
            yield return new WaitForEndOfFrame();

            yield return new WaitForEndOfFrame();

            //double d = Vector3.Distance(tmpLink.go.transform.position, topAnchorPt.position);
            //if (d > 0.2f)
            //    continue;
            break;
        }

        Destroy(sj);
        sj = null;
        */
        //Debug.Log(" Link Transition Ended");
    }
#endif 
}
