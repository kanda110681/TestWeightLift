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
    float SpringElasticity = 10;
    public int MAX_TIMES_WEIGH_ADD = 10;

    public float SlingElasticity
    {
        get { return SpringElasticity; }
        set { 
            SpringElasticity = value;
          //  Debug.Log("[[ Spring Elasticity Modified: " + value);
            cbSlingElasticityModified?.Invoke();
        }
    }

    public float Weight=5;
    public LoadType LType = LoadType.LT_CUBE;

    public UnityAction cbUp;
    public UnityAction cbDown;
    public UnityAction cbAddMass;
    public UnityAction cbSlingElasticityModified;
    public UnityAction cbMassTypeChanged;

   // public float ElasticityScaleFactor = 0.5f;
    public float chainDispSize = 0.3f;

   // public GameObject prefabAddMass;

    public float MAX_SPRING = 50000;
   // public float spring = 1; // actual spring
    public float Damper = 1f;

    public bool bLiftUPOperatable = true;
    public bool bLiftDownOperatable = true;
    public int nSelectedLift = -1; // current operating lift
  //  public Transform topAnchorPt;

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

    //IEnumerator ContinousWeightAddtion(float weight)
    //{
    //    do
    //    {
           
    //        yield return new WaitForSeconds(0.2f);
    //    } while (weight > 0.0001f);
    //}

    //public void LiftReset()
    //{
    //    spring = 1;
    //}
}
