using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftSelector : MonoBehaviour
{
    public List<GameObject> lifts = new List<GameObject>();
    public int lift = -1;
    public LiftSettings ls;
    public WeightManager wm;

    public float ropeLength = 70f;
    public float ropeLinkLength = 1f;

    List<Rigidbody> connectionRigidBodies; 

    GameObject currentLiftGO;
    public Transform AnchorPtTransform;

    public void Awake()
    {
        if(ls != null ) ls.nSelectedLift = -1;
        currentLiftGO = null;
    }

    public void OnLiftSelector(int type)
    {
        type = type - 1;
        if ( lifts.Count <= type )
            return;

        if(currentLiftGO != null)
        {
            Destroy(currentLiftGO);
            currentLiftGO = null;
        }

        lift = type;
        ls.nSelectedLift = lift;

        wm.CheckAndDestroy(lift);

        currentLiftGO = Instantiate(lifts[lift]);
        var lc = currentLiftGO.GetComponent<LiftController>();
        lc.AnchorPtTransform = AnchorPtTransform;

        lc.SetupRopes(type+1, ropeLength, ropeLinkLength);

       
    }

    public void LiftReset()
    {
        // Debug.Log("Lift reset 1 ");
        if (currentLiftGO == null)
            return;


        Time.timeScale = 0;
        ls.LiftReset();

        var lc = currentLiftGO.GetComponent<LiftController>();
        if (lc != null)
        {
            lc.LiftControllerReset();
        }

        //foreach (GameObject lift in lifts)
        //{
        //    var lc = lift.GetComponent<LiftController>();
        //    if(lc != null)
        //    {
        //        lc.LiftControllerReset();
        //    }
        //}
        Time.timeScale = 1;
    }

    Vector3 getPos(Rigidbody rb)
    {
        return new Vector3(rb.gameObject.transform.position.x, rb.gameObject.transform.position.y, rb.gameObject.transform.position.z);
    }

    public void AttachWeightToLift()
    {
        if (lift < 0 || wm.currentWeightGO == null || currentLiftGO == null)
            return;

        var mc = wm.currentWeightGO.GetComponent<MassConfiguration>();
       

        var lc = currentLiftGO.GetComponent<LiftController>();
        int nRopes = lc.ropes.Count;

        if( mc.Mass > 1000 && nRopes == 1)
        {
            WallDisplay.Display("Limit 1000kg");
            return;
        }
        else if (mc.Mass > 1500 && nRopes == 2)
        {
            WallDisplay.Display("Limit is 1500 kg");
            return;
        }
        else if (mc.Mass > 2500 && nRopes == 3)
        {
            WallDisplay.Display("Limit is 2500 kg");
            return;
        }
        else if (mc.Mass > 3000 && nRopes == 4)
        {
            WallDisplay.Display("Limit is 3000 kg");
            return;
        }

        connectionRigidBodies = wm.PrepareWeightConfigurationConnections(nRopes);

        int i = 0;
        foreach(var rb in connectionRigidBodies)
        {
            var tail = lc.ropes[i].ropeTailEnd;

            if(tail.tailJoint == null)
            {
                tail.AttachTailJoint();

              //  Debug.Log("Tail Joint added. " + tail.tailJoint != null);
            }

            tail.go.transform.position = getPos(rb);
            tail.tailJoint.connectedBody = wm.currentWeightGO.GetComponent<Rigidbody>();
            i++;
        }


    }
}
