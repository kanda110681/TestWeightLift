using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public float MaxMassLimit = 3000;
    public Transform topAnchorPt;
    public LiftSettings ls;
    public List<GameObject> weightPrefabs = new List<GameObject>();

    public int currentWeightConfiguration = -1;
    public GameObject currentWeightGO;

    public void Awake()
    {
        ls.cbAddMass = AddMass;
    }

    public void AddMass()
    {
        MassConfiguration mc = null;
        if (currentWeightGO != null)
        {
            mc = currentWeightGO.GetComponent<MassConfiguration>();
            if( mc != null && mc.loadType != ls.LType )
            {
                DestroyImmediate(currentWeightGO);
                currentWeightGO = null;
            }
        }

        if (currentWeightGO != null)
        {
            if (ls.nSelectedLift == 0 && (mc != null && (mc.Mass + ls.Weight) > 1000))
            {
                WallDisplay.Display("Limit is 1000 kg");
                return;
            }
            else if (ls.nSelectedLift == 1 && (mc != null && (mc.Mass + ls.Weight) > 1500))
            {
                WallDisplay.Display("Limit is 1500 kg");
                return;
            }
            else if (ls.nSelectedLift == 2 && (mc != null && (mc.Mass + ls.Weight) > 2500))
            {
                WallDisplay.Display("Limit is 2500 kg");
                return;
            }

            if (ls.Weight > MaxMassLimit || (mc != null && (mc.Mass + ls.Weight) > MaxMassLimit))
            {
                // ls.Weight = 5000;
                WallDisplay.Display("Limit is " + MaxMassLimit + " kg");
                return;
            }

            if (ls.Weight == 0)
            {
                WallDisplay.Display("minimium 1 kg");
                return;
            }
        }
        
       
        if(currentWeightGO == null )
        {
            currentWeightGO = Instantiate(weightPrefabs[(int)ls.LType]);
            currentWeightGO.transform.position = new Vector3(topAnchorPt.position.x, 20, topAnchorPt.position.z);
            mc = currentWeightGO.GetComponent<MassConfiguration>();
        }
        
        if( mc != null )
            mc.UpdateMass(ls.Weight);
    }

    public void DestroyWeight()
    {
        if (currentWeightGO != null)
            Destroy(currentWeightGO);
        currentWeightGO = null;
    }

    public void CheckAndDestroy(int lift)
    {
        if (currentWeightConfiguration == lift)
            return;
        currentWeightConfiguration = -1;
        DestroyWeight();
    }

    public List<Rigidbody> PrepareWeightConfigurationConnections(int nRopes)
    {
        if (currentWeightGO == null)
            return null;
        currentWeightConfiguration = nRopes-1;
        var mc = currentWeightGO.GetComponent<MassConfiguration>();

        return mc.PrepareWeightConfigurationConnections(nRopes);
    }
}
