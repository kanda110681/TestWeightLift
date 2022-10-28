using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public Transform topAnchorPt;
    public LiftSettings ls;
    public List<MassConfiguration> weightGOs = new List<MassConfiguration>();

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

        ls.cbAddMass += AddMass;
        ActivateMass(0);
    }

    private void OnDisable()
    {
        ls.cbAddMass -= AddMass;
    }

    void ActivateMass(int wt)
    {
        foreach(MassConfiguration config in weightGOs)
        {
            config.MassReset();
            config.gameObject.SetActive(false);
        }

        currentWeightConfiguration = wt;
        mc = weightGOs[wt];

        mc.gameObject.SetActive(true);

       // mc.MassReset();

        
        if (wt == 2)
            mc.gameObject.transform.rotation = Quaternion.Euler(0, 0, 90f);
        else
            mc.gameObject.transform.rotation = Quaternion.identity;

        mc.gameObject.transform.position = new Vector3(topAnchorPt.position.x, 3, topAnchorPt.position.z);
    }

    public float CurrentWeight()
    {
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

    public List<Rigidbody> PrepareWeightConfigurationConnections(int nRopes)
    {
        if (mc == null) return null;

        return mc.PrepareWeightConfigurationConnections(nRopes);
    }
}
