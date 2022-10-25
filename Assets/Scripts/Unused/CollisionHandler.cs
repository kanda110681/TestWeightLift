using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    Rigidbody rb;
    Coroutine cr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator AutoWakeupCR()
    {
        yield return null;
        rb.WakeUp();
        StopCoroutine(cr);
        cr = null;
    }

    void AutoWakup()
    {
        if (cr != null) StopCoroutine(cr);
        cr = StartCoroutine(AutoWakeupCR());
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision: " + collision.collider.gameObject.tag);

        if (collision.collider.gameObject.tag == "Environment")
        {
            rb.Sleep();
            AutoWakup();
            return;
        }
           

        if (collision.collider.gameObject.tag != "Ground")
            return;

        if (!rb.IsSleeping())
        {
            rb.Sleep();
            AutoWakup();
        }
        else
            rb.WakeUp();
    }

    public void FixedUpdate()
    {
        if( rb.transform.position.y <= 0.5f || rb.transform.position.y >= 27.0f )
        {
            rb.Sleep();
            // AutoWakup();
            rb.WakeUp();
        }
    }

    public void Update()
    {

    }
}
