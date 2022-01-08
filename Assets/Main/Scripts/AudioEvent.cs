using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AudioEvent
{
 	public float rootMeanSquare;
	public float pitch;
	public float peakEnergy;
	public float zeroCrossingRate;
	public float spectralCentroid;
	public float spectralCrest;
	public float spectralFlatness;
	public float spectralRolloff;
	public float spectralKurtosis;
	public float energyDifference;
	public float spectralDifference;
	public float spectralDifferenceHWR;
	public float complexSpectralDifference;
	public float highFrequencyContent;
}
