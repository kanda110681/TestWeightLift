using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Distance : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;

    public TextMeshProUGUI distTxt;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distTxt.text = Vector3.Distance(pos1.position, pos2.position).ToString();
    }
}
