using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLine : MonoBehaviour
{
    public GameObject ROPE;
    public Rigidbody pt1;
    public Rigidbody pt2;

    public List<Rigidbody> rigidbodies = new List<Rigidbody>();
    LineRenderer lineRenderer;

    public bool bConnected = false;

    private void Start()
    {
       // Debug.Log("p2: " + pt2.transform.position);
    }

    private void createRope()
    {
        var ro = Instantiate(ROPE, pt2.transform);
        var rope = ro.GetComponent<Rope>();
        ro.transform.position = pt2.transform.position;
        rope.topAnchorPtRB = pt2;
        rope.bottomAnchorPtRB = pt1;
        rope.linkLength = 2f;
        rope.Init();
    }

    private void Update()
    {
        if(!bConnected && pt1 != null && pt2 != null )
        {
            bConnected = true;
            createRope();
        }
    }

    // Start is called before the first frame update
    void Start2()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;

        foreach (var rigidbody in rigidbodies)
            Debug.Log(rigidbody.gameObject.transform.position);

    }

    // Update is called once per frame
    void Update2()
    {
        lineRenderer.positionCount = 1;
        foreach (var rigidbody in rigidbodies)
        {
            lineRenderer.SetPosition(lineRenderer.positionCount-1, rigidbody.gameObject.transform.position);
            lineRenderer.positionCount++;
        }
    }
}
