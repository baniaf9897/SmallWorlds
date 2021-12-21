using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSC_Receiver : MonoBehaviour
{
    // Start is called before the first frame update
    public OSC osc;

    void Start()
    {
        osc.SetAddressHandler("/pitch", OnReceivePitch);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnReceivePitch(OscMessage message)
    {
        Debug.Log("Received Pitch Value: ");
        Debug.Log(message);
        Debug.Log("========================");
    }
}
