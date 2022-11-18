using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LiftUISettings : MonoBehaviour
{
    public LiftSettings ls;

    public TMP_InputField springElasticityTxt;
    public TMP_InputField weightTxt;

    private void Awake()
    {
        ls.LType = LoadType.LT_CUBE;
        ls.nSelectedLift = -1;
        ls.Damper = 0;
        ls.Weight = 5f;
        ls.SlingElasticity = 10f;
        ls.bLiftUPOperatable = false;
        ls.bLiftDownOperatable = false;
    
        ls.cbSlingElasticityModified += SlingElasticityModified;
    }

    public void SlingElasticityModified()
    {
        springElasticityTxt.text = ls.SlingElasticity.ToString();
    }

    float Parse(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0.0f;

        float f = 0.0f;

        try
        {
            f = float.Parse(str);
        }
        catch(System.Exception e)
        {
            f = 0.0f;
        }
        

        return f;
    }

    public void UpdateSlingElasticity(string elasticity)
    {
        if (ls) ls.SlingElasticity = Parse(elasticity);
      //  Debug.Log("Elasticiy: " + ls.SlingElasticity);
    }

    public void UpdateMass(string mass)
    {
        if (ls) ls.Weight = Parse(mass);

       // Debug.Log("mass: " + mass);
    }

    public void UpdateMassGeomType(int type)
    {
       // WallDisplay.DisplayUnderProgress();

        if (ls)
        {
            ls.LType = (LoadType)type;
            ls.cbMassTypeChanged?.Invoke();
            //  Debug.Log("type: " + ls.LType);
        }
    }

    public void UpdateDamper(float d)
    {
        if (ls) ls.Damper = d;
    }

    public void AddMass()
    {
        if (ls)
        {
            ls.Weight = Parse(weightTxt.text); 
            ls.AddMass();
        }
    }

    public void LiftUP()
    {
        if(ls) ls.LiftUP();
    }

    public void LiftDown()
    {
        if (ls) ls.LiftDown();
    }

    bool bUpPressed = false;

    public void OnPressedUp()
    {
        bUpPressed = true;
    }

    public void OnNotPressedUp()
    {
        bUpPressed = false;
    }

    bool bDownPressed = false;

    public void OnPressedDown()
    {
        bDownPressed = true;
    }

    public void OnNotPressedDown()
    {
        bDownPressed = false;
    }
    private void Update()
    {
        if(bUpPressed)
        {
            LiftUP();
        }

        if (bDownPressed)
            LiftDown();
    }
}
