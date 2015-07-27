using UnityEngine;
using System.Collections;

public class FitnessComponent {

	public bool on;
	public bool bigIsBetter;
	public float power;
	public float weight;
	public float[] componentScore; // This keeps track of total accumulated score
	public string componentName;
	public bool divideByTimeSteps;

	public FitnessComponent(ref float[] value, bool active, bool bigBetter, float scorePower, float scoreWeight, string name, bool avg) {
		on = active;
		componentScore = value;
		bigIsBetter = bigBetter;
		componentName = name;
		power = scorePower;
		weight = scoreWeight;
		divideByTimeSteps = avg;
	}
}
