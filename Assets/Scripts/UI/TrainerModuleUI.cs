using UnityEngine;
using System.Collections;

public class TrainerModuleUI : MonoBehaviour {

	#region Public and Private Data:

	public bool debugFunctionCalls = false;

	//private int numPlayers = 1; // waiting For REMOVAL!
	//private int maxPlayers = 2;
	//private int curPlayer = 1;

	public Color defaultBGColor = new Color(0.8f, 0.8f, 0.8f);

	// Panels:
	public GameObject panelMenu;
	public GameObject panelDataView;
	public GameObject panelPopulation;
	public GameObject panelNewPopulation;
	public GameObject panelLoadPopulation;
	public GameObject panelSavePopulation;
	public GameObject panelArena;
	public GameObject panelCrossover;
	public GameObject panelTrials;
	public GameObject panelFitnessFunction;
	public GameObject panelMiniGameSettings;
	public GameObject panelDataVisualizations;
	public GameObject panelPlayers;
	public GameObject panelMenuBar;
	public GameObject panelTextLog;
	public GameObject panelWarning;

	public GameController gameController;

	// Panel Scripts:
	public TrainerPlayersUI panelPlayersScript;
	public TrainerMenuBarUI panelMenuBarScript;
	public TrainerMenuUI panelMenuScript;
	public TrainerWarningUI panelWarningScript;
	public TrainerPopulationUI panelPopulationScript;
	public TrainerNewPopulationUI panelNewPopulationScript;
	public TrainerCrossoverUI panelCrossoverScript;
	public TrainerDataVisUI panelDataVisScript;
	public TrainerTrialsUI panelTrialsScript;
	public TrainerFitnessUI panelFitnessScript;
	public TrainerMiniGameUI panelMiniGameScript;
	public TrainerTextLogUI panelTextLogScript;

	// Keeping track of active/inactive states of each panel:
	private bool panelMenuOn = true;
	private bool panelDataViewOn = false;
	private bool panelPopulationOn = false;
	private bool panelNewPopulationOn = false;
	private bool panelLoadPopulationOn = false;
	private bool panelSavePopulationOn = false;
	private bool panelArenaOn = false;
	private bool panelCrossoverOn = true;
	private bool panelTrialsOn = true;
	private bool panelFitnessFunctionOn = false;
	private bool panelMiniGameSettingsOn = false;
	//private bool panelDataVisualizationsOn = true;

	#endregion

	// Use this for initialization
	void Awake () {

	}

	public void InitializeModuleUI() {
		DebugBot.DebugFunctionCall("TModuleUI; InitializeModuleUI(); ", debugFunctionCalls);

		Trainer trainerRef = gameController.masterTrainer;
		if(trainerRef.PlayerList == null) {
			trainerRef.InitializePlayerList();
		}
		if(trainerRef.PlayerList[trainerRef.CurPlayer-1].masterPopulation == null) {
			//trainerRef.PlayerList[trainerRef.CurPlayer-1].InitializeNewPopulation(); 
		}

		// Store sub-panel scripts in a variable
		panelPlayersScript = panelPlayers.GetComponent<TrainerPlayersUI>();  
		panelMenuBarScript = panelMenuBar.GetComponent<TrainerMenuBarUI>();  
		panelMenuScript = panelMenu.GetComponent<TrainerMenuUI>();
		panelWarningScript = panelWarning.GetComponent<TrainerWarningUI>(); 
		panelPopulationScript = panelPopulation.GetComponent<TrainerPopulationUI>();
		panelNewPopulationScript = panelNewPopulation.GetComponent<TrainerNewPopulationUI>();
		panelCrossoverScript = panelCrossover.GetComponent<TrainerCrossoverUI>();
		panelDataVisScript = panelDataVisualizations.GetComponent<TrainerDataVisUI>();
		panelTrialsScript = panelTrials.GetComponent<TrainerTrialsUI>();
		panelFitnessScript = panelFitnessFunction.GetComponent<TrainerFitnessUI>();
		panelMiniGameScript = panelMiniGameSettings.GetComponent<TrainerMiniGameUI>();
		panelTextLogScript = panelTextLog.GetComponent<TrainerTextLogUI>();
		SetAllPanelsFromTrainerData(); // Updates UI elements in every Trainer sub-panel based on current data

		UpdatePanelVisibility();  // Handles which panels should be currently visible
	}

