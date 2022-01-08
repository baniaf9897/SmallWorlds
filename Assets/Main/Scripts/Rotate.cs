using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public float rotateX = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentRot = transform.rotation.eulerAngles;
        transform.Rotate(new Vector3(transform.rotation.eulerAngles.x,0,0));
    }
}
