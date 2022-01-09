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
    public float this[int index]
    { //SPECIAL PROPERTY INDEXERS
        get
        {
            switch (index) { 
                case 0: return rootMeanSquare;
                case 1: return pitch;
                case 2: return peakEnergy;
                case 3: return zeroCrossingRate;
                case 4: return spectralCentroid;
                case 5: return spectralCrest;
                case 6: return spectralFlatness;
                case 7: return spectralRolloff;
                case 8: return spectralKurtosis;
                case 9: return energyDifference;
                case 10: return spectralDifference;
                case 11: return spectralDifferenceHWR;
                case 12: return complexSpectralDifference;
                case 13: return highFrequencyContent;

                default:
                    return 0;
            }

        }
            set
        {

            switch (index)
            {
                case 0: rootMeanSquare = value;break;
                case 1: pitch = value; break; ;
                case 2: peakEnergy = value; break; ;
                case 3: zeroCrossingRate = value; break; ;
                case 4: spectralCentroid = value; break; ;
                case 5: spectralCrest = value; break; ;
                case 6: spectralFlatness = value; break; ;
                case 7: spectralRolloff = value; break; ;
                case 8: spectralKurtosis = value; break; ;
                case 9: energyDifference = value; break; ;
                case 10: spectralDifference = value; break; ;
                case 11: spectralDifferenceHWR = value; break; ;
                case 12: complexSpectralDifference = value; break; ;
                case 13: highFrequencyContent = value; break; ;

                default:
                    break;

            }
        }
    }
}
