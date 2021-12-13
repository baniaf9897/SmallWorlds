using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private ShapeManager m_shapeManager;
    private AudioManager m_audioManager;
    private InteractionManager m_interactionManager;

     void Start()
    {
        Setup();
    }
    
    void Setup()
    {
        m_shapeManager = new ShapeManager();
        m_audioManager = new AudioManager();
        m_interactionManager = new InteractionManager();
       
        Debug.Log("[World] Setup finished");
    }

    void Update()
    {
        
        m_audioManager.Update();
        m_shapeManager.Update(m_audioManager.GetCurrentAudioEvent());

    }

}
