using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightCollisionHandler : MonoBehaviour
{
    public LiftSettings ls;
    public string GROUND = "Floor";
    public string CEILING = "Ceiling";
    public float HeightFromGround = 0f;
    public float LiftMaxHeight = 65f;
    float ObjectHeight = 3f;
    float FORCE_FACTOR = 0.2f;
    MassConfiguration mc;

    bool bApplyHeightForce = false;

    private void Awake()
    {
        mc = GetComponent<MassConfiguration>();
        ObjectHeight = GetComponent<Collider>().bounds.size.y;
        //Debug.Log(GetComponent<Collider>().bounds.size);
        bApplyHeightForce = false;
    }

#if false 
    private void Update()
    {
        //HeightFromGround = transform.position.y - ObjectHeight ;

        //float hr = 1.0f - ((LiftMaxHeight - HeightFromGround) / LiftMaxHeight);
        //float downForce =  hr * 9.8f * mc.Mass * FORCE_FACTOR;

        //if(bApplyHeightForce)
        //    mc.rb.AddForce(Vector3.down * downForce);


     //  Debug.Log("Height: " + HeightFromGround + " DownForce: " + downForce + " Flag: "+ bApplyHeightForce);
    }
#endif 

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(CEILING))
        {
            ls.bLiftUPOperatable = false;
            ls.bLiftDownOperatable = true;
            WallDisplay.Display("Stop Lifting!!");
            bApplyHeightForce = true;
        }
        else if (collision.collider.CompareTag(GROUND))
        {
            ls.bLiftUPOperatable = true;
            ls.bLiftDownOperatable = false;
            WallDisplay.Display("Grounded");
            bApplyHeightForce = false;
        }

        //Debug.Log("collision: " + collision.gameObject.name);
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag(CEILING))
        {
            ls.bLiftUPOperatable = false;
            ls.bLiftDownOperatable = true;
            bApplyHeightForce = true;
        }
        else if (collision.collider.CompareTag(GROUND))
        {
            ls.bLiftUPOperatable = true;
            ls.bLiftDownOperatable = false;
            bApplyHeightForce = false;
        }

    }

    public void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(CEILING))
        {
            ls.bLiftUPOperatable = true;
            ls.bLiftDownOperatable = true;
            bApplyHeightForce = true;
        }
        else if (collision.collider.CompareTag(GROUND))
        {
            ls.bLiftUPOperatable = true;
            ls.bLiftDownOperatable = true;
            bApplyHeightForce = true;
        }

    }
}
