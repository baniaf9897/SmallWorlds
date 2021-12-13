using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
{
    private AudioEvent m_currentAudioEvent;
    public AudioManager()
    {
        Setup();
    }
    void Setup()
    {
        m_currentAudioEvent = new AudioEvent();
        m_currentAudioEvent.value = 0.0f;
        Debug.Log("[AudioManager] Setup finished");
    }

    public AudioEvent GetCurrentAudioEvent()
    {
        return m_currentAudioEvent;
    }

    public void Update()
    {
        m_currentAudioEvent.value = Random.Range(0,10);
        //call audio analysis and set current audio event accordingly 
    }


}
