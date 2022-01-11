using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 

public struct AudioEvent
{
    public float time;
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
    public float this[int index]
    { //SPECIAL PROPERTY INDEXERS
        get
        {
            switch (index) {
                case 0: return time;
                case 1: return rootMeanSquare;
                case 2: return pitch;
                case 3: return peakEnergy;
                case 4: return zeroCrossingRate;
                case 5: return spectralCentroid;
                case 6: return spectralCrest;
                case 7: return spectralFlatness;
                case 8: return spectralRolloff;
                case 9: return spectralKurtosis;
                case 10: return energyDifference;
                case 11: return spectralDifference;
                case 12: return spectralDifferenceHWR;
                case 13: return complexSpectralDifference;
                case 14: return highFrequencyContent;

                default:
                    return 0;
            }

        }
            set
        {

            switch (index)
            {
                case 0: time = value; break;
                case 1: rootMeanSquare = value;break;
                case 2: pitch = value; break; ;
                case 3: peakEnergy = value; break; ;
                case 4: zeroCrossingRate = value; break; ;
                case 5: spectralCentroid = value; break; ;
                case 6: spectralCrest = value; break; ;
                case 7: spectralFlatness = value; break; ;
                case 8: spectralRolloff = value; break; ;
                case 9: spectralKurtosis = value; break; ;
                case 10: energyDifference = value; break; ;
                case 11: spectralDifference = value; break; ;
                case 12: spectralDifferenceHWR = value; break; ;
                case 13: complexSpectralDifference = value; break; ;
                case 14: highFrequencyContent = value; break; ;

                default:
                    break;

            }
        }
    }
}
