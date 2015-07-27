using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerTrialRowUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public int trialIndex;
	
	public TrainerModuleUI trainerModuleScript;
	public TrainerTrialsUI trainerTrialsScript;
	public Button buttonAddEditTrial;
	public Text textAddEditTrial;
	public Slider sliderNumPlays;
	public Text textNumPlays;
	public Slider sliderEvalTime;
	public Text textEvalTime;
	public Slider sliderPower;
	public Text textPower;
	public Slider sliderWeight;
	public Text textWeight;
	public Button buttonRemoveRow;
	//public Image bgImage;

	public int pendingNumPlays;
	public int pendingEvalTime;
	public float pendingPower;
	public float pendingWeight;

	// UI Settings:
	private int minNumPlays = 1;
	private int maxNumPlays = 8;
	private int minEvalTime = 1;
	private int maxEvalTime = 800;
	private float minPower = 0f;
	private float maxPower = 4f;
	private float minWeight = 0f;
	private float maxWeight = 1f;
	
	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TTrialRowUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(currentPlayer.masterTrialsList[trialIndex] != null) { // error catch
			//textCurrentPopulationSize.text = "Current Population Size: " + (populationRef.isFunctional ? populationRef.masterAgentArray.Length.ToString() : "0"); // Update this later!!
			//Current Max Population Size:
			sliderNumPlays.minValue = minNumPlays; // set up slider bounds
			sliderNumPlays.maxValue = maxNumPlays;
			pendingNumPlays = currentPlayer.masterTrialsList[trialIndex].numberOfPlays;
			sliderEvalTime.minValue = minEvalTime; // set up slider bounds
			sliderEvalTime.maxValue = maxEvalTime;
			pendingEvalTime = currentPlayer.masterTrialsList[trialIndex].maxEvaluationTimeSteps;
			sliderPower.minValue = minPower; // set up slider bounds
			sliderPower.maxValue = maxPower;
			pendingPower = currentPlayer.masterTrialsList[trialIndex].power;
			sliderWeight.minValue = minWeight; // set up slider bounds
			sliderWeight.maxValue = maxWeight;
			pendingWeight = currentPlayer.masterTrialsList[trialIndex].weight;


		}
	
		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TTrialRowUI; CheckActivationCriteria(); ", debugFunctionCalls);
				
	}
	
	public void UpdateUIElementStates() {

		// Changing Button Displays !!
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(currentPlayer.masterTrialsList[trialIndex].miniGameManager.gameType == MiniGameManager.MiniGameType.None) {  // If no Trial for this row:
			textAddEditTrial.text = "Add Trial";
			//buttonAddEditTrial.GetComponentsInChildren<Text>()
		}
		else {
			textAddEditTrial.text = currentPlayer.masterTrialsList[trialIndex].miniGameManager.gameType.ToString();  // set to mini-game type
		}
		DebugBot.DebugFunctionCall("TTrialRowUI; UpdateUIElementStates(); " + trialIndex.ToString() + ", " + currentPlayer.masterTrialsList[trialIndex].miniGameManager.gameType.ToString(), debugFunctionCalls);
		//bgImage.color = trainerModuleScript.defaultBGColor;

	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TTrialRowUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
	

		currentPlayer.masterTrialsList[trialIndex].maxEvaluationTimeSteps = pendingEvalTime;
		currentPlayer.masterTrialsList[trialIndex].numberOfPlays = pendingNumPlays;
		currentPlayer.masterTrialsList[trialIndex].power = pendingPower;
		currentPlayer.masterTrialsList[trialIndex].weight = pendingWeight;


		InitializePanelWithTrainerData();
	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TTrialRowUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);

		sliderNumPlays.value = pendingNumPlays;
		textNumPlays.text = pendingNumPlays.ToString();
		sliderEvalTime.value = pendingEvalTime;
		textEvalTime.text = pendingEvalTime.ToString();
		sliderPower.value = pendingPower;
		textPower.text = pendingPower.ToString();
		sliderWeight.value = pendingWeight;
		textWeight.text = pendingWeight.ToString();

		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	#region OnClick & UIElement changed Functions:
	
	public void SliderNumPlays(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TTrialRowUI; SliderNumPlays(); ", debugFunctionCalls);
		pendingNumPlays = (int)sliderValue;
		trainerTrialsScript.valuesChanged = true;
		trainerTrialsScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	public void SliderEvalTime(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TTrialRowUI; SliderEvalTime(); ", debugFunctionCalls);
		pendingEvalTime = (int)sliderValue;
		trainerTrialsScript.valuesChanged = true;
		trainerTrialsScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	public void SliderPower(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TTrialRowUI; SliderPower(); ", debugFunctionCalls);
		pendingPower = sliderValue;
		trainerTrialsScript.valuesChanged = true;
		trainerTrialsScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	public void SliderWeight(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TTrialRowUI; SliderWeight(); ", debugFunctionCalls);
		pendingWeight = sliderValue;
		trainerTrialsScript.valuesChanged = true;
		trainerTrialsScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	#endregion
}
