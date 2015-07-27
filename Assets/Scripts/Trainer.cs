using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Trainer {

	#region Variables
	public bool debugFunctionCalls = false; // turns debug messages on/off
	public GameController gameControllerRef;

	// STATES:
	private bool isPlaying = false;  // When true, the mini-games are being played by the agents, when false, game is paused
	public bool IsPlaying {
		get {
			return isPlaying;
		}
		set {}
	}
	private bool crossoverOn = true; // Note that this refers to the "Training" button on the menuBar, names may have to change in the future.
	public bool CrossoverOn {
		get {
			return crossoverOn;
		}
		set {}
	}
	private bool fastModeOn = false; // simulate and watch in real-time, or crunch the numbers as fast as possible behind-the-scenes.
	public bool FastModeOn {
		get {
			return fastModeOn;
		}
		set {}
	}
	private bool manualOverrideOn = false;  // With this true, the user can play against the AI controllers
	public bool ManualOverrideOn {
		get {
			return manualOverrideOn;
		}
		set {}
	}

	// When true (needs to be paused, between gens), any pending values in the specified panel will be used to update their corresponding data:
	public bool applyPanelPlayers = false;   // basically, true means that the current state of the system allows changes to each of these panel's data
	public bool applyPanelPopulation = false;
	public bool applyPanelCrossover = true; 
	public bool applyPanelTrials = true; 
	public bool applyPanelFitness = false; 
	public bool applyPanelMiniGame = false; 

	private int maxPlayers = 4;
	public int MaxPlayers {
		get {
			return maxPlayers;
		}
		set {}
	}
	private int numPlayers = 1;
	public int NumPlayers {
		get {
			return numPlayers;
		}
		set {
			numPlayers = value;
		}
	}
	private int curPlayer = 1;
	public int CurPlayer {
		get {
			return curPlayer;
		}
		set {
			curPlayer = value;
		}
	}

	// PLAYER LIST:
	private List<Player> playerList;  // This holds the instanced Player classes, depending on numPlayers.  This holds all the information for training and populations!
	public List<Player> PlayerList {
		get {
			return playerList;
		}
		set {
			playerList = value;
		}
	}

	public float playbackSpeed = 1.0f;


	public bool hasValidPlayerList = false;


	// Playing Game States! 
	private int playingCurGeneration = 0;
	private int playingCurTrialIndex;
	private int playingNumTrials = 0; // keep track of highest index Trial for any player
	private int playingCurPlayer;
	private int playingNumPlayers = 0;
	private int playingCurTrialRound;
	private int playingNumTrialRounds = 0;
	private int playingCurAgent;
	private int playingNumAgents = 0;
	private int playingCurMiniGameTimeStep;
	private int playingNumMiniGameTimeSteps = 0;

	public bool betweenGenerations = true;

	#endregion



	// Constructor Method
	public Trainer() {
		DebugBot.DebugFunctionCall("Trainer; Trainer() Constructor!; ", debugFunctionCalls);
		//CheckPanelApplyCriteria();
		InitializePlayerList();
	}

	public void InitializePlayerList() {
		playerList = new List<Player>();
		for(int i = 0; i < numPlayers; i++) {
			Player newPlayer = new Player();
			playerList.Add (newPlayer);
		}
		hasValidPlayerList = true;
		DebugBot.DebugFunctionCall("Trainer; InitializePlayerList; " + playerList.ToString(), debugFunctionCalls);
	}

	public void AddPlayer() {
		Player newPlayer = new Player();
		newPlayer.InitializeNewPopulation();
		playerList.Add (newPlayer);
		UpdatePlayingNumPlayers();
	}

	public void TogglePlayPause() { // !!!!!!!! NEEDS RE-FACTORING!!!
		Debug.Log ("Toggle Play/Pause!");
		PlayerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager.miniGameInstance.PrintTestTargetBallPos();
		isPlaying = !isPlaying;
		if(isPlaying) {
			Time.timeScale = playbackSpeed;
		}
		else {
			Time.timeScale = 0.0f;
		}
		if(betweenGenerations == true && isPlaying == true) { // !!!! Re-Evaluate this way of zero-ing!
			playingCurTrialIndex = 0;
			playingCurPlayer = 0;
			playingCurTrialRound = 0;
			playingCurAgent = 0;
			playingCurMiniGameTimeStep = 0;
			UpdatePlayingNumTimeSteps();
			UpdatePlayingNumTrialRounds();
			UpdatePlayingNumAgents();
			UpdatePlayingNumTrials(); 
			UpdatePlayingNumPlayers();
			//if(!playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager.miniGameInstance.piecesBuilt) { // if current miniGame instance doesn't have pieces built
			//	DebugBot.DebugFunctionCall("PIECES BUILT!! ", debugFunctionCalls);
			//	playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager.miniGameInstance.BuildGamePieces();
			//}
			for(int p = 0; p < playerList.Count; p++) {
				playerList[p].dataManager.InitializeNewGenerationDataArrays(playingCurGeneration);
			}
			playerList[playingCurPlayer].graphKing.BuildTexturesFitnessPerGen(playerList[playingCurPlayer]);
			//playerList[playingCurPlayer].graphKing.BuildTexturesHistoryPerGen(playerList[playingCurPlayer]);
			playerList[playingCurPlayer].graphKing.BuildTexturesCurAgentPerAgent(playerList[playingCurPlayer], playingCurAgent);
			gameControllerRef.trainerUI.SetAllPanelsFromTrainerData();
		}
	}

	public void ToggleTraining() {
		crossoverOn = !crossoverOn;
	}

	public void ToggleFastMode() {
		fastModeOn = !fastModeOn;
	}

	public void PlayRealTimeStep() {
		//DebugBot.DebugFunctionCall("Trainer; PlayRealTimeStep; " + playingCurGeneration + ", " + playingCurTrialIndex + "/" + playingNumTrials + ", " + playingCurPlayer + "/" + playingNumPlayers + ", " + playingCurTrialRound + "/" + playingNumTrialRounds + ", " + playingCurAgent + "/" + playingNumAgents + ", " + playingCurMiniGameTimeStep + "/" + playingNumMiniGameTimeSteps, debugFunctionCalls);
		Debug.Log ("PlayRealTimeStep(): Before CalculateOneStep() & UpdatePlayingState()! game TimeStep: " + playingCurMiniGameTimeStep.ToString());
		// !!!!!!!!!!!!!!  NEED TO UPDATE THIS !!!! so that it properly detects when a mini-game instance has changed and deletes unneeded pieces
		//if(!playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager.miniGameInstance.piecesBuilt) { // if current miniGame instance doesn't have pieces built
		//	DebugBot.DebugFunctionCall("PIECES BUILT!! ", debugFunctionCalls);
		//	playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager.miniGameInstance.BuildGamePieces();
		//}
		playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager.miniGameInstance.PrintTestTargetBallPos();
		// Do whatever it is that it does for one frame -- Main play loop -- Tick Brain, Tick MiniGame:
		CalculateOneStep();
		// Increment Time Step!
		playingCurMiniGameTimeStep++;
		// Update playing State
		UpdatePlayingState();
	}

	public void PlayFastModeChunk(int numSteps) {
		int counter = 0;
		while(counter < numSteps) {
			CalculateOneStep();
			counter++;
			playingCurMiniGameTimeStep++;
			UpdatePlayingState();
		}
	}

	#region Fitness Stuff
	private void CombineFitnessScoresEndRound() {
		// Agent X just finished a round of a game -- Figure out its score.

		// Find playingCurMiniGameTimeStep (i.e. how many timesteps the game was played
		int numTimeSteps = playingCurMiniGameTimeStep;
		// Look through Current player's Trial's fitness manager's Fitness Components
		// Loop through fitnessComponents (array or List?)
		FitnessManager fitnessManagerRef = playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].fitnessManager;
		//Debug.Log ("gameRoundFitnessScoreBEFORE: " + fitnessManagerRef.agentFitnessScores[playingCurAgent].ToString());
		int numGameFitComponents = fitnessManagerRef.gameFitnessComponentList.Count;
		int numBrainFitComponents = fitnessManagerRef.brainFitnessComponentList.Count;
		int numSelectedComponents = 0;
		float gameRoundFitnessScoreRaw = 0f;
		float gameRoundFitnessScoreWeighted = 0f;
		for(int i = 0; i < numGameFitComponents; i++) {
			if(fitnessManagerRef.gameFitnessComponentList[i].on) {
				//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
				// Pass each fitnessManagerRef to DataManager?....eh...
				// How Best to store data and calculate avg's & stuff with data at same time? 
				// And know when/if to Expand Arrays/Lists/Re-write data?
				// Maybe just pass each Score as well as curPlayingData:
				//     whenever agent finishes a round, 
				//     agent finishes a trial, 
				//     player finishes all trials, 
				//     and nextGeneration.
				// ...By Passing to a function in DataManager for updating at each of those times.
				// Then the dataManager takes care of determining if it needs to increase size of array, average agent scores, or do other calculations
				// Once dataManager is done with its housekeeping, tap the current player's GraphKing and update the textures/shader attributes
				// 


				//Debug.Log ("i: " + i.ToString() + ", numFitComponents: " + numGameFitComponents.ToString() + ", componentScore[0]: " + fitnessManagerRef.gameFitnessComponentList[i].componentScore[0]);
			// Get raw values from fitness Manager ( Separate Array that holds only selected fitness components )
			// divide raw values (one per fitness component ) by timesteps to get average value -- need to add a flag on fitness component for averaging it?
				float score = 0f;
				score = fitnessManagerRef.gameFitnessComponentList[i].componentScore[0];
				//Debug.Log ("gameRoundFitnessScoreRAW: " + score.ToString());
				if(fitnessManagerRef.gameFitnessComponentList[i].divideByTimeSteps) {
					score /= (float)numTimeSteps;
				}
				// Adjust raw averaged scores by biggerIsBetter (invert), power and weight.
				if(!fitnessManagerRef.gameFitnessComponentList[i].bigIsBetter) {  // invert values
					score = 1f - score;
				}

				//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
				// Pass or Set RAW fitness score for this component onto DataManager for current Player, Trial, Agent, & FitnessComponent
				gameRoundFitnessScoreRaw += score;
				playerList[playingCurPlayer].dataManager.StoreGameRoundRawData(score, playingCurGeneration, playingCurAgent, playingCurTrialIndex, numSelectedComponents, playingCurTrialRound);

				score = Mathf.Pow (score, fitnessManagerRef.gameFitnessComponentList[i].power);
				score *= fitnessManagerRef.gameFitnessComponentList[i].weight;

				playerList[playingCurPlayer].dataManager.StoreGameRoundWeightedData(score, playingCurGeneration, playingCurAgent, playingCurTrialIndex, numSelectedComponents, playingCurTrialRound);
				//gameRoundFitnessScoreWeighted += score; // OBSOLETE
				//Debug.Log ("gameRoundFitnessScoreMiddle: " + gameRoundFitnessScore.ToString());
				numSelectedComponents++;
			}		
		}
		for(int i = 0; i < numBrainFitComponents; i++) {
			if(fitnessManagerRef.brainFitnessComponentList[i].on) {
				//Debug.Log ("i: " + i.ToString() + ", numFitComponents: " + numGameFitComponents.ToString() + ", componentScore[0]: " + fitnessManagerRef.gameFitnessComponentList[i].componentScore[0]);
				// Get raw values from fitness Manager ( Separate Array that holds only selected fitness components )
				// divide raw values (one per fitness component ) by timesteps to get average value -- need to add a flag on fitness component for averaging it?
				float score = 0f;
				score = fitnessManagerRef.brainFitnessComponentList[i].componentScore[0];
				if(fitnessManagerRef.brainFitnessComponentList[i].divideByTimeSteps) {
					score /= (float)numTimeSteps;
				}
				// Adjust raw averaged scores by biggerIsBetter (invert), power and weight.
				if(!fitnessManagerRef.brainFitnessComponentList[i].bigIsBetter) {  // invert values
					score = 1f - score;
				}
				//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
				// Pass or Set RAW fitness score for this component onto DataManager for current Player, Trial, Agent, & FitnessComponent
				gameRoundFitnessScoreRaw += score;
				playerList[playingCurPlayer].dataManager.StoreGameRoundRawData(score, playingCurGeneration, playingCurAgent, playingCurTrialIndex, numSelectedComponents, playingCurTrialRound);

				score = Mathf.Pow (score, fitnessManagerRef.brainFitnessComponentList[i].power);
				score *= fitnessManagerRef.brainFitnessComponentList[i].weight;

				playerList[playingCurPlayer].dataManager.StoreGameRoundWeightedData(score, playingCurGeneration, playingCurAgent, playingCurTrialIndex, numSelectedComponents, playingCurTrialRound);
				//gameRoundFitnessScoreWeighted += score;
				numSelectedComponents++;
			}
		}
		// Combine All Normalized Scores into a single fitness value for this GameRound.
		//fitnessManagerRef.agentFitnessScoresRaw[playingCurAgent] += gameRoundFitnessScoreRaw/(float)numSelectedComponents; // OBSOLETE
		//fitnessManagerRef.agentFitnessScoresWeighted[playingCurAgent] += gameRoundFitnessScoreWeighted/(float)numSelectedComponents; // OBSOLETE

		//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// Should the fitness score live inside each agent, or in the DataManager for each player, or both/neither?
		// -- Maybe calculate all the scores in Data-Manager, and then just set the Agent's fitness from dataManager --
		// -- Agent storing its own fitness value is currently important to the way Crossover is handled.
		// -- Maybe in the future I could remove agent fitness variable and overhaul Crossover to use dataManager, but this doesn't seem to be too heavy
		// Do I need to store actual fitness score values in fitnessManagerRef? or bypass it and use DataManager?

		// Add this Score to Agent's total fitness score, stored in Fitness Manager for that Trial.
		//Debug.Log ("gameRoundFitnessScoreAFTER: " + fitnessManagerRef.agentFitnessScores[playingCurAgent].ToString());
	}

	private void CombineFitnessScoresGameRounds(int currentAgent) {
		// Agent X just finished all the rounds of its current Trial
		int curAgent = currentAgent;
		int numRounds = playingNumTrialRounds;

		playerList[playingCurPlayer].dataManager.AverageTrialDataPerFitnessComponent(playingCurGeneration, playingCurAgent, playingCurTrialIndex);
		//++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
		// FitnessManager OBSOLETE for this part --:::
		// it will be replaced by DataManager, while fitnessManager will only handle current fitness values and is re-used
		// while the values are permanently stored in the dataManager

		// Check number of GameRounds, get currentAgent number that just finished
		//FitnessManager fitnessManagerRef = playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].fitnessManager;
		// Look at current Trial's Fitness Manager, get currentAgent's score and divide by number of GameRounds to get average agent Score for one Trial
		//fitnessManagerRef.agentFitnessScoresRaw[playingCurAgent] /= (float)playingNumTrialRounds;
		//fitnessManagerRef.agentFitnessScoresWeighted[playingCurAgent] /= (float)playingNumTrialRounds;
		// Game Rounds are identical playthroughs so I can just use a simple un-weighted average here
		// Store this value in Fitness manager for that Trial

		// Sets agent fitness value for this trial -- OBSOLETE?
		// THIS IS AN INTERMEDIATE STEP:
		//playerList[playingCurPlayer].masterPopulation.masterAgentArray[playingCurAgent].fitnessScore = fitnessManagerRef.agentFitnessScores[playingCurAgent];

		//Debug.Log ("AgentTrialFitnessScore: " + fitnessManagerRef.agentFitnessScores[playingCurAgent].ToString());
	}

	private void CombineFitnessScoresAllTrials() {
		// Just before NextGeneration, need to tally up and combine scores from each of the Trials for each Player's Agents

		// Loop through Players
		for(int p = 0; p < playingNumPlayers; p++) {
		//		Get Population Size,
			int populationSize = playerList[p].masterPopulation.populationMaxSize;
		//	
			string agentFitnessTotals = "";
			for(int a = 0; a < populationSize; a++) {
				playerList[playingCurPlayer].dataManager.AverageTrialScoresPerAgent(playingCurGeneration, a);
		
				// Store Agent SCORE:
				//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
				// Should the fitness score live inside each agent, or in the DataManager for each player, or both/neither?
				// -- Maybe calculate all the scores in Data-Manager, and then just set the Agent's fitness from dataManager --
				// -- Agent storing its own fitness value is currently important to the way Crossover is handled.
				// -- Maybe in the future I could remove agent fitness variable and overhaul Crossover to use dataManager
				//playerList[p].masterPopulation.masterAgentArray[a].fitnessScore = agentScoreWeighted; //
				playerList[p].masterPopulation.masterAgentArray[a].fitnessScore = 
					playerList[playingCurPlayer].dataManager.generationDataList[playingCurGeneration].agentDataArray[a].weightedValueAvg;

				// !@!#$#!@#$!@$#$#!@$#!@###$~!!  ----STORE DATA IN DATAMANAGER HERE!!----  !@#$%!#$^%^*&^#$%!@#!
				agentFitnessTotals += "Agent# " + a.ToString() + " Score: " + playerList[p].masterPopulation.masterAgentArray[a].fitnessScore.ToString() + ", Raw: " + playerList[playingCurPlayer].dataManager.generationDataList[playingCurGeneration].agentDataArray[a].rawValueAvg + "; ";
				//Debug.Log ("agentScore: " + playerList[p].masterPopulation.masterAgentArray[a].fitnessScore.ToString());
			}

			playerList[playingCurPlayer].dataManager.AverageTrialScoresPerGen(playingCurGeneration);
			//Debug.Log (agentFitnessTotals);
			playerList[p].graphKing.BuildTexturesFitnessPerGen(playerList[p]);
			playerList[p].graphKing.BuildTexturesHistoryPerGen(playerList[p]);
			//playerList[p].graphKing.BuildTexturesFitnessBasic(playerList[p].dataManager);
		}
	}
	#endregion

	private void UpdatePlayingState() {  // increments current state variables, keeping track of when each should roll-over into the next round
		//Debug.Log ("UpdatePlayingState() playingCurGameStep: " + playingCurMiniGameTimeStep.ToString());

		MiniGameManager currentGameManager = PlayerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager;  // <-- to help readability
		betweenGenerations = false;
		//Debug.Log ("UpdatePlayingState - curGameTimeSteps: " + playingCurMiniGameTimeStep.ToString());
		if(playingCurMiniGameTimeStep >= playingNumMiniGameTimeSteps) {  // hit time limit of minigame
			CombineFitnessScoresEndRound();  // divides accumulated fitness score by time-steps, to keep it in 0-1 range and stores it in AgentScoresArray
			//playerList[0].masterPopulation.masterAgentArray[playingCurAgent].fitnessScore = currentGameManager.miniGameInstance.fitnessScore;
			playingCurMiniGameTimeStep = 0;
			playingCurTrialRound++;  // Same agent, next round playthrough

			//Debug.Log ("UpdatePlayingState - curGameTimeSteps: " + playingCurMiniGameTimeStep.ToString());
			// v v v THIS might cause problems!
			currentGameManager.miniGameInstance.DestroyGamePieceComponents();
			currentGameManager.miniGameInstance.Reset();
			currentGameManager.miniGameInstance.SetGamePieceTransformsFromData();  // set the GameObject.transform (NOT RigidBody) based on gameData
			currentGameManager.miniGameInstance.BuildGamePieceComponents(); // create RigidBody and HingeJoint etc. components on the empty GameObjects

			if(playingCurTrialRound >= playingNumTrialRounds) {   // finished all rounds of current Trial for current Agent
				CombineFitnessScoresGameRounds(playingCurAgent);  // combines the raw scores from all game rounds to get the agent's Fitness score for that TrialIndex
				playingCurTrialRound = 0;

				playingCurAgent++;  // Move on to next agent

				if(playingCurAgent >= playingNumAgents) {
					playingCurAgent = 0;

					playingCurPlayer++;  // Now that the current player has changed, update how many trial rounds for new player
					//DebugBot.DebugFunctionCall("Trainer; UpdatePlayingState; PLAYERS!!: " + playingNumPlayers.ToString(), true);

					if(playingCurPlayer >= playingNumPlayers) {  // finished all active Players for this Trial Index
						//DebugBot.DebugFunctionCall("Trainer; UpdatePlayingState; PLAYERS!! playingCurPlayer >= playingNumPlayers: " + playingNumPlayers.ToString(), true);
						playingCurPlayer = 0;
						playingCurTrialIndex++;  // Move on to next Trial Index


						if(playingCurTrialIndex >= playingNumTrials) {
							CombineFitnessScoresAllTrials(); // combines the scores for all agents' TrialIndex's to get final Agent Fitness Scores
							playingCurTrialIndex = 0;
							betweenGenerations = true;
							//=========================
							// THE NEXT GENERATION!!!!!
							//=========================
							NextGenerationStart(); // Pending Changes Applied & Crossover
							UpdatePlayingNumTimeSteps();
							UpdatePlayingNumTrialRounds();
							UpdatePlayingNumAgents();
							UpdatePlayingNumPlayers();
							UpdatePlayingNumTrials();

							playingCurGeneration++;
							// Initialize dataManager structs for new generation:
							for(int p = 0; p < playerList.Count; p++) {
								playerList[p].dataManager.InitializeNewGenerationDataArrays(playingCurGeneration);
							}
							//DebugBot.DebugFunctionCall("Trainer; UpdatePlayingState; " + playingCurGeneration + ", " + playingCurTrialIndex + "/" + playingNumTrials + ", " + playingCurPlayer + "/" + playingNumPlayers + ", " + playingCurTrialRound + "/" + playingNumTrialRounds + ", " + playingCurAgent + "/" + playingNumAgents + ", " + playingCurMiniGameTimeStep + "/" + playingNumMiniGameTimeSteps, debugFunctionCalls);
						}
						else{ // reset player back to 0, incremented Trial
							UpdatePlayingNumTrialRounds();  // new player means potentially different number of trial rounds
							UpdatePlayingNumAgents();
							UpdatePlayingNumTimeSteps();
						}
					}
					else {  // reset current agent (finished all trial Plays, next player or next Trial Index:
						UpdatePlayingNumTimeSteps(); // Might not be correct placement....
						UpdatePlayingNumTrialRounds();
					}
				}
				else { // current Agent is incremented: 
					if(!fastModeOn) {
						// UPDATE texture for current agent brain diagram with genome for new agent
						playerList[playingCurPlayer].graphKing.BuildTexturesCurAgentPerAgent(playerList[playingCurPlayer], playingCurAgent);
					}
					UpdatePlayingNumTimeSteps();
				}
			}
		}
	}

	public void UpdatePlayingNumTimeSteps() {
		playingNumMiniGameTimeSteps = PlayerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].maxEvaluationTimeSteps;
	}

	public void UpdatePlayingNumTrialRounds() {
		playingNumTrialRounds = PlayerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].numberOfPlays;
	}

	public void UpdatePlayingNumPlayers() {
		//DebugBot.DebugFunctionCall("Trainer; UpdatePlayingNumPlayers; " + playingNumPlayers.ToString(), true);
		playingNumPlayers = playerList.Count;
		//DebugBot.DebugFunctionCall("Trainer; UpdatePlayingNumPlayers; " + playingNumPlayers.ToString(), true);
		// Might keep the number of players static and just use a check to see if there is an active Trial at the specified Index for the cur Player
	}

	public void UpdatePlayingNumAgents() {  // Check how many agents in the current population
		playingNumAgents = playerList[playingCurPlayer].masterPopulation.populationMaxSize;
	}

	public void UpdatePlayingNumTrials() { // highest numbered Trial Index for any player

		int highestNumTrials = 0;
		for(int i = 0; i < numPlayers; i++) { // iterate through all active players
			for(int j = 0; j < PlayerList[i].masterTrialsList.Count; j++) {  // Count how many Trials each player has, keep track of highest number;
				if(j > highestNumTrials) {
					highestNumTrials = j;
				}
			}
		}
		//highestNumTrials -= 1;  // To account for the empty NONE Trial at end of each TrialsList
		//DebugBot.DebugFunctionCall("Trainer; UpdatePlayingNumTrials; " + playingNumTrials.ToString(), true);
		playingNumTrials = highestNumTrials; // set number of Trials to iterate through
	}

	public void CalculateOneStep() {  // This is where the magic happens -- feeds game data to current Brain and Tick's the current mini-game
		// Based on current playingState data, looks up current Trial Index,
		// Look up current Player being evaluated, and see what Mini-game type is selected for that player.
		// If the mini-game is a multi-player game, evaluate all players for that trial simultaneously (SPLIT CODE PATH)
		// Else it's a normal day at the office:  Evaluate time step for current playingTrial index, current playingPlayer index
		// Send gameStateData (InputChannels List) through inputArray to the current Agent's Brain instance.

		// v v v TO BE REPLACED!!
		BrainBase currentBrain = PlayerList[playingCurPlayer].masterPopulation.masterAgentArray[playingCurAgent].brain;  // <-- to help readability
		MiniGameManager currentGameManager = PlayerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager;  // <-- to help readability

		//Debug.Log ("CalculateOneStep() playingCurGameStep: " + playingCurMiniGameTimeStep.ToString());

		// ^ ^ ^ ^ PhysX Simulation step from last CalculateOneStep() happens before the following code in this Function:

		if(!currentGameManager.miniGameInstance.gameInitialized) { // If the miniGame exists but has not been Initialized -- very first timeStep of whole training session
			// Then Initialize the game data:
			currentGameManager.miniGameInstance.Reset();
			currentGameManager.miniGameInstance.InstantiateGamePieces();  // first time Game has been Reset, so instantiateGamePieces
			currentGameManager.miniGameInstance.SetGamePieceTransformsFromData();  // set the GameObject.transform (NOT RigidBody) based on gameData
			currentGameManager.miniGameInstance.BuildGamePieceComponents(); // create RigidBody and HingeJoint etc. components on the empty GameObjects
		}

		// If the game Ran a timeStep during the last call of FixedUpdate/CalculateOneStep, and there is pending PhysX Simulation data yet to be integrated:
		if(currentGameManager.miniGameInstance.gameTicked) { 
			//Debug.Log ("CalculateOneStep() playingCurGameStep: " + playingCurMiniGameTimeStep.ToString());
			// This should only update if game Ran before this function call, otherwise it's skipped and goes onto GameSimulation
			currentGameManager.miniGameInstance.UpdateGameStateFromPhysX(); // integrate pending PhysX stateData back into primary gameData
			if(currentGameManager.miniGameInstance.gameEndStateReached) {  // If, after the end of its gameLoop, some endGame conditions have been met:
				// game ended for reason other than timeOut	
			}
			// =================================================================================================================================================================
			//      END OF GAME LOOP !!!!
			// =================================================================================================================================================================


			playingCurMiniGameTimeStep++;
			currentGameManager.miniGameInstance.GameTimeStepCompleted();  // sets gameTicked, gameUpdatedFromPhysX to 0, increments internal curGameTimeStep

			UpdatePlayingState ();
			// This should check to see if the next gameTimeStep results in a rollover,
			// ... in which case it handles the incrementing of stateData and if a game needs to be Reset, resets curTimeStep to 0
		}

		// Check if this mini-game tests agents in isolation, or drops them all in the same arena together
		if(true) {  //if(miniGame testsIsolated) { If it's isolated, then it can use the multi-agent parallel independent testing:
			// Check how many agents to be tested (how many mini-Game instance -- agent pairs)
			// For ( agents to be tested ) 
			for(int i = 0; i < 1; i++) {
				// Get current agent instance being tested
				//BrainBase currentBrain = PlayerList[playingCurPlayer].masterPopulation.masterAgentArray[playingCurAgent].brain;
				// Get current miniGame instance:

				// Check if miniGame has all pieces built: Or do this somewhere else???
				//if(!currentGameManager.miniGameInstance.piecesBuilt) {
					//currentGameManager.miniGameInstance.BuildGamePieces();
				//}

				// Check if miniGame/agent has finished the game round early, before max time
				//if(playingCurMiniGameTimeStep == (playingNumMiniGameTimeSteps - 1)) { // if game has ended check for miniGameInstance.finished or equivalent
					// Take care of it, fitness + any needed cleanup
					//currentGameManager.miniGameInstance.DeleteGamePieces();
					// Delete physics Components to prevent any physX simulations between this and the next (Reset) Frames
				//}
				//else { // Game is still going on:
				// Set position & velocity variables from the physX objects in the game from between Frames, since the physics sim runs directly after FixedUpdate():
				//if(playingCurMiniGameTimeStep != 0) {  // if it is the very first timeStep of the game, don't grab PhysX simulation info
					//currentGameManager.miniGameInstance.UpdateGameStateFromPhysX();
				//}					
				
				// Now with the updated values for position/velocity etc., pass input values into brains
				currentBrain.BrainMasterFunction(ref currentGameManager.brainInput, ref currentGameManager.brainOutput);
				// Run the game for one timeStep: (Note that this will only modify non-physX variables -- the actual movement and physX sim happens just afterward -- so keep that in mind)
				currentGameManager.miniGameInstance.Tick ();

				if(false) {  // if( playbackSpeed is > some threshold value...
					// If it's running too fast, turn off gamePiece render components, certain graphs

				}
				else {
					// turn on Visualizations and graphs:
					currentGameManager.miniGameInstance.VisualizeGameState();
					playerList[playingCurPlayer].graphKing.BuildTexturesCurAgentPerTick(playerList[playingCurPlayer], playerList[playingCurPlayer].masterTrialsList[playingCurTrialIndex].miniGameManager, playingCurAgent);
				}
				//}
			}
			// Update playingState now that multiple agents have been evaluated -- messing up the playingCurAgent
		}
		//else {  // if miniGame tests all agents together:
		//	// Do whatever...
		//}

		// .......................................
		// ... Next is PhysX Simulation (hidden)
	}

	public void NextGenerationStart() {
		gameControllerRef.trainerUI.CheckForAllPendingApply(); // apply any pending edits
		gameControllerRef.trainerUI.panelDataVisScript.InitializePanelWithTrainerData();

		// CROSSOVER!!!!!! ++++++++++++++++++
		if(crossoverOn) {
			for(int p = 0; p < numPlayers; p++) {  // iterate through playerList
				//Debug.Log ("crossover, player" + p.ToString () + ", " + playerList[p].MasterPopulation.brainType.ToString() + playerList[p].masterCupid.tempName);
				playerList[p].masterCupid.PerformCrossover(ref playerList[p].masterPopulation);  

				//for(int t = 0; t < playerList[p].masterTrialsList.Count; t++ ) { // Hacky zero out fitness Scores
					//playerList[p].masterTrialsList[t].fitnessManager = 0f;
				//}
				//for(int m = 0; m < playerList[p].MasterPopulation.masterAgentArray.Length; m++) {
				//	playerList[p].MasterPopulation.masterAgentArray[m].brain.genome.PrintBiases();
				//}
				// looks at player's Population instance and does Crossover based on masterCupid instances settings, and population's size/brain-type
				// Creates a new population and updates the player's masterPopulation to that new Agent population
			}
		}




	}

	public string GetCurrentGamePlayingState() {
		string output = "Current Generation: " + playingCurGeneration + 
						"\nCurrent Trial Index: " + (playingCurTrialIndex + 1) + " / " + playingNumTrials + 
						"\nCurrent Player: " + (playingCurPlayer + 1) + " / " + playingNumPlayers + 
						"\nCurrent Game Play Count: " + (playingCurTrialRound + 1) + " / " + playingNumTrialRounds + 
						"\nCurrent Agent: " + (playingCurAgent + 1) + " / " + playingNumAgents + 
						"\nCurrent Game Time Step: " + (playingCurMiniGameTimeStep + 1) + "/" + playingNumMiniGameTimeSteps;
		return output;
	}

}
