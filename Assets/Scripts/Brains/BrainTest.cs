using UnityEngine;
using System.Collections;

public class BrainTest : BrainBase {

	// Specific Braintype options here (in future)

	public BrainTest() {

	}

	public override void BrainMasterFunction(ref float[][] inputArray, ref float[][] outputArray) {
		string debugMessage = "";
		// hacky 1D version:
		if(inputArray.Length >= 2) {
			if(outputArray.Length >= 1) {
				outputArray[0][0] = (inputArray[1][0] - inputArray[0][0]) * 0.1f;
			}
		}
		// hacky 2D version:
		if(inputArray.Length >= 4) {
			if(outputArray.Length >= 2) {
				outputArray[0][0] = (inputArray[2][0] - inputArray[0][0]) * 0.1f;
				outputArray[1][0] = (inputArray[3][0] - inputArray[1][0]) * 0.1f;
			}
		}


		/*for(int i = 0; i < outputArray.Length; i++) {
			outputArray[i][0] *= 0.998f;
			debugMessage += outputArray[i][0].ToString() + ", ";
		}
		Debug.Log("BrainTest: BrainMasterFunction! " + debugMessage + outputArray.Length.ToString ());
		*/
	}
	
	public override void CopyBrainSettingsFrom(BrainBase sourceBrain) {
		
	}
}
