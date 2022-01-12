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
    public static int currentPolyphonicPitch;

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
            server.TryAddMethodPair("/pp", ReadPolyphonicPitchValuesFromOSC, MainThreadMethod);
        }

        void ReadPolyphonicPitchValuesFromOSC(OscMessageValues values)
        {
            float[] temp = new float[(128)];

            values.ReadBlobAsFloatArray(0, ref temp);
            float max = 0.0f;
            int maxIndex = 0;

            for(int i = 0; i < temp.Length; i++)
            {
                if(temp[i] > max)
                {
                    max = temp[i];
                    maxIndex = i;
                }
            }

            currentPolyphonicPitch = maxIndex;
            //MidiToNote(maxIndex, ref currentPolyphonicPitch);


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
        currentPolyphonicPitch = -1;
      
    }


    public static void MidiToNote(int midi, ref string s)
    {
        int note = midi % 12;

        switch (note)
        {
            case 0:
                s = "C";
                break;
            case 1:
                s = "C#";
                break;
            case 2:
                s = "D";
                break;
            case 3:
                s = "D#";
                break;
            case 4:
                s = "E";
                break;
            case 5:
                s = "F";
                break;
            case 6:
                s = "F#";
                break;
            case 7:
                s = "G";
                break;
            case 8:
                s = "G#";
                break;
            case 9:
                s = "A";
                break;
            case 10:
                s = "Bb";
                break;
            case 11:
                s = "B";
                break;
            default:
                break;

        }

        //int octave = (midi - 12) / 12;
        //s += octave.ToString();
    }

}
