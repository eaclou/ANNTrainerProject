using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrainerTrialsUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public TrainerModuleUI trainerModuleScript;
	public TrainerTrialRowUI trainerTrialRowScript;
	public GameObject panelDock;
	[SerializeField] GameObject trialRowPrefab;
	public GameObject panelVisible;
	public Button buttonApply;
	public Button buttonCancel;
	public Image bgImage;

	private Population populationRef;

	//public pending;
	private bool trialRowsPopulated = false;

	private List<TrainerTrialRowUI> pendingTrialRowsList;

	// UI Settings:	
	private bool panelActive = false;  // requires valid population
	public bool valuesChanged = false;
	public bool applyPressed = false;
	
	public void InitializePanelWithTrainerData() {

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		DebugBot.DebugFunctionCall("TTrialsUI; InitializePanelWithTrainerData(); " + currentPlayer.masterTrialsList.Count.ToString(), debugFunctionCalls);

		InitializePanelTrialsList();
		valuesChanged = false;
		applyPressed = false;	
		UpdateUIWithCurrentData();
	}

	public void InitializePanelTrialsList() {
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		// CLEAR CURRENT LIST:
		foreach (Transform child in panelDock.transform) {
			GameObject.Destroy(child.gameObject);
		}
		//pendingNumSelectedInputs = 0;
		//pendingMaxSelectedInputs = currentPlayer.MasterPopulation.numInputNodes;
		pendingTrialRowsList = new List<TrainerTrialRowUI>();
		for(int i = 0; i < currentPlayer.masterTrialsList.Count; i++) {
			GameObject trialsListRow = (GameObject)Instantiate (trialRowPrefab);
			TrainerTrialRowUI miniGameTrialRowScript = trialsListRow.GetComponent<TrainerTrialRowUI>();
			miniGameTrialRowScript.trialIndex = i;
			miniGameTrialRowScript.trainerModuleScript = trainerModuleScript;
			miniGameTrialRowScript.trainerTrialsScript = this;
			pendingTrialRowsList.Add (miniGameTrialRowScript);
			
			miniGameTrialRowScript.buttonAddEditTrial.onClick.AddListener (
				() => {ClickEditTrial(miniGameTrialRowScript.trialIndex); trainerModuleScript.ClickAddTrial();}
			);
			miniGameTrialRowScript.InitializePanelWithTrainerData();
			trialsListRow.transform.SetParent(panelDock.transform);
		}
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TTrialsUI; CheckActivationCriteria(); ", debugFunctionCalls);
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
		DebugBot.DebugFunctionCall("TTrialsUI; UpdateUIElementStates(); ", debugFunctionCalls);
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
		if(applyPressed) {
			bgImage.color = new Color(0.99f, 0.75f, 0.6f);
		}
		else {
			bgImage.color = trainerModuleScript.defaultBGColor;
		}
	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TTrialsUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TTrialsUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;


		for(int i = 0; i < pendingTrialRowsList.Count; i++) {  // loop through players
			pendingTrialRowsList[i].SetTrainerDataFromUIApply();			
		}

		trainerModuleScript.gameController.masterTrainer.UpdatePlayingNumTrialRounds();
		trainerModuleScript.gameController.masterTrainer.UpdatePlayingNumTimeSteps();
		InitializePanelWithTrainerData();
	}

	public void ClickEditTrial(int trialNum) {  // Goes to Fitness Function Panel
		DebugBot.DebugFunctionCall("TTrialsUI; ClickEditTrial(); ", debugFunctionCalls);
		//Debug.Log ("PlayersButtonPrevPlayer, dataCP: " + dataCurPlayer.ToString() + ", dataNP: " + dataNumPlayers.ToString() + ", pendCP: " + pendingCurPlayer.ToString() + ", pendNP: " + pendingNumPlayers.ToString());
		//int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		//int dataNumPlayers = trainerModuleScript.gameController.masterTrainer.NumPlayers;
		//if(dataNumPlayers > 1) {   // if one player then no change
		//	dataCurPlayer--;
		//	if(dataCurPlayer < 1) {
		//		dataCurPlayer = dataNumPlayers;
		//	}
		//	trainerModuleScript.gameController.masterTrainer.CurPlayer = dataCurPlayer;
		//	UpdateUIWithCurrentData();
		//}
		int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		Player curPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[dataCurPlayer-1];
		curPlayer.currentTrialForEdit = trialNum; // UPDATE THIS when multi-Trials is established !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	}

	public void ClickApply() {
		DebugBot.DebugFunctionCall("TTrialsUI; ClickApply(); ", debugFunctionCalls);
		applyPressed = true;
		UpdateUIElementStates();  // change background color to indicate pending changes
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		
		// !!!!!!!!!!! TEMPORARY !!!!!! Replace this code once play/pause/fastMode etc. are in and the Trainer class will trigger this when ApplyCriteria are met
		if(trainer.applyPanelTrials) {  // if apply criteria are met currently:
			SetTrainerDataFromUIApply();
		}
		//DebugFunctionCall("ClickApply()");
	}
	public void ClickCancel() {
		DebugBot.DebugFunctionCall("TTrialsUI; ClickCancel(); ", debugFunctionCalls);
		InitializePanelWithTrainerData();
		//DebugFunctionCall("ClickCancel()");
	}
}
