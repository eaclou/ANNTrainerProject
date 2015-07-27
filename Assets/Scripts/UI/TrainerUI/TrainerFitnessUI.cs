using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrainerFitnessUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public TrainerModuleUI trainerModuleScript;
	public GameObject panelVisible;
	public Button buttonEditMiniGameType;
	public Text textButtonEditMiniGameType;

	public Transform transformFitnessCompTableSpace;
	[SerializeField] GameObject goFitnessCompRowPrefab;

	public Button buttonApply;
	public Button buttonCancel;
	public Button buttonBack;
	public Image bgImage;

	private Population populationRef;

	public FitnessManager pendingFitnessManager;
	public MiniGameManager pendingMiniGameManager;

	//public int pendingNumSelectedInputs;
	//public int pendingMaxSelectedInputs;
	//public int pendingNumSelectedOutputs;
	//public int pendingMaxSelectedOutputs;
	
	// UI Settings:	
	private bool panelActive = false;  // requires valid population
	public bool valuesChanged = false;
	public bool applyPressed = false;
	
	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TFitnessUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		// SET PENDING values from trainer data:
		if(pendingFitnessManager == null) {
			pendingFitnessManager = new FitnessManager(currentPlayer);
			//CopyFitnessComponentList(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.masterFitnessCompList, pendingFitnessManager.masterFitnessCompList);
			InitializePanelFitnessCompList();
		}
		if(pendingMiniGameManager == null) {
			pendingMiniGameManager = new MiniGameManager(currentPlayer);		
		}
		currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.UpdatePopulationSize(); // Tie this into Population!!!!!
		currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.SetMasterFitnessComponentList(); // add fitnessComp lists from brain and minigame
		pendingFitnessManager.SetMasterFitnessComponentList();

		// OLD:
		List<FitnessComponent> dataFitnessComponentList = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.masterFitnessCompList;
		CopyFitnessComponentList(dataFitnessComponentList, pendingFitnessManager.masterFitnessCompList);
		// NEW:
		List<FitnessComponent> dataBrainFitnessComponentList = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList;
		CopyFitnessComponentList(dataBrainFitnessComponentList, pendingFitnessManager.brainFitnessComponentList);
		List<FitnessComponent> dataGameFitnessComponentList = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList;
		CopyFitnessComponentList(dataGameFitnessComponentList, pendingFitnessManager.gameFitnessComponentList);

		InitializePanelFitnessCompList();

		valuesChanged = false;
		applyPressed = false;

		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TFitnessUI; CheckActivationCriteria(); ", debugFunctionCalls);
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
		DebugBot.DebugFunctionCall("TFitnessUI; UpdateUIElementStates(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		//DebugBot.DebugFunctionCall("TFitnessUI; UpdateUIElementStates(); " + trainer.PlayerList.Count.ToString(), debugFunctionCalls);
		Player currentPlayer = trainer.PlayerList[trainer.CurPlayer-1];
		// Changing Button Displays !!
		if(panelActive) {
			panelVisible.SetActive (true);
		}
		else {
			panelVisible.SetActive (false);
		}
		if(valuesChanged) {
			buttonApply.interactable = true;
			buttonCancel.interactable = true;
		}
		else {
			buttonApply.interactable = false;
			buttonCancel.interactable = false;
		}
		// Choose Mini-game Drop-down:
		if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType == MiniGameManager.MiniGameType.None) {
			textButtonEditMiniGameType.text = "Mini-Game Type";
		}
		else {
			textButtonEditMiniGameType.text = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType.ToString();
		}
		// Background Color for pending changes:
		if(applyPressed) {
			bgImage.color = new Color(0.99f, 0.75f, 0.6f);
		}
		else {
			bgImage.color = trainerModuleScript.defaultBGColor;
		}
	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TFitnessUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		// Copy values from pending over to data
		CopyFitnessComponentList(pendingFitnessManager.masterFitnessCompList, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.masterFitnessCompList);
		CopyFitnessComponentList(pendingFitnessManager.brainFitnessComponentList, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList);
		CopyFitnessComponentList(pendingFitnessManager.gameFitnessComponentList, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList);
		//CopyFitnessComponentList(pendingFitnessManager.brainFitnessComponentList, currentPlayer.masterPopulation.templateBrain.brainFitnessComponentList);
		//CopyFitnessComponentList(pendingFitnessManager.gameFitnessComponentList, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.miniGameInstance.fitnessComponentList);

		currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.SetFitnessComponentScoreArray();  // update distilled Array from FitnessComponentList

		InitializePanelWithTrainerData();
	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TFitnessUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void InitializePanelFitnessCompList() {
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		//DebugBot.DebugFunctionCall("TFitnessUI; InitializePanelFitnessCompList(" + pendingMiniGameManager.miniGameInstance.inputChannelsList.Count.ToString() + "); ", debugFunctionCalls);
		// CLEAR CURRENT LIST:
		foreach (Transform child in transformFitnessCompTableSpace) {
			GameObject.Destroy(child.gameObject);
		}
		//pendingNumSelectedInputs = 0;
		//pendingMaxSelectedInputs = currentPlayer.masterPopulation.numInputNodes;
		// Where Am I setting the InputChannelList values????? --answer: inside miniGameInputRowScript.InitializePanelWithTrainerData();
		for(int i = 0; i < pendingFitnessManager.brainFitnessComponentList.Count; i++) {
			GameObject fitnessComponentListRow = (GameObject)Instantiate (goFitnessCompRowPrefab);
			TrainerFitnessCompRowUI fitnessComponentRowScript = fitnessComponentListRow.GetComponent<TrainerFitnessCompRowUI>();
			fitnessComponentRowScript.inBrainList = true; 
			fitnessComponentRowScript.fitnessIndex = i; // CHANGE LATER!!!!!!!
			fitnessComponentRowScript.trainerModuleScript = trainerModuleScript;
			fitnessComponentRowScript.trainerFitnessScript = this;
			fitnessComponentRowScript.InitializePanelWithTrainerData();
			fitnessComponentListRow.transform.SetParent(transformFitnessCompTableSpace);
		}
		for(int i = 0; i < pendingFitnessManager.gameFitnessComponentList.Count; i++) {
			GameObject fitnessComponentListRow = (GameObject)Instantiate (goFitnessCompRowPrefab);
			TrainerFitnessCompRowUI fitnessComponentRowScript = fitnessComponentListRow.GetComponent<TrainerFitnessCompRowUI>();
			fitnessComponentRowScript.inBrainList = false; 
			fitnessComponentRowScript.fitnessIndex = i; // CHANGE LATER!!!!!!!
			fitnessComponentRowScript.trainerModuleScript = trainerModuleScript;
			fitnessComponentRowScript.trainerFitnessScript = this;
			fitnessComponentRowScript.InitializePanelWithTrainerData();
			fitnessComponentListRow.transform.SetParent(transformFitnessCompTableSpace);
		}
	}

	public void CopyFitnessComponentList(List<FitnessComponent> source, List<FitnessComponent> target) {		// Maybe update this by making it a function of BrainInputChannel that takes an instance of itself
		// Check if both are the same length?
		int numComponents = source.Count;
		if(numComponents == target.Count) {
			for(int i = 0; i < numComponents; i++) {
				string newName = "";
				newName = source[i].componentName;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].componentName = newName;
				float newValue = 0f;
				newValue = source[i].componentScore[0];  // Make sure these are allocating new memory and will be copies, not references!
				target[i].componentScore[0] = newValue;
				bool newOn = false;
				newOn = source[i].on;   // Make sure these are allocating new memory and will be copies, not references!
				target[i].on = newOn;
				bool newBigIsBetter = false;
				newBigIsBetter = source[i].bigIsBetter;   // Make sure these are allocating new memory and will be copies, not references!
				target[i].bigIsBetter = newBigIsBetter;
				float newPower = 0f;
				newPower = source[i].power;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].power = newPower;
				float newWeight = 0f;
				newWeight = source[i].weight;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].weight = newWeight;
				target[i].divideByTimeSteps = source[i].divideByTimeSteps;
				DebugBot.DebugFunctionCall("TFitnessUI; CopyFitnessComponentList: " + i.ToString() + ": " + source[i].on.ToString() + ", bib: " + source[i].bigIsBetter.ToString() + ", pow: " + source[i].power.ToString() + ", targ: " + target[i].on.ToString() + ", bib: " + target[i].bigIsBetter.ToString() + ", pow: " + target[i].power.ToString(), debugFunctionCalls);
			}
		}
		else {
			DebugBot.DebugFunctionCall("TFitnessUI; CopyFitnessComponentList(); Arrays of Different Length! source= " + source.Count.ToString () + ", target= " + target.Count.ToString (), debugFunctionCalls);
		}
	}

	public void ClickApply() {
		DebugBot.DebugFunctionCall("TFitnessUI; ClickApply(); ", debugFunctionCalls);
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
		DebugBot.DebugFunctionCall("TFitnessUI; ClickCancel(); ", debugFunctionCalls);
		//if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType != pendingMiniGameManager.gameType) { // if game-type has changed
		//	InitializePanelInputList();
		//	InitializePanelOutputList();
		//	pendingMiniGameManager.SetMiniGameType(pendingMiniGameManager.gameType);
		//}
		InitializePanelWithTrainerData();
		//DebugFunctionCall("ClickCancel()");
	}
}
