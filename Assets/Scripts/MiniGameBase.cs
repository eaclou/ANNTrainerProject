using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameBase {
	public static bool debugFunctionCalls = true; // turns debug messages on/off

	public List<BrainInputChannel> inputChannelsList;
	public List<BrainOutputChannel> outputChannelsList;

	public List<FitnessComponent> fitnessComponentList;

	public bool gameEndStateReached = false;
	public bool piecesBuilt = false;
	public bool gameInitialized = false;
	public bool gameTicked = false;
	public bool gameUpdatedFromPhysX = false;
	public int gameCurrentTimeStep = 0;



	//public float fitnessScore = 0f;

	// Constructor!!
	public MiniGameBase() {
		// Brain Inputs!:
		inputChannelsList = new List<BrainInputChannel>();
		// Brain Outputs!:
		
		outputChannelsList = new List<BrainOutputChannel>();

		fitnessComponentList = new List<FitnessComponent>();
		
	}

	public virtual void Tick() {  // Runs the mini-game for a single evaluation step.
		DebugBot.DebugFunctionCall("MiniGameBase; Tick();", debugFunctionCalls);
	}

	public virtual void Reset() {

	}

	public virtual void BuildGamePieces() {
		
	}
	
	public virtual void DeleteGamePieces() {
		
	}

	public virtual void VisualizeGameState() {

	}

	public virtual void UpdateGameStateFromPhysX() {

	}

	public virtual void PrintTestTargetBallPos() {  // TEMPORARY!!! DEBUG

	}

	public virtual void InstantiateGamePieces() {
		
	}

	public virtual void UninstantiateGamePieces() {
		
	}

	public virtual void BuildGamePieceComponents() {
		
	}

	public virtual void DestroyGamePieceComponents() {
		
	}

	public virtual void SetGamePieceTransformsFromData() {

	}

	public virtual void GameTimeStepCompleted() {
		
	}
}
