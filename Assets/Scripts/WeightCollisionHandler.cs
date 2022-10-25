using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightCollisionHandler : MonoBehaviour
{
    public LiftSettings ls;

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Ceiling"))
            return;

        ls.bLiftUPOperatable = false;   

        WallDisplay.Display("Stop Lifting!!");
        //Debug.Log("collision: " + collision.gameObject.name);

    }

    public void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Ceiling"))
            return;

        ls.bLiftUPOperatable = false;
       // WallDisplay.Display("Stop Lifting!!");
    }

    public void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.CompareTag("Ceiling"))
            return;

       // Debug.Log("collision exit: " + collision.gameObject.name);
        ls.bLiftUPOperatable = true;
    }
}
