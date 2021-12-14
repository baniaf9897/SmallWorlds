using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager 
{
    private AudioEvent m_currentAudioEvent;

    static List<float> m_debugData;

    int m_numSamples = 256;
    bool calculating = false;
    public AudioManager()
    {
        Setup();
    }
    void Setup()
    {
        m_currentAudioEvent = new AudioEvent();
        m_currentAudioEvent.value = 0.0f;

        if(m_debugData == null)
        {
            Debug.Log("init debug data");
            m_debugData = new List<float>();
            for(int i = 0; i < 1000; i++)
            {
                m_debugData.Add(Random.Range(0,5));
            }
        }


        Debug.Log("[AudioManager] Setup finished");
    }

    public AudioEvent GetCurrentAudioEvent()
    {
        return m_currentAudioEvent;
    }
    
    public List<float> GetAudioData()
    {
        return m_debugData;
    }

    public void Update()
    {
        m_currentAudioEvent.value = AnalyzeMaxFrequency();//Random.Range(1,5);
        //call audio analysis and set current audio event accordingly 
    }

    float RetrieveFreqFromBin(int i)
    {
        return i * AudioSettings.outputSampleRate / m_numSamples;
    }

    public float AnalyzeMaxFrequency()
    {
        if (calculating)
        {
            return -1.0f;
        }

        calculating = true;
        float[] spectrum = new float[m_numSamples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        float maxValue = -1.0f;
        int i = 0;

        for (int x = 0; x < spectrum.Length; x++)
        {
            if (spectrum[x] > maxValue)
            {
                maxValue = spectrum[x];
                i = x;
            }
        }

        float freq = RetrieveFreqFromBin(i);

        calculating = false;
        return freq;
    }

    public float AnalyzeCentroid()
    {
        if (calculating)
        {
            return -1.0f;
        }

        calculating = true;
        float[] spectrum = new float[m_numSamples];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        float centroid;
        float centerFreq;

        float fxAmp = 0.0f;
        float amp = 0.0f;


        for (int x = 0; x < spectrum.Length; x++)
        {
            centerFreq = RetrieveFreqFromBin(x);

            fxAmp += spectrum[x] * centerFreq;
            amp += spectrum[x];

        }

        centroid = fxAmp / amp;

        calculating = false;
        return centroid;

    }





}
