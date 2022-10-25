using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollisionHandler : MonoBehaviour
{
    public LiftSettings ls;

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("RopeTail"))
            return;

        ls.bLiftDownOperatable = false;

        WallDisplay.Display("Rope limit reached !!");
    }

    public void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("RopeTail"))
            return;

        ls.bLiftDownOperatable = false;
    }

    public void OnCollisionExit(Collision collision)
    {
        if (!collision.collider.CompareTag("RopeTail"))
            return;

        ls.bLiftDownOperatable = true;
    }
}

