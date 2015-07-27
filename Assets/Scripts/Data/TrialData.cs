using UnityEngine;
using System.Collections;

public class TrialData {

	// TrialData
	//     Contains list/array of FitnessComponentData,
	//     -As well as The Raw Score for this Trial (after combining all the fitnessComponentData's
	//     -And the Modified Score for the Trial (after adjusting by power & weight) -- Do I want in 0-1 range, or actual value?
	//     -And Number of Fitness Components
	public FitnessComponentData[] fitnessComponentDataArray;
	public float rawValueTotal;
	public float weightedValueTotal;
	public float rawValueAvg;
	public float weightedValueAvg;
	
	public TrialData(int numFitnessComponents) {  // number of fitness components for this player's current Trial
		fitnessComponentDataArray = new FitnessComponentData[numFitnessComponents];
	}
}
