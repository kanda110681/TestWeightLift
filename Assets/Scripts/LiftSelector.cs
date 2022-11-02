using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LiftSelector : MonoBehaviour
{
    int lift = -1; // current lift and rope configuration
    GameObject currentLiftGO; 
    public Material ropeMaterial;

    
    public LiftSettings ls;
    public WeightManager wm;

    public float ropeLength = 70f;
    public float ropeLinkLength = 1f;
   
    public Transform AnchorPtTransform;  // rope head starting point

    // indepent movement
    //public float TopHeightDistLimit = 68.0f;   
    // public float BottomHeightDistLimit = 41f;


    /*
     * [BEFORE CALL] - New lift (ropes) going to be configured. Notify to others.
     */
    public UnityAction cbResetRequired;

    /*
     *  [AFTER CALL] - all ropes and weight configured. Notify to others.
     */
    public UnityAction cbNewLiftConfigured;
    

    public void Awake()
    {
        if(ls != null ) ls.nSelectedLift = -1;
        currentLiftGO = null;
    }

    private void OnEnable()
    {
        ls.cbMassTypeChanged += MassTypeChanged;
    }

    private void OnDisable()
    {
        ls.cbMassTypeChanged -= MassTypeChanged;
    }

    void MassTypeChanged()
    {
        if (lift >= 0 && lift != 3)  // avoid 3 ropes // it does not configured it correctly - centre of gravity is not correct
            lift++; 
        OnLiftSelector(lift);
    }


    /*
     *  Directly linked to Canvas UI 
     *  - Always create new lift controller for every change in ropes and load types
     */
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

        // Destroy existing go
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

        // Destroy load and reconfigure load based on ropes 
        wm.CheckAndDestroy(lift);

        // New object creation
        currentLiftGO = new GameObject();
        currentLiftGO.name = "Lift Rope Controller";
        var lc = currentLiftGO.AddComponent<LiftController>();
        lc.ls = ls;
        lc.wm = wm;
        lc.AnchorPtTransform = AnchorPtTransform;
       // lc.TopHeightDistLimit = TopHeightDistLimit;
       // lc.BottomHeightDistLimit = BottomHeightDistLimit;
        lc.ropeMaterial = ropeMaterial;
        lc.Init();

        StartCoroutine(SetupRopes());
    }

    IEnumerator SetupRopes()
    {
       // yield return new WaitForSeconds(0.1f);

        /*
         *  Main lift controller - Reset - recreate top main rope 
         */
        cbResetRequired?.Invoke(); 

        /*  Slight delay for main rope crteation
         */
        yield return new WaitForSeconds(0.2f);

        /*
         * Setup bottom ropes based on lift types
         */
        var lc = currentLiftGO.GetComponent<LiftController>();

        int r = lift + 1;
        if (lift == 3)
            r = 4;
        lc.SetupRopes(r, ropeLength, ropeLinkLength, wm);

        /*
         *  Inform to Main lift controller - ropes are ready to attached to main rope
         */
        cbNewLiftConfigured?.Invoke();
    }

    public LiftController GetCurrentLiftConotroller()
    {
        if(currentLiftGO != null)   
            return currentLiftGO.GetComponent<LiftController>();

        return null;
    }
}
