using UnityEngine;
using System.Collections;

public class AgentData {

	// AgentData
	//     Contains List of TrialData,
	//     -And Number of Trials
	public TrialData[] trialDataArray;
	public float rawValueTotal = 0f;
	public float weightedValueTotal = 0f;
	public float rawValueAvg = 0f; // Average of all Trials
	public float weightedValueAvg = 0f;
	
	public AgentData(int numTrials) {
		trialDataArray = new TrialData[numTrials];
	}
}
