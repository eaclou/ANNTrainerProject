using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerPopulationUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public TrainerModuleUI trainerModuleScript;
	public Text textCurrentPopulationSize;
	public Slider sliderMaxPopulationSize;
	public Text textMaxPopulationSize;
	public Button buttonApply;
	public Button buttonCancel;
	//public Button buttonNewPopulation;
	public GameObject gameObjectPopulationOptions;
	public Slider sliderAgentsToAdd;
	public Text textAgentsToAdd;
	public Toggle toggleFillWithCopies;
	public Toggle toggleFillWithRandom;
	public Button buttonAddToPopulation;
	public Slider sliderAgentsToRemove;
	public Text textAgentsToRemove;
	public Toggle toggleRemoveByRank;
	public Toggle toggleRemoveByRandom;
	public Button buttonRemoveFromPopulation;
	public Slider sliderAmountToAverage;
	public Text textAmountToAverage;
	public Button buttonAveragePopulationGenomes;
	public Image bgImage;

	private Population populationRef;

	public int pendingMaxPopulationSize;
	// UI Settings:
	private int minMaxPopulationSize = 1;
	private int maxMaxPopulationSize = 100;

	private bool nullPopulation = true;

	public bool valuesChanged = false;
	public bool applyPressed = false;


	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TPopUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		if(trainer.PlayerList != null) {

			int curPlayer = trainer.CurPlayer;
			//Debug.Log ("InitializePanelWithTrainerData(), " + trainer.PlayerList[curPlayer-1].maxMaxPopulationSize.ToString());
			//sliderMaxPopulationSize.minValue = trainer.PlayerList[curPlayer-1].minMaxPopulationSize;
			//sliderMaxPopulationSize.maxValue = trainer.PlayerList[curPlayer-1].maxMaxPopulationSize;
			//sliderMaxPopulationSize.value = trainer.PlayerList[curPlayer-1].maxPopulationSize;
			if(trainer.PlayerList[curPlayer-1].masterPopulation != null) {  // if the current player has a Population instance:
				populationRef = trainer.PlayerList[curPlayer-1].masterPopulation; // get current population instance
				//Current Population text:
				textCurrentPopulationSize.text = "Current Population Size: " + (populationRef.isFunctional ? populationRef.masterAgentArray.Length.ToString() : "0"); // Update this later!!
				//Current Max Population Size:
				sliderMaxPopulationSize.minValue = minMaxPopulationSize; // set up slider bounds
				sliderMaxPopulationSize.maxValue = maxMaxPopulationSize;
				sliderMaxPopulationSize.value = populationRef.populationMaxSize;
				textMaxPopulationSize.text = populationRef.populationMaxSize.ToString();

			}
			else {  // Population hasn't been created yet:
				//textMaxPopulationSize.text = trainer.PlayerList[curPlayer-1].maxPopulationSize.ToString();
			}
		}

		valuesChanged = false;
		applyPressed = false;

		UpdateUIWithCurrentData();
	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TPopUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		populationRef.SetMaxPopulationSize(pendingMaxPopulationSize);  // Set new max population Size
		populationRef.TempResizeMasterAgentArray();
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		trainer.UpdatePlayingNumAgents();
		InitializePanelWithTrainerData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TPopUI; CheckActivationCriteria(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		int curPlayer = trainer.CurPlayer;
		bool activePlayerList = false;
		bool activePopulationCurPlayer = false;
		if(trainer.PlayerList == null) {
			activePlayerList = false;
		}
		else {
			activePlayerList = true;
			if(trainer.PlayerList[curPlayer-1].masterPopulation == null) {
				activePopulationCurPlayer = false;
			}
			else {
				activePopulationCurPlayer = true;
			}
		}
		// Calculate Criteria:		
		//Active Population Options:

		if(activePopulationCurPlayer) {  // Current criteria is merely to have an active population -- come back to this and tighten restrictions later
			//Debug.Log ("trainer.PlayerList = null ");
			nullPopulation = false;
		}
		else {
			nullPopulation = true;
		}
	}
	
	public void UpdateUIElementStates() {
		DebugBot.DebugFunctionCall("TPopUI; UpdateUIElementStates(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		// Changing Button Displays !!

		// Active Population Options:
		if(nullPopulation) {
			gameObjectPopulationOptions.SetActive (false);
			//buttonDataView.interactable = false;
		}
		else {
			gameObjectPopulationOptions.SetActive (true);
			//buttonDataView.interactable = true;
		}

		if(valuesChanged) {
			buttonApply.interactable = true;
			buttonCancel.interactable = true;
		}
		else {
			buttonApply.interactable = false;
			buttonCancel.interactable = false;
		}
		if(applyPressed) {
			bgImage.color = new Color(0.99f, 0.75f, 0.6f);
		}
		else {
			bgImage.color = trainerModuleScript.defaultBGColor;
		}
	}

	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TPopUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		//Current Max Population Size:
		sliderMaxPopulationSize.value = pendingMaxPopulationSize;
		textMaxPopulationSize.text = pendingMaxPopulationSize.ToString();
		//int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		//sliderNumPlayers.value = pendingNumPlayers;		
		//textNumPlayers.text = pendingNumPlayers.ToString();
		//textCurPlayer.text = "PLAYER: " + dataCurPlayer.ToString();
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void SliderMaxPopulation(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TPopUI; SliderMaxPopulation(); ", debugFunctionCalls);
		pendingMaxPopulationSize = (int)sliderValue;
		int dataMaxPopulationSize = populationRef.populationMaxSize;
		if(pendingMaxPopulationSize != dataMaxPopulationSize) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
		//DebugFunctionCall("SliderNumPlayersChanged()");
	}

	public void ClickApply() {
		DebugBot.DebugFunctionCall("TPopUI; ClickApply(); ", debugFunctionCalls);
		applyPressed = true;
		UpdateUIElementStates();  // change background color to indicate pending changes
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;

		// !!!!!!!!!!! TEMPORARY !!!!!! Replace this code once play/pause/fastMode etc. are in and the Trainer class will trigger this when ApplyCriteria are met
		if(trainer.betweenGenerations) {  // if apply criteria are met currently:
			SetTrainerDataFromUIApply();
		}
		//DebugFunctionCall("ClickApply()");
	}
	public void ClickCancel() {
		DebugBot.DebugFunctionCall("TPopUI; ClickCancel(); ", debugFunctionCalls);
		InitializePanelWithTrainerData();
		//DebugFunctionCall("ClickCancel()");
	}

}
