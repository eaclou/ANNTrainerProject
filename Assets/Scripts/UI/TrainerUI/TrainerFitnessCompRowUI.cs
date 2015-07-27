using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerFitnessCompRowUI : MonoBehaviour {

	public bool debugFunctionCalls = true;

	public bool inBrainList;
	public int fitnessIndex;
	
	public TrainerModuleUI trainerModuleScript;
	public TrainerFitnessUI trainerFitnessScript;
	public FitnessManager fitnessManagerRef;

	public Toggle toggleComponentSelected;
	public Text textFitnessComponentName;

	public Toggle toggleBigIsBetter;
	public Slider sliderPower;
	public Text textPower;
	public Slider sliderWeight;
	public Text textWeight;
	//public Image bgImage;

	//public FitnessComponent pendingFitnessComp;
	public bool pendingChannelOn;
	public bool pendingBigIsBetter;
	public float pendingPower;
	public float pendingWeight;
	public string pendingComponentName;

	// UI Settings:
	//private string fitnessComponentType = "none";
	private bool biggerIsBetter = true;
	private float minPower = 0f;
	private float maxPower = 4f;
	private float minWeight = 0f;
	private float maxWeight = 1f;
	
	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);


		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(fitnessManagerRef == null) {
			//fitnessManagerRef = new FitnessManager(currentPlayer);
		}

		if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager != null) { // error catch  -- look into this later
			if(inBrainList) {
				pendingChannelOn = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].on;
				toggleComponentSelected.isOn = pendingChannelOn;
				pendingBigIsBetter = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].bigIsBetter;
				toggleBigIsBetter.isOn = pendingBigIsBetter;

				pendingPower = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].power;
				pendingWeight = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].weight;
			}
			else {
				pendingChannelOn = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].on;
				toggleComponentSelected.isOn = pendingChannelOn;
				pendingBigIsBetter = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].bigIsBetter;
				toggleBigIsBetter.isOn = pendingBigIsBetter;

				pendingPower = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].power;
				pendingWeight = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].weight;
			}
			sliderPower.minValue = minPower; // set up slider bounds
			sliderPower.maxValue = maxPower;
			sliderWeight.minValue = minWeight; // set up slider bounds
			sliderWeight.maxValue = maxWeight;

		}
	
		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; CheckActivationCriteria(); ", debugFunctionCalls);
				
	}
	
	public void UpdateUIElementStates() {

		// Changing Button Displays !!
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(inBrainList) {
			if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].componentName == null) {  // If no Trial for this row:
				textFitnessComponentName.text = "Fitness Component Name";
				//buttonAddEditTrial.GetComponentsInChildren<Text>()
			}
			else {
				textFitnessComponentName.text = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].componentName;  // set to mini-game type
			}
		}
		else {
			if(currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].componentName == null) {  // If no Trial for this row:
				textFitnessComponentName.text = "Fitness Component Name";
				//buttonAddEditTrial.GetComponentsInChildren<Text>()
			}
			else {
				textFitnessComponentName.text = currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].componentName;  // set to mini-game type
			}
		}

	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(inBrainList) {
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].on = pendingChannelOn;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].componentName = pendingComponentName;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].bigIsBetter = pendingBigIsBetter;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].power = pendingPower;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.brainFitnessComponentList[fitnessIndex].weight = pendingWeight;
		}
		else {
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].on = pendingChannelOn;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].componentName = pendingComponentName;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].bigIsBetter = pendingBigIsBetter;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].power = pendingPower;
			currentPlayer.masterTrialsList[currentPlayer.currentTrialForEdit].fitnessManager.gameFitnessComponentList[fitnessIndex].weight = pendingWeight;
		}
		InitializePanelWithTrainerData();
	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);

		toggleComponentSelected.isOn = pendingChannelOn;
		toggleBigIsBetter.isOn = pendingBigIsBetter;
		sliderPower.value = pendingPower;
		textPower.text = pendingPower.ToString();
		sliderWeight.value = pendingWeight;
		textWeight.text = pendingWeight.ToString();

		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	#region OnClick & UIElement changed Functions:

	public void ToggleChannel(bool toggle) {
		//DebugBot.DebugFunctionCall("TMiniGameInputRowUI; ToggleChannel(); " + inputListIndex.ToString() + ", " + toggle.ToString(), true);

		pendingChannelOn = toggle;
		trainerFitnessScript.pendingFitnessManager.masterFitnessCompList[fitnessIndex].on = toggle;
		trainerFitnessScript.valuesChanged = true;

		trainerFitnessScript.UpdateUIWithCurrentData();
	}
	
	public void ToggleBigIsBetter(bool toggle) {
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; ToggleBigIsBetter(); ", debugFunctionCalls);
		//pendingBigIsBetter = value;
		//trainerFitnessScript.valuesChanged = true;
		//trainerFitnessScript.UpdateUIWithCurrentData();
		//UpdateUIWithCurrentData();

		pendingBigIsBetter = toggle;
		trainerFitnessScript.pendingFitnessManager.masterFitnessCompList[fitnessIndex].bigIsBetter = toggle;
		trainerFitnessScript.valuesChanged = true;
		trainerFitnessScript.UpdateUIWithCurrentData();
			
	}

	public void SliderPower(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; SliderPower(); ", debugFunctionCalls);
		pendingPower = sliderValue;
		trainerFitnessScript.pendingFitnessManager.masterFitnessCompList[fitnessIndex].power = pendingPower;
		trainerFitnessScript.valuesChanged = true;
		trainerFitnessScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	public void SliderWeight(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TFitnessCompRowUI; SliderWeight(); ", debugFunctionCalls);
		pendingWeight = sliderValue;
		trainerFitnessScript.pendingFitnessManager.masterFitnessCompList[fitnessIndex].weight = pendingWeight;
		trainerFitnessScript.valuesChanged = true;
		trainerFitnessScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	#endregion
}
