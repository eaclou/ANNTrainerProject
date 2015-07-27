using UnityEngine;
using System.Collections;

public class Trial {
	public bool debugFunctionCalls = false; // turns debug messages on/off

	//public MiniGameManager.MiniGameType gameType;
	public Player playerRef;
	public MiniGameManager miniGameManager;
	public FitnessManager fitnessManager;
	public int numberOfPlays;
	public int maxEvaluationTimeSteps;
	public float power;
	public float weight;

	// Constructor Methods:
	public Trial() {
		DebugBot.DebugFunctionCall("Trial; Trial(); Constructor", debugFunctionCalls);

		miniGameManager = new MiniGameManager(playerRef);
		fitnessManager = new FitnessManager(playerRef);
		//miniGameManager.playerRef = playerRef;
		//miniGameManager.gameType = MiniGameManager.MiniGameType.None;
		//masterMiniGame.Shout ();  // DEBUG METHOD
		numberOfPlays = 1;
		maxEvaluationTimeSteps = 30;
		power = 1f;
		weight = 1f;
	}

	public Trial(Player playerReference) {
		DebugBot.DebugFunctionCall("Trial; Trial(); Constructor", debugFunctionCalls);
		playerRef = playerReference;
		miniGameManager = new MiniGameManager(playerRef);
		fitnessManager = new FitnessManager(playerRef);
		//miniGameManager.playerRef = playerRef;
		//miniGameManager.gameType = MiniGameManager.MiniGameType.None;
		//masterMiniGame.Shout ();  // DEBUG METHOD
		numberOfPlays = 1;
		maxEvaluationTimeSteps = 30;
		power = 1f;
		weight = 1f;
	}

	// TRY TO REMOVE and use this method inside MiniGameManager instance!! ++++++++++++++++++++++++++++++++++++++++++++
	/*public void ChangeMiniGameType(MiniGameManager.MiniGameType newGameType) {
		//masterMiniGame = null;
		miniGameManager.gameType = newGameType;  // Update GameType to new Type!

		// CREATE mini-game instance!  !!!!!!!!!!!!! Come back to this for Improvement Later!!!!!!!
		/*if(newGameType == MiniGame.MiniGameType.None) {
			MiniGame newGameInstance = new MiniGame();
			masterMiniGame = newGameInstance as MiniGame;
			masterMiniGame.Shout ();  // DEBUG METHOD
		}
		else if(newGameType == MiniGame.MiniGameType.MoveRandomDirection1D) {
			MiniGameMoveRandomDirection1D newGameInstance = new MiniGameMoveRandomDirection1D();
			masterMiniGame = newGameInstance as MiniGameMoveRandomDirection1D;
			masterMiniGame.Shout ();  // DEBUG METHOD
		}
		else if(newGameType == MiniGame.MiniGameType.MoveRandomDirection2D) {
			MiniGameMoveRandomDirection2D newGameInstance = new MiniGameMoveRandomDirection2D();
			masterMiniGame = newGameInstance;
			masterMiniGame.Shout ();  // DEBUG METHOD
		}
		else if(newGameType == MiniGame.MiniGameType.MoveRandomDirection3D) {
			MiniGameMoveRandomDirection3D newGameInstance = new MiniGameMoveRandomDirection3D();
			masterMiniGame = newGameInstance;
			masterMiniGame.Shout ();  // DEBUG METHOD
		}*/
		//DebugBot.DebugFunctionCall("Trial; ChangeMiniGameType(); " + miniGameManager.gameType.ToString(), debugFunctionCalls);
	//}
}
