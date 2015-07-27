using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerCrossoverUI : MonoBehaviour {

	public bool debugFunctionCalls = true;

	public TrainerModuleUI trainerModuleScript;
	public GameObject panelCrossoverPanel;

	public Slider sliderMasterMutationRate;
	public Text textMasterMutationRate;
	public Slider sliderMaximumWeight;
	public Text textMaximumWeight;
	public Slider sliderMutationDriftScale;
	public Text textMutationDriftScale;
	public Slider sliderSeverLinkChance;
	public Text textSeverLinkChance;
	public Slider sliderAddLinkChance;
	public Text textAddLinkChance;
	public Slider sliderFunctionChance;
	public Text textFunctionChance;

	public Slider sliderNumSwapPositions;
	public Text textNumSwapPositions;
	public Slider sliderNumFactions;
	public Text textNumFactions;
	public Slider sliderMinNumParents;
	public Text textMinNumParents;
	public Slider sliderMaxNumParents;
	public Text textMaxNumParents;
	public Toggle toggleBreedWithSimilar;

	public Slider sliderSurvivalRate;
	public Text textSurvivalRate;
	public Toggle toggleSurvivalByRanking;
	public Toggle toggleSurvivalRandom;
	public Toggle toggleSurvivalByFitLottery;

	public Slider sliderBreedingRate;
	public Text textBreedingRate;
	public Toggle toggleBreedingByRanking;
	public Toggle toggleBreedingRandom;
	public Toggle toggleBreedingByFitLottery;

	public Button buttonApply;
	public Button buttonCancel;
	public Image bgImage;

	private Player playerRef;

	public CrossoverManager pendingCrossoverManager;

	// UI Settings:
	
	private bool panelActive = false;  // requires valid population
	
	public bool valuesChanged = false;
	public bool applyPressed = false;

	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TCrossoverUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		//CheckActivationCriteria();


		// set population references:
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		int curPlayer = trainer.CurPlayer;
		playerRef = trainer.PlayerList[curPlayer-1]; // get current population instance
		if(pendingCrossoverManager == null) {
			pendingCrossoverManager = new CrossoverManager();
		}
		pendingCrossoverManager.CopyFromSourceCrossoverManager(playerRef.masterCupid);

		// SET values from trainer data:
		sliderMasterMutationRate.minValue = 0f; // set up slider bounds
		sliderMasterMutationRate.maxValue = 0.2f;
		//sliderMasterMutationRate.value = pendingCrossoverManager.masterMutationRate;
		//textMasterMutationRate.text = pendingCrossoverManager.masterMutationRate.ToString();
		sliderMaximumWeight.minValue = 0f; // set up slider bounds
		sliderMaximumWeight.maxValue = 5f;
		sliderMutationDriftScale.minValue = 0f; // set up slider bounds
		sliderMutationDriftScale.maxValue = 1f;
		sliderSeverLinkChance.minValue = 0f; // set up slider bounds
		sliderSeverLinkChance.maxValue = 1f;
		sliderAddLinkChance.minValue = 0f; // set up slider bounds
		sliderAddLinkChance.maxValue = 1f;
		sliderFunctionChance.minValue = 0f; // set up slider bounds
		sliderFunctionChance.maxValue = 1f;

		sliderNumSwapPositions.minValue = 1;
		sliderNumSwapPositions.maxValue = 20;
		sliderNumFactions.minValue = 1;
		sliderNumFactions.maxValue = 20;
		sliderMinNumParents.minValue = 1;
		sliderMinNumParents.maxValue = 20;
		sliderMaxNumParents.minValue = 1;
		sliderMaxNumParents.maxValue = 20;

		sliderSurvivalRate.minValue = 0f; // set up slider bounds
		sliderSurvivalRate.maxValue = 1f;
		sliderBreedingRate.minValue = 0f; // set up slider bounds
		sliderBreedingRate.maxValue = 1f;

		
		valuesChanged = false;
		applyPressed = false;
		
		UpdateUIWithCurrentData();
	}
	
	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TCrossoverUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		//populationRef.SetMaxPopulationSize(pendingMaxPopulationSize);  // Set new max population Size
		playerRef.masterCupid.CopyFromSourceCrossoverManager(pendingCrossoverManager);
		InitializePanelWithTrainerData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TCrossoverUI; CheckActivationCriteria(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		int curPlayer = trainer.CurPlayer;

		panelActive = false;


		if(trainer.PlayerList != null) {
			if(trainer.PlayerList[curPlayer-1].masterPopulation != null) {
				if(trainer.PlayerList[curPlayer-1].masterPopulation.isFunctional) {
					panelActive = true;
				}
			}
		}	

	}
	
	public void UpdateUIElementStates() {
		DebugBot.DebugFunctionCall("TCrossoverUI; UpdateUIElementStates(); ", debugFunctionCalls);
		// Changing Button Displays !!
		
		// Active Population Options:
		if(panelActive) {
			panelCrossoverPanel.SetActive (true);
			//buttonDataView.interactable = false;
		}
		else {
			panelCrossoverPanel.SetActive (false);
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
		DebugBot.DebugFunctionCall("TCrossoverUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		sliderMasterMutationRate.value = pendingCrossoverManager.masterMutationRate;
		textMasterMutationRate.text = pendingCrossoverManager.masterMutationRate.ToString();
		sliderMaximumWeight.value = pendingCrossoverManager.maximumWeightMagnitude;
		textMaximumWeight.text = pendingCrossoverManager.maximumWeightMagnitude.ToString();
		sliderMutationDriftScale.value = pendingCrossoverManager.mutationDriftScale;
		textMutationDriftScale.text = pendingCrossoverManager.mutationDriftScale.ToString();
		sliderSeverLinkChance.value = pendingCrossoverManager.mutationRemoveLinkChance;
		textSeverLinkChance.text = pendingCrossoverManager.mutationRemoveLinkChance.ToString();
		sliderAddLinkChance.value = pendingCrossoverManager.mutationAddLinkChance;
		textAddLinkChance.text = pendingCrossoverManager.mutationAddLinkChance.ToString();
		sliderFunctionChance.value = pendingCrossoverManager.mutationFunctionChance;
		textFunctionChance.text = pendingCrossoverManager.mutationFunctionChance.ToString();

		sliderNumSwapPositions.value = pendingCrossoverManager.numSwapPositions;
		textNumSwapPositions.text = pendingCrossoverManager.numSwapPositions.ToString();
		sliderNumFactions.value = pendingCrossoverManager.numFactions;
		textNumFactions.text = pendingCrossoverManager.numFactions.ToString();
		sliderMinNumParents.value = pendingCrossoverManager.minNumParents;
		textMinNumParents.text = pendingCrossoverManager.minNumParents.ToString();
		sliderMaxNumParents.value = pendingCrossoverManager.maxNumParents;
		textMaxNumParents.text = pendingCrossoverManager.maxNumParents.ToString();
		
		sliderSurvivalRate.value = pendingCrossoverManager.survivalRate; 
		textSurvivalRate.text = pendingCrossoverManager.survivalRate.ToString(); 
		sliderBreedingRate.value = pendingCrossoverManager.breedingRate; 
		textBreedingRate.text = pendingCrossoverManager.breedingRate.ToString(); 

		CheckActivationCriteria();
		UpdateUIElementStates();
	}	

	public void SliderMasterMutationRate(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderMasterMutationRate(); ", debugFunctionCalls);
		pendingCrossoverManager.masterMutationRate = sliderValue;
		float dataMasterMutationRate = playerRef.masterCupid.masterMutationRate;
		if(pendingCrossoverManager.masterMutationRate != dataMasterMutationRate) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderMaximumWeight(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderMaximumWeight(); ", debugFunctionCalls);
		pendingCrossoverManager.maximumWeightMagnitude = sliderValue;
		float dataWeightMagnitude = playerRef.masterCupid.maximumWeightMagnitude;
		if(pendingCrossoverManager.maximumWeightMagnitude != dataWeightMagnitude) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderMutationDriftScale(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderMutationDriftScale(); ", debugFunctionCalls);
		pendingCrossoverManager.mutationDriftScale = sliderValue;
		float dataMutationDriftScale = playerRef.masterCupid.mutationDriftScale;
		if(pendingCrossoverManager.mutationDriftScale != dataMutationDriftScale) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderRemoveLinkChance(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderRemoveLinkChance(); ", debugFunctionCalls);
		pendingCrossoverManager.mutationRemoveLinkChance = sliderValue;
		float dataRemoveLinkChance = playerRef.masterCupid.mutationRemoveLinkChance;
		if(pendingCrossoverManager.mutationRemoveLinkChance != dataRemoveLinkChance) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderAddLinkChance(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderAddLinkChance(); ", debugFunctionCalls);
		pendingCrossoverManager.mutationAddLinkChance = sliderValue;
		float dataAddLinkChance = playerRef.masterCupid.mutationAddLinkChance;
		if(pendingCrossoverManager.mutationAddLinkChance != dataAddLinkChance) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderFunctionChance(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderFunctionChance(); ", debugFunctionCalls);
		pendingCrossoverManager.mutationFunctionChance = sliderValue;
		float dataFunctionChance = playerRef.masterCupid.mutationFunctionChance;
		if(pendingCrossoverManager.mutationFunctionChance != dataFunctionChance) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderNumSwapPositions(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderNumSwapPositions(); ", debugFunctionCalls);
		pendingCrossoverManager.numSwapPositions = (int)sliderValue;
		int dataNumSwapPositions = playerRef.masterCupid.numSwapPositions;
		if(pendingCrossoverManager.numSwapPositions != dataNumSwapPositions) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderNumFactions(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderNumFactions(); ", debugFunctionCalls);
		pendingCrossoverManager.numFactions = (int)sliderValue;
		int dataNumFactions = playerRef.masterCupid.numFactions;
		if(pendingCrossoverManager.numFactions != dataNumFactions) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderMinNumParents(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderMinNumParents(); ", debugFunctionCalls);
		pendingCrossoverManager.minNumParents = (int)sliderValue;
		int dataMinNumParents = playerRef.masterCupid.minNumParents;
		if(pendingCrossoverManager.minNumParents != dataMinNumParents) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderMaxNumParents(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderMaxNumParents(); ", debugFunctionCalls);
		pendingCrossoverManager.maxNumParents = (int)sliderValue;
		int dataMaxNumParents = playerRef.masterCupid.maxNumParents;
		if(pendingCrossoverManager.maxNumParents != dataMaxNumParents) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderSurvivalRate(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderSurvivalRate(); ", debugFunctionCalls);
		pendingCrossoverManager.survivalRate = sliderValue;
		float dataSurvivalRate = playerRef.masterCupid.survivalRate;
		if(pendingCrossoverManager.survivalRate != dataSurvivalRate) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void SliderBreedingRate(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TCrossoverUI; SliderBreedingRate(); ", debugFunctionCalls);
		pendingCrossoverManager.breedingRate = sliderValue;
		float dataBreedingRate = playerRef.masterCupid.breedingRate;
		if(pendingCrossoverManager.breedingRate != dataBreedingRate) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}

	public void ToggleBreedWithSimilar(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleBreedWithSimilar(); ", debugFunctionCalls);
		pendingCrossoverManager.breedWithSimilar = value;
	}

	public void ToggleSurvivalByRank(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleSurvivalByRank(); ", debugFunctionCalls);
		if(value) {  // if was off, turning on:
			pendingCrossoverManager.survivalByRank = true;
			pendingCrossoverManager.survivalStochastic = false;
			pendingCrossoverManager.survivalByRaffle = false;
			toggleSurvivalRandom.isOn = false;
			toggleSurvivalByFitLottery.isOn = false;
			valuesChanged = true;
		}
		else {
			if(pendingCrossoverManager.survivalByRank) {
				toggleSurvivalByRanking.isOn = true;
			}
		}
	}
	public void ToggleSurvivalStochastic(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleSurvivalStochastic(); ", debugFunctionCalls);
		if(value) {  // if was off, turning on:
			pendingCrossoverManager.survivalByRank = false;
			pendingCrossoverManager.survivalStochastic = true;
			pendingCrossoverManager.survivalByRaffle = false;
			toggleSurvivalByRanking.isOn = false;
			toggleSurvivalByFitLottery.isOn = false;
			valuesChanged = true;
		}
		else {
			if(pendingCrossoverManager.survivalStochastic) {
				toggleSurvivalRandom.isOn = true;
			}
		}
	}
	public void ToggleSurvivalByRaffle(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleSurvivalByRaffle(); ", debugFunctionCalls);
		if(value) {  // if was off, turning on:
			pendingCrossoverManager.survivalByRank = false;
			pendingCrossoverManager.survivalStochastic = false;
			pendingCrossoverManager.survivalByRaffle = true;
			toggleSurvivalByRanking.isOn = false;
			toggleSurvivalRandom.isOn = false;
			valuesChanged = true;
		}
		else {  // if was on and turning off:
			if(pendingCrossoverManager.survivalByRaffle) { // make sure it only affects the current toggle being turned off -- 
				// before this was being triggered by the other toggles being switched off programattically, creating infinite loop
				toggleSurvivalByFitLottery.isOn = true;
			}
		}
	}

	public void ToggleBreedingByRank(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleBreedingByRank(); ", debugFunctionCalls);
		if(value) {  // if was off, turning on:
			pendingCrossoverManager.breedingByRank = true;
			pendingCrossoverManager.breedingStochastic = false;
			pendingCrossoverManager.breedingByRaffle = false;
			//toggleBreedingByRanking.isOn = true;
			toggleBreedingRandom.isOn = false;
			toggleBreedingByFitLottery.isOn = false;
			valuesChanged = true;
		}
		else {
			if(pendingCrossoverManager.breedingByRank) {
				toggleBreedingByRanking.isOn = true;
			}
		}
	}
	public void ToggleBreedingStochastic(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleBreedingStochastic(); ", debugFunctionCalls);
		if(value) {  // if was off, turning on:
			pendingCrossoverManager.breedingByRank = false;
			pendingCrossoverManager.breedingStochastic = true;
			pendingCrossoverManager.breedingByRaffle = false;
			toggleBreedingByRanking.isOn = false;
			toggleBreedingByFitLottery.isOn = false;
			valuesChanged = true;
		}
		else {
			if(pendingCrossoverManager.breedingStochastic) {
				toggleBreedingRandom.isOn = true;
			}
		}
	}
	public void ToggleBreedingByRaffle(bool value) {
		DebugBot.DebugFunctionCall("TCrossoverUI; ToggleBreedingByRaffle(); ", debugFunctionCalls);
		if(value) {  // if was off, turning on:
			pendingCrossoverManager.breedingByRank = false;
			pendingCrossoverManager.breedingStochastic = false;
			pendingCrossoverManager.breedingByRaffle = true;
			toggleBreedingByRanking.isOn = false;
			toggleBreedingRandom.isOn = false;
			valuesChanged = true;
		}
		else {
			if(pendingCrossoverManager.breedingByRaffle) {
				toggleBreedingByFitLottery.isOn = true;
			}
		}
	}

	
	public void ClickApply() {
		DebugBot.DebugFunctionCall("TCrossoverUI; ClickApply(); ", debugFunctionCalls);
		applyPressed = true;
		UpdateUIElementStates();  // change background color to indicate pending changes
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		
		// !!!!!!!!!!! TEMPORARY !!!!!! Replace this code once play/pause/fastMode etc. are in and the Trainer class will trigger this when ApplyCriteria are met
		if(trainer.applyPanelCrossover) {  // if apply criteria are met currently:
			SetTrainerDataFromUIApply();
		}
		//DebugFunctionCall("ClickApply()");
	}
	public void ClickCancel() {
		DebugBot.DebugFunctionCall("TCrossoverUI; ClickCancel(); ", debugFunctionCalls);
		InitializePanelWithTrainerData();
		//DebugFunctionCall("ClickCancel()");
	}
}
