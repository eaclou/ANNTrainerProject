using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerMiniGameOutputRowUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public int outputListIndex;
	
	public TrainerModuleUI trainerModuleScript;
	public TrainerMiniGameUI trainerMiniGameScript;
	public Toggle toggleOutputSource;
	public Text textOutputSourceName;
	public Button buttonMapKeys;
	//public Image bgImage;

	//public int pendingNumNodes;
	public bool pendingChannelOn;

	// UI Settings:
	//private int minNumNodes = 1;
	//private int maxNumNodes = 4;
	
	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TMiniGameOutputRowUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		pendingChannelOn = trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.outputChannelsList[outputListIndex].on;
		toggleOutputSource.isOn = pendingChannelOn;
	
		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		DebugBot.DebugFunctionCall("TMiniGameOutputRowUI; CheckActivationCriteria(); ", debugFunctionCalls);
				
	}
	
	public void UpdateUIElementStates() {

		// Changing Button Displays !!
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		textOutputSourceName = toggleOutputSource.transform.GetComponentInChildren<Text>(); // Is this line needed?
		textOutputSourceName.text = trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.outputChannelsList[outputListIndex].channelName; // set display name

		DebugBot.DebugFunctionCall("TMiniGameOutputRowUI; UpdateUIElementStates(); " + textOutputSourceName.text.ToString(), debugFunctionCalls);

	}
	
	public void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TMiniGameOutputRowUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);

		toggleOutputSource.isOn = pendingChannelOn;

		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	#region OnClick & UIElement changed Functions:

	public void ToggleChannel(bool toggle) {
		//DebugBot.DebugFunctionCall("TMiniGameOutputRowUI; ToggleChannel(); " + outputListIndex.ToString() + ", " + toggle.ToString(), true);

		if(toggle) { // if Selecting:
			if(trainerMiniGameScript.pendingNumSelectedOutputs >= trainerMiniGameScript.pendingMaxSelectedOutputs) {  // if not enough outputChannels in brain:
				toggleOutputSource.isOn = false;  // 
			}
			else {
				pendingChannelOn = toggle;
				trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.outputChannelsList[outputListIndex].on = toggle;
				trainerMiniGameScript.valuesChanged = true;
			}
		}
		else {
			pendingChannelOn = toggle;
			trainerMiniGameScript.pendingMiniGameManager.miniGameInstance.outputChannelsList[outputListIndex].on = toggle;
			trainerMiniGameScript.valuesChanged = true;
		}
		trainerMiniGameScript.UpdateUIWithCurrentData();
	}

	#endregion
}
