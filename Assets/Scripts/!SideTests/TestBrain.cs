using UnityEngine;
using System.Collections;

public class TestBrain {
	
	// Specific Braintype options here (in future)
	
	public TestBrain() {
		
	}
	
	public void TestBrainFunction(float[][] inputArray, ref float[][] outputArray) {
		string debugMessage = "";
		for(int i = 0; i < outputArray.Length; i++) {
			outputArray[i][0] += 1.37f;
			debugMessage += outputArray[i][0].ToString() + ", ";
		}
		Debug.Log("TestBrain: TestBrainFunction! " + debugMessage + outputArray.Length.ToString());
	}
}

