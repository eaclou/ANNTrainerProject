using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrainerMiniGameUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public TrainerModuleUI trainerModuleScript;
	public GameObject panelVisible;
	public Button buttonChooseMiniGame;
	public Text textButtonChooseMiniGame;
	public Transform dropDownChooseMiniGamePanel;
	[SerializeField] GameObject buttonDropDownOptionPrefab;

	public GameObject panelInputsVisible;
	public GameObject panelOutputsVisible;
	public GameObject panelOptionsVisible;

	public Button buttonInputPanel;
	public Toggle inputSelectAll;
	public Button buttonOutputPanel;
	public Toggle outputSelectAll;
	public Button buttonOptionsPanel;
	public Text textInputSources;
	public Text textOutputActions;

	public Transform inputListTableSpace;
	[SerializeField] GameObject panelMiniGameInputRowPrefab;
	public Transform outputListTableSpace;
	[SerializeField] GameObject panelMiniGameOutputRowPrefab;
	public Transform optionsListTableSpace;
	[SerializeField] GameObject panelMiniGameOptionsRowPrefab;
	public Button buttonApply;
	public Button buttonCancel;
	public Image bgImage;

	private Population populationRef;

	//public MiniGame.MiniGameType pendingMiniGameType;
	public MiniGameManager pendingMiniGameManager;
	
	// UI Settings:	
	public bool panelActive = false;  // requires valid population
	public bool inputsPanelOn = true;
	public bool outputsPanelOn = false;
	public bool optionsPanelOn = false;

	public bool applyPressed = false;
	public bool valuesChanged = false;
	private bool dropDownPopulated = false;

	public int pendingNumSelectedInputs;
	public int pendingMaxSelectedInputs;
	public int pendingNumSelectedOutputs;
	public int pendingMaxSelectedOutputs;

	public MiniGameManager.MiniGameType[] availableGameTypes;


	
	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TMiniGameUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		// SET PENDING values from trainer data:
		if(pendingMiniGameManager == null) {
			//DebugBot.DebugFunctionCall("TMiniGameUI; New InitializePanelWithTrainerData(); ", debugFunctionCalls);
			pendingMiniGameManager = new MiniGameManager(currentPlayer);

			InitializePanelInputList();
			InitializePanelOutputList();
		}

		// if game-type has changed -- this should only happen when clicking CANCEL ?  -- as Apply sets data from pendingData
		if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType != pendingMiniGameManager.gameType) { 
			pendingMiniGameManager.SetMiniGameType(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType);
		}
		List<BrainInputChannel> dataInputChannelsList = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.miniGameInstance.inputChannelsList;
		CopyInputChannelsList(dataInputChannelsList, pendingMiniGameManager.miniGameInstance.inputChannelsList);
		InitializePanelInputList();	
		List<BrainOutputChannel> dataOutputChannelList = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.miniGameInstance.outputChannelsList;
		CopyOutputChannelsList(dataOutputChannelList, pendingMiniGameManager.miniGameInstance.outputChannelsList);
		InitializePanelOutputList();

		currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.SetInputOutputArrays();

		if(!dropDownPopulated) {  // So it only creates buttons on startup
			foreach (string type in System.Enum.GetNames(typeof(MiniGameManager.MiniGameType))) {
				DebugBot.DebugFunctionCall("TMiniGameUI; " + type, debugFunctionCalls);
				GameObject button = (GameObject)Instantiate (buttonDropDownOptionPrefab);
				button.GetComponentInChildren<Text>().text = type;
				string enumType = "";
				enumType = type;
				button.GetComponent<Button>().onClick.AddListener (
					() => {ChooseMiniGame(enumType);}
					);
				button.transform.SetParent(dropDownChooseMiniGamePanel);
			}
			dropDownPopulated = true;
		}

		valuesChanged = false;
		applyPressed = false;

		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TMiniGameUI; CheckActivationCriteria(); ", debugFunctionCalls);
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
		DebugBot.DebugFunctionCall("TMiniGameUI; UpdateUIElementStates(); ", debugFunctionCalls);

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

		// Inputs / Outputs / Options toggles
		if(inputsPanelOn) {
			buttonInputPanel.interactable = false;
			buttonOutputPanel.interactable = true;
			buttonOptionsPanel.interactable = true;
			panelInputsVisible.SetActive (true);
			panelOutputsVisible.SetActive (false);
			panelOptionsVisible.SetActive (false);
		}
		if(outputsPanelOn) {
			buttonInputPanel.interactable = true;
			buttonOutputPanel.interactable = false;
			buttonOptionsPanel.interactable = true;
			panelInputsVisible.SetActive (false);
			panelOutputsVisible.SetActive (true);
			panelOptionsVisible.SetActive (false);
		}
		if(optionsPanelOn) {
			buttonInputPanel.interactable = true;
			buttonOutputPanel.interactable = true;
			buttonOptionsPanel.interactable = false;
			panelInputsVisible.SetActive (false);
			panelOutputsVisible.SetActive (false);
			panelOptionsVisible.SetActive (true);
		}

		// Choose Mini-game Drop-down:
		if(pendingMiniGameManager.gameType == MiniGameManager.MiniGameType.None) {
			textButtonChooseMiniGame.text = "Choose Mini-Game Type (Drop-Down)";
		}
		else {
			textButtonChooseMiniGame.text = pendingMiniGameManager.gameType.ToString();
		}
		// Background Color for pending changes:
		if(applyPressed) {
			bgImage.color = new Color(0.99f, 0.75f, 0.6f);
		}
		else {
			bgImage.color = trainerModuleScript.defaultBGColor;
		}
	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TMiniGameUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		//pendingMiniGameManager.SetMiniGameType(pendingMiniGameManager.gameType); 
		UpdateSelectedChannelCounts();
		textInputSources.text = "Input Sources: " + pendingNumSelectedInputs.ToString() + " / " + pendingMaxSelectedInputs.ToString();
		textOutputActions.text = "Output Actions: " + pendingNumSelectedOutputs.ToString() + " / " + pendingMaxSelectedOutputs.ToString();

		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	private void UpdateSelectedChannelCounts() {
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		// INPUTS!!
		pendingNumSelectedInputs = 0;
		pendingMaxSelectedInputs = currentPlayer.masterPopulation.numInputNodes;
		for(int i = 0; i < pendingMiniGameManager.miniGameInstance.inputChannelsList.Count; i++) {
			if(pendingMiniGameManager.miniGameInstance.inputChannelsList[i].on) {
				pendingNumSelectedInputs++;
			}
		}
		//  OUTPUTS!!
		pendingNumSelectedOutputs = 0;
		pendingMaxSelectedOutputs = currentPlayer.masterPopulation.numOutputNodes;
		for(int i = 0; i < pendingMiniGameManager.miniGameInstance.outputChannelsList.Count; i++) {
			if(pendingMiniGameManager.miniGameInstance.outputChannelsList[i].on) {
				pendingNumSelectedOutputs++;
			}
		}
	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TMiniGameUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		// IF this slot used to be empty (None):
		int numberOfTrials = currentPlayer.masterTrialsList.Count;
		if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType == MiniGameManager.MiniGameType.None) {
			if(pendingMiniGameManager.gameType != MiniGameManager.MiniGameType.None) {  // NEW ADD TRIAL button/row created if this slot went from none-->something !!!!!!
				if(currentPlayer.currentTrialForEdit == (numberOfTrials - 1)) {  // If this is the last item in the Trials array, add a new one with None
					// THIS WILL NEED to eventually handle more than 1 player, loop through playersList or store that info somewhere
					currentPlayer.AddNewTrialRow();
					currentPlayer.hasValidTrials = true;  // !!!!!!!! Update this Stuff later to handle removing/adding Trials
				}
			}
		}
		// If gameType is changing:
		if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType != pendingMiniGameManager.gameType) { // if game-type has changed
			// If GameType is changing AND changing to None:
			if(pendingMiniGameManager.gameType == MiniGameManager.MiniGameType.None) {  // NEW ADD TRIAL button/row created if this slot went from none-->something !!!!!!
				if(currentPlayer.currentTrialForEdit == (numberOfTrials - 2)) {  // If this is the second-to-last item in the Trials array, remove lastIndex
					// THIS WILL NEED to eventually handle more than 1 player, loop through playersList or store that info somewhere
					currentPlayer.RemoveLastTrialRow();
					if(currentPlayer.currentTrialForEdit == 0) { // if this was also the first, no longer
						currentPlayer.hasValidTrials = false;
					}					
				}
			}
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.SetMiniGameType(pendingMiniGameManager.gameType); // Access the MiniGameManager object in the Player's TrialsList Trial and change GameType
			currentPlayer.graphKing.BuildTexturesCurAgentPerTick(currentPlayer, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager, 0);
		}

		trainerModuleScript.gameController.masterTrainer.UpdatePlayingNumTrials();
		trainerModuleScript.panelMenuBarScript.InitializePanelWithTrainerData(); // Update menu-Bar

		CopyInputChannelsList(pendingMiniGameManager.miniGameInstance.inputChannelsList, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.miniGameInstance.inputChannelsList);
		CopyOutputChannelsList(pendingMiniGameManager.miniGameInstance.outputChannelsList, currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.miniGameInstance.outputChannelsList);

		InitializePanelWithTrainerData();
		//trainerModuleScript.ClickBackToTrials();
	}
	
	/*public void ClickChooseMiniGame() {  // Goes to Fitness Function Panel
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickChooseMiniGame(); ", debugFunctionCalls);

		//int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		//Player curPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[dataCurPlayer-1];
	}*/

	public void ChooseMiniGame(string gameType) {  // clicked on a drop-down sub-button:
		DebugBot.DebugFunctionCall("TMiniGameUI; ChooseMiniGame(" + gameType + "); ", debugFunctionCalls);
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(currentPlayer.masterTrialsList.Count-1 < currentPlayer.currentTrialForEdit) {  // if trialsList doesn't have an entry for current Index
			DebugBot.DebugFunctionCall("TMiniGameUI; Index out of RANGE!; ", debugFunctionCalls);
		}
		else { // TrialsList has a valid entry:
			MiniGameManager.MiniGameType parsed_enum = (MiniGameManager.MiniGameType)System.Enum.Parse( typeof( MiniGameManager.MiniGameType ), gameType );

			Trial dataMiniGameTrial = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit];
			if(pendingMiniGameManager.gameType != parsed_enum) { // if the values are different:
				pendingMiniGameManager.gameType = parsed_enum;
				valuesChanged = true;
				pendingMiniGameManager.SetMiniGameType(pendingMiniGameManager.gameType); 
				// Inputs / Outputs button toggles
				InitializePanelInputList();
				InitializePanelOutputList();
				//DebugBot.DebugFunctionCall("TMiniGameUI; VALUES CHANGED", debugFunctionCalls);
			}			 
			//DebugBot.DebugFunctionCall("TMiniGameUI; ChooseMiniGame(" + pendingMiniGameManager.gameType.ToString() + "); " + pendingMiniGameManager.gameType.ToString(), debugFunctionCalls);
		}
		UpdateUIWithCurrentData();
	}

	public void InitializePanelInputList() {
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		DebugBot.DebugFunctionCall("TMiniGameUI; InitializePanelInputList(" + pendingMiniGameManager.miniGameInstance.inputChannelsList.Count.ToString() + "); ", debugFunctionCalls);
		// CLEAR CURRENT LIST:
		foreach (Transform child in inputListTableSpace) {
			GameObject.Destroy(child.gameObject);
		}
		pendingNumSelectedInputs = 0;
		pendingMaxSelectedInputs = currentPlayer.masterPopulation.numInputNodes;
		// Where Am I setting the InputChannelList values????? --answer: inside miniGameInputRowScript.InitializePanelWithTrainerData();
		for(int i = 0; i < pendingMiniGameManager.miniGameInstance.inputChannelsList.Count; i++) {
			GameObject inputListRow = (GameObject)Instantiate (panelMiniGameInputRowPrefab);
			TrainerMiniGameInputRowUI miniGameInputRowScript = inputListRow.GetComponent<TrainerMiniGameInputRowUI>();
			miniGameInputRowScript.inputListIndex = i; // CHANGE LATER!!!!!!!
			miniGameInputRowScript.trainerModuleScript = trainerModuleScript;
			miniGameInputRowScript.trainerMiniGameScript = this;
			miniGameInputRowScript.InitializePanelWithTrainerData();
			inputListRow.transform.SetParent(inputListTableSpace);
		}
	}

	public void InitializePanelOutputList() {
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		// CLEAR CURRENT LIST:
		foreach (Transform child in outputListTableSpace) {
			GameObject.Destroy(child.gameObject);
		}
		pendingNumSelectedOutputs = 0;
		pendingMaxSelectedOutputs = currentPlayer.masterPopulation.numOutputNodes;
		for(int i = 0; i < pendingMiniGameManager.miniGameInstance.outputChannelsList.Count; i++) {
			GameObject outputListRow = (GameObject)Instantiate (panelMiniGameOutputRowPrefab);
			TrainerMiniGameOutputRowUI miniGameOutputRowScript = outputListRow.GetComponent<TrainerMiniGameOutputRowUI>();
			miniGameOutputRowScript.outputListIndex = i; // CHANGE LATER!!!!!!!
			miniGameOutputRowScript.trainerModuleScript = trainerModuleScript;
			miniGameOutputRowScript.trainerMiniGameScript = this;			
			miniGameOutputRowScript.InitializePanelWithTrainerData();
			outputListRow.transform.SetParent(outputListTableSpace);
		}
	}

	public void ClickInputPanel() {
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickInputPanel(); ", debugFunctionCalls);
		inputsPanelOn = true;
		outputsPanelOn = false;
		optionsPanelOn = false;
		UpdateUIWithCurrentData();
	}

	public void ClickOutputPanel() {
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickOutputPanel(); ", debugFunctionCalls);
		inputsPanelOn = false;
		outputsPanelOn = true;
		optionsPanelOn = false;
		UpdateUIWithCurrentData();
	}

	public void ClickOptionsPanel() {
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickOptionsPanel(); ", debugFunctionCalls);
		inputsPanelOn = false;
		outputsPanelOn = false;
		optionsPanelOn = true;
		UpdateUIWithCurrentData();
	}

	public void ClickInputToggleSelectAll() {
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickInputToggleSelectAll(); ", debugFunctionCalls);
	}

	public void ClickOutputToggleSelectAll() {
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickOutputToggleSelectAll(); ", debugFunctionCalls);
	}
	
	public void ClickApply() {
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickApply(); ", debugFunctionCalls);
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
		DebugBot.DebugFunctionCall("TMiniGameUI; ClickCancel(); ", debugFunctionCalls);
		//if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].miniGameManager.gameType != pendingMiniGameManager.gameType) { // if game-type has changed
		//	InitializePanelInputList();
		//	InitializePanelOutputList();
		//	pendingMiniGameManager.SetMiniGameType(pendingMiniGameManager.gameType);
		//}
		InitializePanelWithTrainerData();
		//DebugFunctionCall("ClickCancel()");
	}

	public void CopyInputChannelsList(List<BrainInputChannel> source, List<BrainInputChannel> target) {		// Maybe update this by making it a function of BrainInputChannel that takes an instance of itself
		// Check if both are the same length?
		int numInputs = source.Count;
		if(numInputs == target.Count) {
			for(int i = 0; i < numInputs; i++) {
				string newName = "";
				newName = source[i].channelName;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].channelName = newName;
				float newValue = 0f;
				newValue = source[i].channelValue[0];  // Make sure these are allocating new memory and will be copies, not references!
				target[i].channelValue[0] = newValue;
				bool newOn = false;
				newOn = source[i].on;   // Make sure these are allocating new memory and will be copies, not references!
				target[i].on = newOn;
				DebugBot.DebugFunctionCall("TMiniGameUI; CopyInputChannelsList: " + source[i].on.ToString() + ", targ: " + target[i].on.ToString(), debugFunctionCalls);
			}
		}
		else {
			DebugBot.DebugFunctionCall("TMiniGameUI; CopyInputChannelsList(); Arrays of Different Length!", debugFunctionCalls);
		}
	}
	
	public void CopyOutputChannelsList(List<BrainOutputChannel> source, List<BrainOutputChannel> target) {		
		// Check if both are the same length?
		int numOutputs = source.Count;
		if(numOutputs == target.Count) {
			for(int i = 0; i < numOutputs; i++) {
				string newName = "";
				newName = source[i].channelName;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].channelName = newName;
				float newValue = 0f;
				newValue = source[i].channelValue[0];  // Make sure these are allocating new memory and will be copies, not references!
				target[i].channelValue[0] = newValue;
				bool newOn = false;
				newOn = source[i].on;   // Make sure these are allocating new memory and will be copies, not references!
				target[i].on = newOn;
			}
		}
		else {
			DebugBot.DebugFunctionCall("TMiniGameUI; CopyOutputChannelsList(); Arrays of Different Length!", debugFunctionCalls);
		}
	}
}
