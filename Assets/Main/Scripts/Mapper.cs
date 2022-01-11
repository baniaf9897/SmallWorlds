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

    [Range(0.0f, 20.0f)]
    public float minMass;
    [Range(0.0f, 20.0f)]
    public float maxMass;

    [Range(0.0f, 2.0f)]
    public float minFriction;
    [Range(0.0f, 2.0f)]
    public float maxFriction;

    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;

    [Range(1, 32768 * 2)]
    public int minNumber;
    [Range(1, 32768 * 2)]
    public int maxNumber;
}
public class Mapper : MonoBehaviour
{


    public ShapePropertyDomains limits;

    public ParamId numberMapper;
    public ParamId massMapper;
    public ParamId speedMapper;
    public ParamId frictionMapper;
    public ParamId colorMapper;
    public ParamId sizeMapper ;

    public ParamId valueMapper;

    private void OnValidate()
    {
       /* AudioEvent e = new AudioEvent();
        e.rootMeanSquare = 1000;
        e.pitch = 1000;
        e.spectralCentroid = 1000;
        e.spectralCrest = 1000;
        e.spectralDifference = 1000;
        e.spectralDifferenceHWR = 1000;
        e.spectralFlatness = 1000;
        e.spectralKurtosis = 1000;
        e.spectralRolloff = 1000;
        e.highFrequencyContent= 1000;
        e.peakEnergy = 1000;
        e.zeroCrossingRate = 1000;
        e.complexSpectralDifference = 1000;

        float number = ValidateNumberofParticles(e);

        Debug.Log("Number  " + number);*/
    }


    public float[] GetParamLimit(ParamId paramID)
    {

        return new float[2] {0.0f,1.0f};

       /* if(paramID == ParamId.peakEnergy || paramID == ParamId.rootMeanSquare)
        {
            return new float[2]{0.0f,1.0f};
        }
        else if(paramID == ParamId.spectralCentroid)
        {
            return new float[2] { 0.0f, 150.0f };
        }
        float[] bounds = new float[2];
   
        bounds[0] = 100.0f;
        bounds[1] = 1000.0f;

        return bounds;*/
    }

    public float Map(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    public int ValidateNumberofParticles(AudioEvent audioEvent){

        float[] bounds = GetParamLimit(numberMapper);

        float param  = audioEvent[(int)numberMapper];

        return (int)Map(param,bounds[0],bounds[1], limits.minNumber, limits.maxNumber);
    }

    public float ValidateMass(AudioEvent audioEvent)
    {
        float[] bounds = GetParamLimit(massMapper);

        float param = audioEvent[(int)massMapper];

        return Map(param, bounds[0], bounds[1], limits.minMass, limits.maxMass);
    }

    public float ValidateSpeed(AudioEvent audioEvent)
    {
        float[] bounds = GetParamLimit(speedMapper);

        float param = audioEvent[(int)speedMapper];

        return Map(param, bounds[0], bounds[1], limits.minSpeed, limits.maxSpeed);
    }

    public float ValidateFriction(AudioEvent audioEvent)
    {
        float[] bounds = GetParamLimit(frictionMapper);

        float param = audioEvent[(int)frictionMapper];

        return Map(param, bounds[0], bounds[1], limits.minFriction, limits.maxFriction);
    }

    public float ValidateSize(AudioEvent audioEvent)
    {
        float[] bounds = GetParamLimit(sizeMapper);

        float param = audioEvent[(int)sizeMapper];

        return Map(param, bounds[0], bounds[1], limits.minSize, limits.maxSize);
    }

    public float ValidateValue(AudioEvent audioEvent)
    {
        return audioEvent[(int)valueMapper];
    }

    public Color ValidateColor(AudioEvent audioEvent)

    {
        float[] bounds = GetParamLimit(frictionMapper);
        float param = audioEvent[(int)frictionMapper];

        return Color.Lerp(Color.red, Color.blue, Map(param, bounds[0], bounds[1],0.0f,1.0f));
    }

}
