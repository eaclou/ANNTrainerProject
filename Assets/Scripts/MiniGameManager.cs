using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameManager {

	//public List<MiniGameInputChannel> selectedInputChannels = new List<MiniGameInputChannel>();

	public bool debugFunctionCalls = false; // turns debug messages on/off
	public delegate void DelegateMiniGameTick();
	DelegateMiniGameTick delegateMiniGameTick;

	public enum MiniGameType {
		None,
		MoveToTarget1D,
		MoveToTarget2D,
		MoveToTarget3D,
		RobotArmFantasy,
		PhysXTests,
		PhysXWorm
	};

	public Player playerRef;
	// determines class type of miniGameInstance
	public MiniGameType gameType = MiniGameType.None;
	// The actual miniGame instance object:
	public MiniGameBase miniGameInstance;

	// CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	// The list of potential Channels is stored inside the miniGameInstance Object,
	// The arrays of paramater references for the Brain (only consists of the selected channels) lives here in the MiniGameManager
	public float[][] brainInput;  //  Come back to this and see if I should use a BrainInputChannel class instead of float;
	public float[][] brainOutput;  // !! Or if this should live inside the miniGameInstance


	public MiniGameManager(Player playerReference) {
		DebugBot.DebugFunctionCall("MiniGameManager; MiniGameManager(); Constructor", debugFunctionCalls);
		playerRef = playerReference;
		SetMiniGameType(gameType);
		//inputSourceList = new string[0];
		//outputActionsList = new string[0];
	}

	public void SetInputOutputArrays() {
		//DebugBot.DebugFunctionCall("MiniGameManager; SetInputOutputArrays(); " + playerRef.currentTrialForEdit.ToString(), debugFunctionCalls);
		// ++++++ NEW APPROACH !! ++++++++++++++++++++++++++++++++++++++++++++++++
		// re-initialize data arrays in case population has changed settings
		//brainInput = null;
		//brainOutput = null;
		// Prep the input data for the brain by making the array dimensions match the number of input/output Neurons in the current brain:
		brainInput = new float[playerRef.masterPopulation.numInputNodes][];
		brainOutput = new float[playerRef.masterPopulation.numOutputNodes][];
		// Find length of Channel Lists
		int numInputChannels = miniGameInstance.inputChannelsList.Count;
		int numOutputChannels = miniGameInstance.outputChannelsList.Count;
		//DebugBot.DebugFunctionCall("MiniGameManager; SetInputOutputArrays(); inputCount: " + miniGameInstance.inputChannelsList.Count.ToString() + ", outputCount: " + miniGameInstance.outputChannelsList.Count.ToString(), debugFunctionCalls);
		// Loop through original Channel Lists, and if a channel is selected, pass a ref of its value to the next Index in the brainDataArrays
		int currentInputArrayIndex = 0;
		int currentOutputArrayIndex = 0;
		for(int i = 0; i < numInputChannels; i++) {
			//DebugBot.DebugFunctionCall("MiniGameManager; SetInputOutputArrays(); input: " + miniGameInstance.inputChannelsList[i].channelValue[0].ToString(), debugFunctionCalls);
			if(miniGameInstance.inputChannelsList[i].on) {
				brainInput[currentInputArrayIndex] = miniGameInstance.inputChannelsList[i].channelValue; // send reference of channel value to current brainInputArray Index
				//DebugBot.DebugFunctionCall("MiniGameManager; SetInputOutputArrays(); brainInput[i][0]: " + brainInput[currentInputArrayIndex][0].ToString() + ", miniGameChannelValue[0]: " + miniGameInstance.inputChannelsList[i].channelValue[0].ToString(), debugFunctionCalls);
				//brainInput[currentInputArrayIndex][0] -= 333.33f;
				//miniGameInstance.inputChannelsList[i].channelValue[0] += 44.4444f;
				currentInputArrayIndex++; // increment current brainInput Index
			}
		}
		for(int o = 0; o < numOutputChannels; o++) {
			if(miniGameInstance.outputChannelsList[o].on) {
				brainOutput[currentOutputArrayIndex] = miniGameInstance.outputChannelsList[o].channelValue; // send reference of channel value to current brainOutputArray Index
				currentOutputArrayIndex++; // increment current brainOutput Index
			}
		}
		// Fill any remaining indices with value of zero ( this will happen if the brain has more nodes than the number of selected Channels )
		while(currentInputArrayIndex < brainInput.Length) {
			float[] zeroArray = new float[1];
			zeroArray[0] = 0f;
			brainInput[currentInputArrayIndex] = zeroArray; // zero out extra indices
			currentInputArrayIndex++; // increment current brainInput Index
		}
		while(currentOutputArrayIndex < brainOutput.Length) {
			float[] zeroArray = new float[1];
			zeroArray[0] = 0f;
			brainOutput[currentOutputArrayIndex] = zeroArray; // zero out extra indices
			currentOutputArrayIndex++; // increment current brainOutput Index
		}



		string debugMessage = "BrainInput: ";
		for(int x = 0; x < brainInput.Length; x++) {
			debugMessage += brainInput[x][0].ToString() + ", ";
		}
		debugMessage += "BrainOutput: ";
		for(int y = 0; y < brainOutput.Length; y++) {
			debugMessage += brainOutput[y][0].ToString() + ", ";
		}
		//DebugBot.DebugFunctionCall("MiniGameManager; SetInputOutputArrays(); brainInput: " + debugMessage, debugFunctionCalls);
		// END NEW APPROACH !! ++++++++++++++++++++++++++++++++++++++++++++++++++++
	}

	public void SetMiniGameType(MiniGameManager.MiniGameType newGameType) {  // Change game type and re-instantiate miniGameInstance
		DebugBot.DebugFunctionCall("Trial; SetMiniGameType(); " + gameType.ToString(), debugFunctionCalls);
		miniGameInstance = null;
		gameType = newGameType;  // Update GameType to new Type!
		
		// CREATE mini-game instance!  !!!!!!!!!!!!! Come back to this for Improvement Later!!!!!!!
		if(newGameType == MiniGameType.None) {
			miniGameInstance = new MiniGameBase();
			//miniGameInstance = newGameInstance as MiniGame;
		}
		else if(newGameType == MiniGameType.MoveToTarget1D) {
			MiniGameMoveToTarget1D newGameInstance = new MiniGameMoveToTarget1D();
			miniGameInstance = newGameInstance as MiniGameMoveToTarget1D;
		}
		else if(newGameType == MiniGameType.MoveToTarget2D) {
			MiniGameMoveToTarget2D newGameInstance = new MiniGameMoveToTarget2D();
			miniGameInstance = newGameInstance;
		}
		else if(newGameType == MiniGameType.MoveToTarget3D) {
			MiniGameMoveToTarget3D newGameInstance = new MiniGameMoveToTarget3D();
			miniGameInstance = newGameInstance;
		}
		else if(newGameType == MiniGameType.RobotArmFantasy) {
			MiniGameRobotArm newGameInstance = new MiniGameRobotArm();
			miniGameInstance = newGameInstance;
		}
		else if(newGameType == MiniGameType.PhysXTests) {
			MiniGamePhysXTests newGameInstance = new MiniGamePhysXTests();
			miniGameInstance = newGameInstance;
		}
		else if(newGameType == MiniGameType.PhysXWorm) {
			MiniGamePhysXWorm newGameInstance = new MiniGamePhysXWorm();
			miniGameInstance = newGameInstance;
		}

		SetInputOutputArrays();
	}
}