	public void CheckForAllPendingApply() {  // check each sub-panel for a pending apply and if so, run that panel's ApplyFromUIData method
		DebugBot.DebugFunctionCall("TModuleUI; CheckForAllPendingApply(); ", debugFunctionCalls);
		Trainer trainerRef = gameController.masterTrainer;

		// Right now, this should work because this function is only called during the NextGeneration method of the Trainer
		trainerRef.applyPanelPlayers = true;   // basically, true means that the current state of the system allows changes to each of these panel's data
		trainerRef.applyPanelPopulation = true;
		trainerRef.applyPanelCrossover = true; 
		trainerRef.applyPanelTrials = true; 
		trainerRef.applyPanelFitness = true; 
		trainerRef.applyPanelMiniGame = true; 
		// Population Panel:
		if(panelPopulationScript.applyPressed) {
			if(trainerRef.applyPanelPopulation) {
				panelPopulationScript.SetTrainerDataFromUIApply();
			}
		}
		// Players Panel:
		if(panelPlayersScript.applyPressed) {
			if(trainerRef.applyPanelPlayers) {
				panelPlayersScript.SetTrainerDataFromUIApply();
			}
		}
		// Trials Panel:
		if(panelTrialsScript.applyPressed) {
			if(trainerRef.applyPanelTrials) {
				panelTrialsScript.SetTrainerDataFromUIApply();
			}
		}
		// Fitness Panel:
		if(panelFitnessScript.applyPressed) {
			if(trainerRef.applyPanelFitness) {
				panelFitnessScript.SetTrainerDataFromUIApply();
			}
		}
		// MiniGame Panel:
		if(panelMiniGameScript.applyPressed) {
			if(trainerRef.applyPanelMiniGame) {
				panelMiniGameScript.SetTrainerDataFromUIApply();
			}
		}
		// Crossover Panel:
		if(panelCrossoverScript.applyPressed) {
			if(trainerRef.applyPanelCrossover) {
				panelCrossoverScript.SetTrainerDataFromUIApply();
			}
		}
		// Fill out the other panels:

		// Reset:
		trainerRef.applyPanelPlayers = false;   // basically, true means that the current state of the system allows changes to each of these panel's data
		trainerRef.applyPanelPopulation = false;
		trainerRef.applyPanelCrossover = false; 
		trainerRef.applyPanelTrials = false; 
		trainerRef.applyPanelFitness = false; 
		trainerRef.applyPanelMiniGame = false; 
	}

	#region Update Panels UI from current Data:

	public void SetAllPanelsFromTrainerData() {
		DebugBot.DebugFunctionCall("TModuleUI; SetAllPanelsFromTrainerData(); ", debugFunctionCalls);
		panelPlayersScript.InitializePanelWithTrainerData();
		panelMenuBarScript.InitializePanelWithTrainerData();
		panelMenuScript.InitializePanelWithTrainerData();
		panelPopulationScript.InitializePanelWithTrainerData();
		panelNewPopulationScript.InitializePanelWithTrainerData();
		panelCrossoverScript.InitializePanelWithTrainerData();
		panelDataVisScript.InitializePanelWithTrainerData();
		panelTrialsScript.InitializePanelWithTrainerData();
		panelFitnessScript.InitializePanelWithTrainerData();
		panelMiniGameScript.InitializePanelWithTrainerData();
		panelTextLogScript.InitializePanelWithTrainerData();
	}

	#endregion
	
