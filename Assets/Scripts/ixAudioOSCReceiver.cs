using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscCore;
using UnityEngine.Events;

public enum ParamId
{
    time,
    rootMeanSquare,
	pitch,
	peakEnergy,
	zeroCrossingRate,
	spectralCentroid,
	spectralCrest,
	spectralFlatness,
	spectralRolloff,
	spectralKurtosis,
	energyDifference,
	spectralDifference,
	spectralDifferenceHWR,
	complexSpectralDifference,
	highFrequencyContent,
	COUNT
};

public class ixAudioOSCReceiver : MonoBehaviour
{

    public static AudioEvent currentAudioEvent;

    class FloatParamsCallback
    {
        OscActionPair ActionPair { get; set; }

        public float[] m_params = new float[(int) ParamId.COUNT];
        public FloatParamsCallback(OscServer server)
        {
            // Single-thread version..
          //  server.TryAddMethod("/params", ReadValuesFromOSC);
            // Double-thread version..
            server.TryAddMethodPair("/params", ReadValuesFromOSC, MainThreadMethod);
        }

        void ReadValuesFromOSC(OscMessageValues values)
        {
           /* for (int i = 0; i < (int)ParamId.COUNT; i++) { 
                m_params[i] = values.ReadFloatElement(i);
                Debug.Log(values.ReadFloatElement(i));
            }*/
            values.ReadBlobAsFloatArray(0,ref m_params);

            currentAudioEvent.time =                    m_params[0];
            currentAudioEvent.rootMeanSquare =          m_params[1];
            currentAudioEvent.pitch =                   m_params[2];
            currentAudioEvent.peakEnergy =              m_params[3];
            currentAudioEvent.zeroCrossingRate =        m_params[4];
            currentAudioEvent.spectralCentroid =        m_params[5];
            currentAudioEvent.spectralCrest =           m_params[6];
            currentAudioEvent.spectralFlatness =        m_params[7];
            currentAudioEvent.spectralRolloff =         m_params[8];
            currentAudioEvent.spectralKurtosis =        m_params[9];
            currentAudioEvent.energyDifference =        m_params[10];
            currentAudioEvent.spectralDifference =      m_params[11];
            currentAudioEvent.spectralDifferenceHWR =   m_params[12];
            currentAudioEvent.complexSpectralDifference = m_params[13];
            currentAudioEvent.highFrequencyContent =     m_params[14];

        }

        void MainThreadMethod()
        {
           
        }
    }

    FloatParamsCallback m_callback;
    public void Start()
	{
        OscReceiver receiver = GetComponentInParent<OscReceiver>();

        m_callback = new FloatParamsCallback(receiver.Server);

        currentAudioEvent = new AudioEvent();
      
    }
}
