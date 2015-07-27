using UnityEngine;
using System.Collections;

public class TestGameManager {

	public TestTrainer testTrainerRef;

	public TestGame testGameInstance;
	
	// CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	// The list of potential Channels is stored inside the miniGameInstance Object,
	// The arrays of paramater references for the Brain (only consists of the selected channels) lives here in the MiniGameManager
	public float[][] brainInput;  //  Come back to this and see if I should use a BrainInputChannel class instead of float;
	public float[][] brainOutput;  // !! Or if this should live inside the miniGameInstance

	public TestGameManager(TestTrainer testTrainer) {
		testTrainerRef = testTrainer;
		testGameInstance = new TestGame();
		SetInputOutputArrays();
	}

	public void SetInputOutputArrays() {

		// re-initialize data arrays in case population has changed settings
		brainInput = null;
		brainOutput = null;
		// Prep the input data for the brain by making the array dimensions match the number of input/output Neurons in the current brain:
		brainInput = new float[testTrainerRef.numInputNodes][];
		brainOutput = new float[testTrainerRef.numOutputNodes][];
		//for(int bic = 0; bic < testTrainerRef.numInputNodes; bic++) {
		//
		//}
		// Find length of Channel Lists
		int numInputChannels = testGameInstance.inputChannelsList.Count;
		int numOutputChannels = testGameInstance.outputChannelsList.Count;
		// Loop through original Channel Lists, and if a channel is selected, pass a ref of its value to the next Index in the brainDataArrays
		int currentInputArrayIndex = 0;
		int currentOutputArrayIndex = 0;
		for(int i = 0; i < numInputChannels; i++) {
			if(testGameInstance.inputChannelsList[i].on) {
				brainInput[currentInputArrayIndex] = testGameInstance.inputChannelsList[i].channelValue; // send reference of channel value to current brainInputArray Index
				currentInputArrayIndex++; // increment current brainInput Index
			}
		}
		for(int o = 0; o < numOutputChannels; o++) {
			if(testGameInstance.outputChannelsList[o].on) {
				brainOutput[currentOutputArrayIndex] = testGameInstance.outputChannelsList[o].channelValue; // send reference of channel value to current brainOutputArray Index
				currentOutputArrayIndex++; // increment current brainOutput Index
			}
		}
		// Fill any remaining indices with value of zero ( this will happen if the brain has more nodes than the number of selected Channels )
		while(currentInputArrayIndex < testTrainerRef.numInputNodes) {
			float[] zeroArray = new float[1];
			zeroArray[0] = 0f;
			brainInput[currentInputArrayIndex] = zeroArray; // zero out extra indices
			currentInputArrayIndex++; // increment current brainInput Index
		}
		while(currentOutputArrayIndex < testTrainerRef.numOutputNodes) {
			float[] zeroArray = new float[1];
			zeroArray[0] = 0f;
			brainOutput[currentOutputArrayIndex] = zeroArray; // zero out extra indices
			currentOutputArrayIndex++; // increment current brainOutput Index
		}
		/*
		string debugMessage = "BrainInput: ";
		for(int x = 0; x < brainInput.Length; x++) {
			debugMessage += brainInput[x][0].ToString() + ", ";
		}
		debugMessage += "BrainOutput: ";
		for(int y = 0; y < brainOutput.Length; y++) {
			debugMessage += brainOutput[y][0].ToString() + ", ";
		}
		Debug.Log ("TestGameManager: " + debugMessage);
		*/
	}
}
