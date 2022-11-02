using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum LoadType
{
    LT_CUBE,
    LT_DISC,
    LT_CYLINDER,
}

[CreateAssetMenu(menuName = "Create/LiftSettings")]
public class LiftSettings : ScriptableObject
{
    // Joint - Damper 
    public float Damper = 1f;


    // Joint - Spring 
    float SpringElasticity = 10;
    public float SlingElasticity
    {
        get { return SpringElasticity; }
        set { 
            SpringElasticity = value;
          //  Debug.Log("[[ Spring Elasticity Modified: " + value);
            cbSlingElasticityModified?.Invoke();
        }
    }

    // Load weight and shape types
    public float Weight=5;
    public LoadType LType = LoadType.LT_CUBE;

    // Observers - Callbacks
    public UnityAction cbUp;
    public UnityAction cbDown;
    public UnityAction cbAddMass;
    public UnityAction cbSlingElasticityModified;
    public UnityAction cbMassTypeChanged;

    // Lift controller - limitation set it from Load
    public bool bLiftUPOperatable = true;
    public bool bLiftDownOperatable = true;

    // current operating lift (Ropes)
    public int nSelectedLift = -1; 

    public void LiftUP()
    {
        if (!bLiftUPOperatable)
            return;

        cbUp?.Invoke();
    }

    public void LiftDown()
    {
        if (!bLiftDownOperatable)
            return;

        cbDown?.Invoke();
    }

    public void AddMass()
    {
        cbAddMass?.Invoke();
    }
      
}
