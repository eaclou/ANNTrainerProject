using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerMenuBarUI : MonoBehaviour {

	public bool debugFunctionCalls = true;

	public TrainerModuleUI trainerModuleScript;

	public Button buttonMenu;
	public Button buttonDataView;
	public Button buttonPopulation;
	public Button buttonArena;
	public Button buttonTraining;
	public Button buttonPlay;
	public Button buttonFastMode;
	public Button buttonManualOverride;
	public Button buttonSlower;
	public Button buttonRealTime;
	public Button buttonFaster;
	public Text textTimeScale;
	public Image bgImage;

	private bool menuActive = true;
	private bool dataViewActive = false;
	private bool populationActive = false;
	private bool arenaActive = false;
	private bool trainingActive = false;
	private bool playActive = false;
	private bool fastModeActive = false;
	private bool manualOverrideActive = false;

	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TMenuBarUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TMenuBarUI; CheckActivationCriteria(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		int curPlayer = trainer.CurPlayer;
		bool activePlayerList = trainer.hasValidPlayerList;  // initialize as false
		bool activePopulationCurPlayer = trainer.PlayerList[curPlayer-1].hasValidPopulation;
		bool activeTrial = trainer.PlayerList[curPlayer-1].hasValidTrials;

		// Calculate Criteria: +++++++++++++++++++++++
		
		//Data View:
		if(activeTrial) {  // Current criteria is merely to have an active population -- come back to this and tighten restrictions later
			//Debug.Log ("trainer.PlayerList = null ");
			dataViewActive = false;
		}
		else {
			dataViewActive = false;
		}
		
		//Population
		if(!trainer.IsPlaying && activePopulationCurPlayer) {  // Trainer can't be playing a game - needs to be paused, and needs an active Population
			populationActive = true;
		}
		else {
			populationActive = false;
		}
		
		//Arena:
		if(activeTrial) {  // Needs an active population (and mini-game/compatible brain etc. -- add later
			arenaActive = true;
		}
		else {
			arenaActive = false;
		}
		
		//PlayPause & Training:
		if(activeTrial) {  // needs active population ready to play miniGame Trial
			playActive = true;
			trainingActive = true;
			fastModeActive = true;
		}
		else {
			playActive = false;
			trainingActive = false; // will also eventually need Legal Crossover class
			fastModeActive = false;
		}
		
		//Manual Override:
		if(activeTrial) {  // Needs an active population (and mini-game/compatible brain etc. -- add later
			manualOverrideActive = false;
		}
		else {
			manualOverrideActive = false;
		}
	}

	public void UpdateUIElementStates() {
		DebugBot.DebugFunctionCall("TMenuBarUI; UpdateUIElementStates(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		// Changing Button Displays !!
		// Menu Button:
		buttonMenu.interactable = true;
		// Data View Button:
		if(dataViewActive) {
			buttonDataView.interactable = true;
		}
		else {
			buttonDataView.interactable = false;
		}
		//Population
		if(populationActive) {
			buttonPopulation.interactable = true;
		}
		else {
			buttonPopulation.interactable = false;
		}
		//Arena
		if(arenaActive) {
			buttonArena.interactable = true;
		}
		else {
			buttonArena.interactable = false;
		}
		//IsTraining / Crossover
		if(trainingActive) {
			buttonTraining.interactable = true;
		}
		else {
			buttonTraining.interactable = false;
		}
		if(trainer.CrossoverOn) {
			//Color onColor = new Color(1.0f, 0.8f, 0.65f);
			//buttonTraining.colors.normalColor = onColor;
			buttonTraining.GetComponentInChildren<Text>().text = "Training";
		}
		else {
			//Color offColor = new Color(1f, 1f, 1f);
			//buttonTraining.colors.normalColor = offColor;
			buttonTraining.GetComponentInChildren<Text>().text = "Evaluating";
		}
		//Play Pause
		if(playActive) {
			buttonPlay.interactable = true;
		}
		else {
			buttonPlay.interactable = false;
		}
		if(trainer.IsPlaying) {
			buttonPlay.GetComponentInChildren<Text>().text = "Playing";
		}
		else {
			buttonPlay.GetComponentInChildren<Text>().text = "Paused";
		}
		textTimeScale.text = "Time Scale: " + trainer.playbackSpeed.ToString();
		//Fast Mode
		/*if(fastModeActive) {
			buttonFastMode.interactable = true;
		}
		else {
			buttonFastMode.interactable = false;
		}
		if(trainer.FastModeOn) {
			buttonFastMode.GetComponentInChildren<Text>().text = "Fast Mode!";
		}
		else {
			buttonFastMode.GetComponentInChildren<Text>().text = "Real Time";
		}
		//Manual Override!
		if(manualOverrideActive) {
			buttonManualOverride.interactable = true;
		}
		else {
			buttonManualOverride.interactable = false;
		}
		if(trainer.ManualOverrideOn) {
			buttonManualOverride.GetComponentInChildren<Text>().text = "Stick Engaged!";
		}
		else {
			buttonManualOverride.GetComponentInChildren<Text>().text = "Autonomous";
		}
		*/

		bgImage.color = trainerModuleScript.defaultBGColor;		
	}

	public void ClickPlay() {
		DebugBot.DebugFunctionCall("TMenuBarUI; ClickPlay(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		//int curPlayer = trainer.CurPlayer;
		trainer.TogglePlayPause();
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void ClickTraining() {
		DebugBot.DebugFunctionCall("TMenuBarUI; ClickTraining(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		//int curPlayer = trainer.CurPlayer;
		trainer.ToggleTraining();
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void ClickFastMode() {
		DebugBot.DebugFunctionCall("TMenuBarUI; ClickFastMode(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		//int curPlayer = trainer.CurPlayer;
		trainer.ToggleFastMode();
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void ClickSlowerPlayback() {
		DebugBot.DebugFunctionCall("TMenuBarUI; ClickSlowerPlayback(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;

		float timeScale = trainer.playbackSpeed * 0.75f;
		if(timeScale < 0.01f) { timeScale = 0.01f; }
		trainer.playbackSpeed = timeScale;
		if(trainer.IsPlaying) {   // change actual timeScale if the game is running, if paused, only change playback speed
			Time.timeScale = trainer.playbackSpeed;
		}

		//trainer.ToggleFastMode();
		//CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void ClickRealtimePlayback() {
		DebugBot.DebugFunctionCall("TMenuBarUI; ClickRealtimePlayback(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;

		//float timeScale = Time.timeScale * 0.75f;
		//if(timeScale < 0.01f) { timeScale = 0.01f; }
		trainer.playbackSpeed = 1.0f;
		if(trainer.IsPlaying) {   // change actual timeScale if the game is running, if paused, only change playback speed
			Time.timeScale = trainer.playbackSpeed;
		}

		//CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void ClickFasterPlayback() {
		DebugBot.DebugFunctionCall("TMenuBarUI; ClickFasterPlayback(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;

		float timeScale = trainer.playbackSpeed * 1.25f;
		if(timeScale > 100.0f) { timeScale = 100.0f; }
		trainer.playbackSpeed = timeScale;
		if(trainer.IsPlaying) {  // change actual timeScale if the game is running, if paused, only change playback speed
			Time.timeScale = trainer.playbackSpeed;
		}

		//CheckActivationCriteria();
		UpdateUIElementStates();
	}
}
