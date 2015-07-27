using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerMiniGameInputRowUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public int inputListIndex;
	
	public TrainerModuleUI trainerModuleScript;
	public TrainerMiniGameUI trainerMiniGameScript;
	public Toggle toggleInputSource;
	public Text textInputSourceName;
	public Slider sliderNumNodes;
	public Text textNumNodesValue;
	//public Image bgImage;

	public int pendingNumNodes;
	public bool pendingChannelOn;

	// UI Settings:
	private int minNumNodes = 1;
	private int maxNumNodes = 4;
	
	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TMiniGameInputRowUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		sliderNumNodes.minValue = minNumNodes; // set up slider bounds
		sliderNumNodes.maxValue = maxNumNodes;
		pendingChannelOn = trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.inputChannelsList[inputListIndex].on;
		toggleInputSource.isOn = pendingChannelOn;

		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TMiniGameInputRowUI; CheckActivationCriteria(); ", debugFunctionCalls);
				
	}
	
	public void UpdateUIElementStates() {

		// Changing Button Displays !!
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		textInputSourceName = toggleInputSource.transform.GetComponentInChildren<Text>();
		textInputSourceName.text = trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.inputChannelsList[inputListIndex].channelName; // set display name

		//textAddEditTrial.text = currentPlayer.masterTrialsList[trialIndex].gameType.ToString();  // set to mini-game type

		DebugBot.DebugFunctionCall("TMiniGameInputRowUI; UpdateUIElementStates(); ", debugFunctionCalls);
		//bgImage.color = trainerModuleScript.defaultBGColor;

	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TMiniGameInputRowUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);

		//toggleInputSource.isOn = pendingChannelOn;
		sliderNumNodes.value = pendingNumNodes;
		textNumNodesValue.text = pendingNumNodes.ToString();

		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	#region OnClick & UIElement changed Functions:
	
	public void SliderNumNodes(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TMiniGameInputRowUI; SliderNumPlays(); ", debugFunctionCalls);
		pendingNumNodes = (int)sliderValue;
		trainerMiniGameScript.valuesChanged = true;
		trainerMiniGameScript.UpdateUIWithCurrentData();
		UpdateUIWithCurrentData();
	}

	public void ToggleChannel(bool toggle) {
		//DebugBot.DebugFunctionCall("TMiniGameInputRowUI; ToggleChannel(); " + inputListIndex.ToString() + ", " + toggle.ToString(), true);

		if(toggle) { // if Selecting:
			if(trainerMiniGameScript.pendingNumSelectedInputs >= trainerMiniGameScript.pendingMaxSelectedInputs) {  // if not enough inputChannels in brain:
				toggleInputSource.isOn = false;  // 
			}
			else {
				pendingChannelOn = toggle;
				trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.inputChannelsList[inputListIndex].on = toggle;
				trainerMiniGameScript.valuesChanged = true;
			}
		}
		else {
			pendingChannelOn = toggle;
			trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.inputChannelsList[inputListIndex].on = toggle;
			trainerMiniGameScript.valuesChanged = true;
		}
		trainerMiniGameScript.UpdateUIWithCurrentData();
	}

	#endregion
}