	#region Panels On/Off Functions:
	private void UpdatePanelVisibility() {
		DebugBot.DebugFunctionCall("TModuleUI; UpdatePanelVisibility(); ", debugFunctionCalls);
		if(panelMenuOn) {
			panelMenu.SetActive(true);
		}
		else {
			panelMenu.SetActive (false);
		}
		if(panelDataViewOn) {
			panelDataView.SetActive(true);
		}
		else {
			panelDataView.SetActive (false);
		}
		if(panelPopulationOn) {
			panelPopulation.SetActive(true);
		}
		else {
			panelPopulation.SetActive (false);
		}
		if(panelNewPopulationOn) {
			panelNewPopulation.SetActive(true);
		}
		else {
			panelNewPopulation.SetActive (false);
		}
		if(panelLoadPopulationOn) {
			panelLoadPopulation.SetActive(true);
		}
		else {
			panelLoadPopulation.SetActive (false);
		}
		if(panelSavePopulationOn) {
			panelSavePopulation.SetActive(true);
		}
		else {
			panelSavePopulation.SetActive (false);
		}
		if(panelArenaOn) {
			panelArena.SetActive(true);
		}
		else {
			panelArena.SetActive (false);
			ArenaGroup.arenaGroupStatic.ArenaViewOff();
		}
		if(panelCrossoverOn) {
			panelCrossover.SetActive(true);
		}
		else {
			panelCrossover.SetActive (false);
		}
		if(panelTrialsOn) {
			panelTrials.SetActive(true);
		}
		else {
			panelTrials.SetActive (false);
		}
		if(panelFitnessFunctionOn) {
			panelFitnessFunction.SetActive(true);
		}
		else {
			panelFitnessFunction.SetActive (false);
		}
		if(panelMiniGameSettingsOn) {
			panelMiniGameSettings.SetActive(true);
		}
		else {
			panelMiniGameSettings.SetActive (false);
		}
		
	}  // Looks at the bool values for each panel and turns it on or off

	public void ClickMenu() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickMenu(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelMenuOn = true;
		UpdatePanelVisibility();
	}

	public void ClickDataView() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickDataView(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelDataViewOn = true;
		UpdatePanelVisibility();
	}

	public void ClickPopulation() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickPopulation(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelPopulationOn = true;
		UpdatePanelVisibility();
		panelPopulationScript.InitializePanelWithTrainerData();
	}

	public void ClickNewPopulation() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickNewPopulation(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelNewPopulationOn = true;
		UpdatePanelVisibility();
		panelNewPopulationScript.InitializePanelWithTrainerData();
	}

	public void ClickLoadPopulation() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickLoadPopulation(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelLoadPopulationOn = true;
		UpdatePanelVisibility();
	}

	public void ClickSavePopulation() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickSavePopulation(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelSavePopulationOn = true;
		UpdatePanelVisibility();
	}

	public void ClickArena() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickArena(); ", debugFunctionCalls);
		TurnOffExclusivePanelsPlayspace();
		panelArenaOn = true;
		ArenaGroup.arenaGroupStatic.ArenaViewOn();
		UpdatePanelVisibility();
	}

	public void ClickBackToTrials() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickBackToTrials(); ", debugFunctionCalls);
		TurnOffExclusivePanelsTrials();
		panelTrialsOn = true;
		UpdatePanelVisibility();
		panelTrialsScript.InitializePanelWithTrainerData();
	}

	public void ClickAddTrial() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickAddTrial(); ", debugFunctionCalls);
		TurnOffExclusivePanelsTrials();
		panelFitnessFunctionOn = true;
		UpdatePanelVisibility();
		panelFitnessScript.InitializePanelWithTrainerData();
	}

	public void ClickChooseMiniGame() {
		DebugBot.DebugFunctionCall("TModuleUI; ClickChooseMiniGame(); ", debugFunctionCalls);
		TurnOffExclusivePanelsTrials();
		panelMiniGameSettingsOn = true;
		UpdatePanelVisibility();
		panelMiniGameScript.InitializePanelWithTrainerData();
	}

	private void TurnOffExclusivePanelsPlayspace() {
		DebugBot.DebugFunctionCall("TModuleUI; TurnOffExclusivePanelsPlayspace(); ", debugFunctionCalls);
		panelMenuOn = false;
		panelDataViewOn = false;
		panelPopulationOn = false;
		panelNewPopulationOn = false;
		panelLoadPopulationOn = false;
		panelSavePopulationOn = false;
		panelArenaOn = false;
	}
	
	private void TurnOffExclusivePanelsTrials() {
		DebugBot.DebugFunctionCall("TModuleUI; TurnOffExclusivePanelsTrials(); ", debugFunctionCalls);
		panelTrialsOn = false;
		panelFitnessFunctionOn = false;
		panelMiniGameSettingsOn = false;
	}
	#endregion

}
