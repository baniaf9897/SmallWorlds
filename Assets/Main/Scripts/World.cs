using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public struct ShapePropertyDomains
{


    [Range(0.1f, 5.0f)]
    public float minSize;

    [Range(0.1f, 5.0f)]
    public float maxSize;

    [Range(0.0f, 2.0f)]
    public float minCoherence;
    [Range(0.0f, 2.0f)]
    public float maxCoherence;

    [Range(0.0f, 2.0f)]
    public float minSeperation;
    [Range(0.0f, 2.0f)]
    public float maxSeperation;

    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;

    [Range(1, 32768)]
    public int minNumber;
    [Range(1, 32768)]
    public int maxNumber;
}
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
    private AudioManager m_audioManager;
    private InteractionManager m_interactionManager;
 
     void Start()
    {
        Setup();
    }
    
    void Setup()
    {
        m_shapeManager = new ShapeManager(computeShaderTmp,materialTmp,quadSize,limits);
        m_audioManager = new AudioManager();
        m_interactionManager = new InteractionManager();


        Debug.Log("[World] Setup finished");
    }

    private void OnValidate()
    {
        Setup();
        m_shapeManager.SetGlobalCoherence(limits.minCoherence);

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
            AudioEvent audioEvent = new AudioEvent();
            audioEvent.value = data[i];
            m_shapeManager.Update(audioEvent);
        }

        Debug.Log("[World] Rapid Mode finished");

    }

    void Update()
    {
        if (!rapidMode)
        {
            m_audioManager.Update();
            m_shapeManager.Update(m_audioManager.GetCurrentAudioEvent());
        }
        else
            m_shapeManager.Draw();
    }

}
