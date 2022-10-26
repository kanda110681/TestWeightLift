using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftController : MonoBehaviour
{
    public float speed = 1f;
    public LiftSettings ls;
    public List<Rope> ropes = new List<Rope>();

    public Transform AnchorPtTransform;
    public float AnchorAwayDist = 0;
    public float HeightDist = 0;

    public float TopHeightDist = 68.0f;
    public float TopHeightLimitThreshold = 5f;
    public float BottomHeightLimitThreshold = 41f;

    public float AnchorAwayDistUPLimit = -75f;
    public float AnchorAwayDistDownLimit = -10;

    public bool bRopesReady = false;

    float ropeLength;
    float ropeLinkLength;

    public void Awake()
    {
        ls.cbDown = LiftDown;
        ls.cbUp = LiftUP;
    }

    public void AddRope(Vector3 direction)
    {
        var rope = this.gameObject.AddComponent<Rope>();
        InitRope(rope, direction);
        ropes.Add(rope);
    }

    public void InitRope(Rope rope, Vector3 direction)
    {
        rope.length = ropeLength;
        rope.linkLength = ropeLinkLength;
        rope.ropeCreationDirection = direction;
        rope.Init();
    }

    public void SetupRopes(int nRopes, float ropeLength, float ropeLinkLength)
    {
        if (nRopes <= 0)
            return;

        this.ropeLength = ropeLength;
        this.ropeLinkLength = ropeLinkLength;
        float diviation = 0.1f;
       
        if ( nRopes == 1 )
        {
            InitRope(ropes[0], Vector3.down);
        }
        else if( nRopes == 2)
        {
            InitRope(ropes[0], new Vector3(-diviation, -0.5f, 0f));
            InitRope(ropes[1], new Vector3(diviation, -0.5f, 0f));
        }
        else if (nRopes == 3)
        {
            InitRope(ropes[0], new Vector3(-diviation, -0.5f, 0f));
            InitRope(ropes[1], new Vector3(diviation, -0.5f, 0f));
            InitRope(ropes[2], new Vector3(diviation, -0.5f, diviation));
        }
        else if (nRopes == 4)
        {
            InitRope(ropes[0], new Vector3(-diviation, -0.5f, -diviation));
            InitRope(ropes[1], new Vector3(-diviation, -0.5f, diviation));

            InitRope(ropes[2], new Vector3(diviation, -0.5f, -diviation));
            InitRope(ropes[3], new Vector3(diviation, -0.5f, diviation));
        }

        bRopesReady = true;
    }

    public void LiftUP()
    {
        if (!bRopesReady) return;

        float h = (TopHeightDist - ropes[0].ropeTailEnd.go.transform.position.y);

        //if (AnchorAwayDist < AnchorAwayDistUPLimit)
        if( h < TopHeightLimitThreshold )
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

        float h = (TopHeightDist - ropes[0].ropeTailEnd.go.transform.position.y);
        //if (AnchorAwayDist > AnchorAwayDistDownLimit)
        if( h > BottomHeightLimitThreshold)
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
     
    public void LiftControllerReset()
    {
       // Debug.Log("Lift reset ");

        bool bStopMovement = false;
        do
        {
            bStopMovement = false;
            foreach (Rope rope in ropes)
            {
                var dist = AnchorPtTransform.transform.position.y - rope.transform.position.y;
                if (dist > AnchorAwayDistDownLimit)
                    break;

                rope.gameObject.transform.position -= (Vector3.up * speed);
                bStopMovement = true;
            }
        } while (bStopMovement);
    }

    public void Update()
    {
        if (ropes.Count <= 0 || !bRopesReady )
            return;


        // Test only
        AnchorAwayDist = AnchorPtTransform.transform.position.y - ropes[0].transform.position.y;
        HeightDist = AnchorPtTransform.transform.position.y - ropes[0].ropeTailEnd.go.transform.position.y;

       // float h = (TopHeightDist - ropes[0].ropeTailEnd.go.transform.position.y);
       // Debug.Log("Tail Distance: " +  h);

        //if( h > 40.5 ) // testing
        //{
        //    ls.spring = 10000;
        //    ls.Damper = 10000;
        //}

        foreach (Rope rope in ropes)
        {
            rope.spring = ls.spring;
            rope.damper = ls.Damper;
        }

    }
}
