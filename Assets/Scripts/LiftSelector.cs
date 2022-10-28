using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LiftSelector : MonoBehaviour
{
    //  public List<GameObject> lifts = new List<GameObject>();
    LiftController liftContoller;
    GameObject currentLiftGO;
    public Material ropeMaterial;

    public int lift = -1;
    public LiftSettings ls;
    public WeightManager wm;

    public float ropeLength = 70f;
    public float ropeLinkLength = 1f;

    List<Rigidbody> connectionRigidBodies; 

    
    public Transform AnchorPtTransform;

    public float TopHeightDistLimit = 68.0f;
    public float BottomHeightDistLimit = 41f;

    public UnityAction cbNewLiftConfigured;
    public UnityAction cbResetRequired;

    public void Awake()
    {
        if(ls != null ) ls.nSelectedLift = -1;
        ls.cbMassTypeChanged += MassTypeChanged;
        currentLiftGO = null;
    }
    void MassTypeChanged()
    {
        if (lift >= 0 && lift != 3)  // avoid 3 ropes
            lift++; 
        OnLiftSelector(lift);
    }

    public void OnLiftSelector(int type)
    {
        if(type != -1 )
            WallDisplay.DisplayUnderProgress();

        lift = -1;
        if (type == 1)
        {
            lift = 0;
        }
        else if (type == 2)
        {
            lift = 1;
        }
        else if (type == 3) // 4 ropes
        {
            lift = 3;
        }
        
        ls.nSelectedLift = lift;

       
        if(currentLiftGO != null)
        {
            var tmpLC = currentLiftGO.GetComponent<LiftController>();
            if (tmpLC != null)
                tmpLC.SetInvalidLift();

            Destroy(currentLiftGO);
            currentLiftGO = null;
        }

        if (lift < 0)
            return;

        wm.CheckAndDestroy(lift);

        currentLiftGO = new GameObject();
        var lc = currentLiftGO.AddComponent<LiftController>();
        lc.ls = ls;
        lc.wm = wm;
        lc.AnchorPtTransform = AnchorPtTransform;
        lc.TopHeightDistLimit = TopHeightDistLimit;
        lc.BottomHeightDistLimit = BottomHeightDistLimit;
        lc.ropeMaterial = ropeMaterial;
        lc.Init();

        //lc.SetupRopes(type+1, ropeLength, ropeLinkLength, wm);
        StartCoroutine(SetupRopes());
    }

    IEnumerator SetupRopes()
    {
        yield return new WaitForSeconds(0.2f);

        cbResetRequired?.Invoke();

        var lc = currentLiftGO.GetComponent<LiftController>();

        int r = lift + 1;
        if (lift == 3)
            r = 4;
        lc.SetupRopes(r, ropeLength, ropeLinkLength, wm);

        cbNewLiftConfigured?.Invoke();
    }

    public LiftController GetCurrentLiftConotroller()
    {
        if(currentLiftGO != null)   
            return currentLiftGO.GetComponent<LiftController>();

        return null;
    }
}
