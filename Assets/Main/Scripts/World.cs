using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class World : MonoBehaviour
{
    public bool rapidMode = false;

    [Range(0.0f, 1.0f)]
    public float rapidModeProgress = 0.0f;

    public ComputeShader computeShaderTmp;
    public Material materialTmp;

    public ShapePropertyDomains limits;
    [Range(0f, 1f)]
    public float quadSize = 0.5f;

    private ShapeManager m_shapeManager;
    public AudioManager m_audioManager;
    private InteractionManager m_interactionManager;

    ixAudioOSCReceiver m_audioOSCReceiver;
    Mapper m_mapper;
 
     void Start()
    {
        Setup();
    }
    
    void Setup()
    {
        m_mapper = GetComponent<Mapper>();
        if(m_mapper == null)
        {
            Debug.LogWarning("NO MAPPER FOUND ! ADD MAPPER !");
        }


        m_audioOSCReceiver = GetComponent<ixAudioOSCReceiver>();

        m_shapeManager = new ShapeManager(computeShaderTmp,materialTmp,quadSize,m_mapper);
        m_audioManager = new AudioManager();
        m_interactionManager = new InteractionManager();


        Debug.Log("[World] Setup finished");
    }

    private void OnValidate()
    {
        Setup();

        if (rapidMode)
        {
            RunRapidMode();
        };

    }


    private void OnDestroy()
    {
        m_shapeManager.OnDestroy();
    }
    void RunRapidMode()
    {

        //get Data
        List<float> data = m_audioManager.GetAudioData(); 

        for (int i = 0; i < rapidModeProgress * data.Count; i++)
        {
           // AudioEvent audioEvent = new AudioEvent();
           // audioEvent.rootMeanSquare = data[i];
          //  m_shapeManager.Update(audioEvent);
        }

        Debug.Log("[World] Rapid Mode finished");

    }

    void Update()
    {
        if (!rapidMode)
        {
            m_audioManager.Update();
            m_shapeManager.Update(ixAudioOSCReceiver.currentAudioEvent);
            //m_shapeManager.Update(m_audioManager.GetCurrentAudioEvent());
        }
        else
            m_shapeManager.Draw();
    }

}
