using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FitnessManager {

	public Player playerRef;
	//public Trial trialRef;
	// KEEP THEM SEPARATE and make sure to copy data down to the Mini-game and Brain lsists upon Apply
	public List<FitnessComponent> brainFitnessComponentList; 
	public List<FitnessComponent> gameFitnessComponentList; 

	// EVENTUALLY REMOVE THIS:
	public List<FitnessComponent> masterFitnessCompList;  // This stores all possible fitness components, assembled from brain & miniGame's fitnessCompList 


	// +++++++ MAKE OBSOLETE: transfer to DataManager!
	public float[] agentFitnessScoresRaw; // holds the scores for each Agent on this Trial
	public float[] agentFitnessScoresWeighted; // holds the scores for each Agent on this Trial



	public float[][] fitnessComponentRawScoresArray;  //  Stores the references to selected fitness Components (raw un-averaged scores from miniGame and Brain)


	// Constructor
	public FitnessManager(Player player) {
		playerRef = player;
		agentFitnessScoresRaw = new float[playerRef.masterPopulation.populationMaxSize];
		agentFitnessScoresWeighted = new float[playerRef.masterPopulation.populationMaxSize];
		masterFitnessCompList = new List<FitnessComponent>();
		brainFitnessComponentList = new List<FitnessComponent>();
		gameFitnessComponentList = new List<FitnessComponent>();
		//SetMasterFitnessComponentList();
	}

	public void UpdatePopulationSize() {
		agentFitnessScoresRaw = new float[playerRef.masterPopulation.populationMaxSize];
		agentFitnessScoresWeighted = new float[playerRef.masterPopulation.populationMaxSize];
	}

	public void SetMasterFitnessComponentList() { // Looks at player's brain and Trial's miniGame instance to assemble a combined FitnessComponentList
		//DebugBot.DebugFunctionCall("SetMasterFitnessComponentList: " + playerRef.masterTrialsList.Count.ToString (), true);
		BrainBase currentBrainRef = playerRef.masterPopulation.templateBrain; // OR TEMPLATE BRAIN???
		MiniGameBase currentMiniGameRef = playerRef.masterTrialsList[playerRef.currentTrialForEdit].miniGameManager.miniGameInstance;

		brainFitnessComponentList = currentBrainRef.brainFitnessComponentList;
		gameFitnessComponentList = currentMiniGameRef.fitnessComponentList;


		masterFitnessCompList = new List<FitnessComponent>();
		//if(currentBrainRef.brainFitnessComponentList 
		for(int i = 0; i < currentBrainRef.brainFitnessComponentList.Count; i++) {  // grab fitnessComponents from current brain type
			masterFitnessCompList.Add (currentBrainRef.brainFitnessComponentList[i]);
		}
		for(int j = 0; j < currentMiniGameRef.fitnessComponentList.Count; j++) {
			masterFitnessCompList.Add (currentMiniGameRef.fitnessComponentList[j]);
		}
		//DebugBot.DebugFunctionCall("SetMasterFitnessComponentList: Length: " + masterFitnessCompList.Count.ToString () + ", brainList: " + brainFitnessComponentList.Count.ToString() + ", gameList: " + gameFitnessComponentList.Count.ToString(), true);
	}

	public void SetFitnessComponentScoreArray() {

		int numBrainFitnessComponents = brainFitnessComponentList.Count;
		int numGameFitnessComponents = gameFitnessComponentList.Count;
		int numActiveComponents = 0;
		// Find out how many active channels there are:
		for(int i = 0; i < numBrainFitnessComponents; i++) {
			if(brainFitnessComponentList[i].on) {
				numActiveComponents++;
			}
		}
		for(int i = 0; i < numGameFitnessComponents; i++) {
			if(gameFitnessComponentList[i].on) {
				numActiveComponents++;
			}
		}
		fitnessComponentRawScoresArray = new float[numActiveComponents][];
		int curFitnessCompIndex = 0;
		for(int i = 0; i < numBrainFitnessComponents; i++) {
			if(brainFitnessComponentList[i].on) {
				fitnessComponentRawScoresArray[curFitnessCompIndex] = brainFitnessComponentList[i].componentScore; // send reference of channel value to current brainInputArray Index
				// rawScoresArray now holds a 'live' reference to each fitnessComponent's value
				curFitnessCompIndex++; // increment current brainInput Index
			}
		}
		for(int i = 0; i < numGameFitnessComponents; i++) {
			if(gameFitnessComponentList[i].on) {
				fitnessComponentRawScoresArray[curFitnessCompIndex] = gameFitnessComponentList[i].componentScore; // send reference of channel value to current brainInputArray Index
				// rawScoresArray now holds a 'live' reference to each fitnessComponent's value
				curFitnessCompIndex++; // increment current brainInput Index
			}
		}

		string debugMessage = "FitnessScoreArray: ";
		for(int x = 0; x < fitnessComponentRawScoresArray.Length; x++) {
			debugMessage += fitnessComponentRawScoresArray[x][0].ToString() + ", ";
		}
		//DebugBot.DebugFunctionCall("MiniGameManager; SetInputOutputArrays(); brainInput: " + debugMessage, debugFunctionCalls);
		// END NEW APPROACH !! ++++++++++++++++++++++++++++++++++++++++++++++++++++
	}
}
