using UnityEngine;
using System.Collections;

public class FitnessComponentData {

	// FitnessComponentData
	//     Contains Both Raw and Modified Scores for this component (again, 0-1, or percentage of total weight, actual value?
	public float rawValueTotal = 0f;
	public float weightedValueTotal = 0f;
	public float rawValueAvg = 0f;
	public float weightedValueAvg = 0f;
	public float[] rawValuesArray;
	public float[] weightedValuesArray;
	
	public FitnessComponentData(int numGameRounds) {
		rawValuesArray = new float[numGameRounds];
		weightedValuesArray = new float[numGameRounds];
	}
}
