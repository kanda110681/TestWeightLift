using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public Transform topAnchorPt;  // Reference for load placement on floor
    public LiftSettings ls;
   
    public List<GameObject> weightGOs = new List<GameObject>(); // prefab
    GameObject currentWeightGO;

    // Max weight limit setup for all lift configuration
    public List<float> LiftMaxWeightLimits = new List<float>();

    public int currentWeightConfiguration = -1;
    MassConfiguration mc = null;

    public static WeightManager Instance;

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(Instance);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

       
        ActivateMass(0);
    }

    private void OnEnable()
    {
        ls.cbAddMass += AddMass;
    }

    private void OnDisable()
    {
        ls.cbAddMass -= AddMass;
    }


    /*
     * Always only one Load available
     * Delete old one and create new one
     */ 
    void ActivateMass(int wt)
    {
        if(currentWeightGO != null)
        {
            DestroyImmediate(currentWeightGO);
            currentWeightGO = null;
            mc = null;
        }

        currentWeightConfiguration = wt;

        currentWeightGO = Instantiate(weightGOs[currentWeightConfiguration]);
        mc = currentWeightGO.GetComponent<MassConfiguration>();
        mc.gameObject.SetActive(true);

        if (wt == 2) // pipe rotation
            mc.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90f);
        else
            mc.gameObject.transform.rotation = Quaternion.identity;

        // place load based on anchor pt - rope position
        mc.gameObject.transform.position = new Vector3(topAnchorPt.position.x, 3, topAnchorPt.position.z);
    }

    public float CurrentWeight()
    {
        if (mc == null)
            return 0;

        return mc.Mass;
    }

    public void AddMass()
    {
        if( mc == null || ls.nSelectedLift < 0) return;

        if (ls.Weight == 0)
        {
            WallDisplay.Display("Minimium 1 kg");
            return;
        }

        if(ls.Weight > 500)
        {
            WallDisplay.Display("Add Limit 500 kg");
            return;
        }

        float nw = mc.Mass + ls.Weight;
        if ( nw > LiftMaxWeightLimits[ls.nSelectedLift] )
        {
            WallDisplay.Display("Limit is " + LiftMaxWeightLimits[ls.nSelectedLift] + " kg");
            return;
        }
        
        mc.UpdateMass(ls.Weight);
    }
   
    public void CheckAndDestroy(int lift)
    {
        if (mc == null) return;

        ActivateMass((int)ls.LType);
    }


    /*
     *  Lift controller calls it - ropes tail end attached to this child rb
     */
    public List<Rigidbody> PrepareWeightConfigurationConnections(int nRopes)
    {
        if (mc == null) return null;

        return mc.PrepareWeightConfigurationConnections(nRopes);
    }
}
