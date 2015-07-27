using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerPlayersUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public TrainerModuleUI trainerModuleScript;
	public Slider sliderNumPlayers;
	public Text textNumPlayers;
	public Text textCurPlayer;
	public Button buttonApply;
	public Button buttonCancel;
	public Image bgImage;

	public int pendingNumPlayers;

	public bool valuesChanged = false;
	public bool applyPressed = false;

	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TPlayersUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		pendingNumPlayers = trainer.NumPlayers;
		sliderNumPlayers.minValue = 1;
		sliderNumPlayers.maxValue = trainer.MaxPlayers;	
		valuesChanged = false;
		applyPressed = false;
		UpdateUIWithCurrentData();
	}

	public void SetTrainerDataFromUIApply() {
		DebugBot.DebugFunctionCall("TPlayersUI; SetTrainerDataFromUIApply(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		trainer.NumPlayers = pendingNumPlayers;
		trainer.AddPlayer ();
		InitializePanelWithTrainerData();
	}

	private void UpdateUIElementStates() {
		DebugBot.DebugFunctionCall("TPlayersUI; UpdateUIElementStates(); ", debugFunctionCalls);
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

	private void UpdateUIWithCurrentData() {
		DebugBot.DebugFunctionCall("TPlayersUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		sliderNumPlayers.value = pendingNumPlayers;		
		textNumPlayers.text = pendingNumPlayers.ToString();
		textCurPlayer.text = "PLAYER: " + dataCurPlayer.ToString();
		UpdateUIElementStates();
	}

	#region UI Click and OnChanged Functions:

	public void SliderNumPlayersChanged(float sliderValue) { // On Slider Value Changed
		DebugBot.DebugFunctionCall("TPlayersUI; SliderNumPlayersChanged(); ", debugFunctionCalls);
		pendingNumPlayers = (int)sliderValue;
		int dataNumPlayers = trainerModuleScript.gameController.masterTrainer.NumPlayers;
		if(pendingNumPlayers != dataNumPlayers) {
			valuesChanged = true;
		}
		else {
			valuesChanged = false;
		}
		UpdateUIWithCurrentData();  // Will update text display of PENDING numPlayers value (NOT the applied value!)
	}
	
	public void ClickNextPlayer() {
		DebugBot.DebugFunctionCall("TPlayersUI; ClickNextPlayer(); ", debugFunctionCalls);
		//Debug.Log ("PlayersButtonPrevPlayer, dataCP: " + dataCurPlayer.ToString() + ", dataNP: " + dataNumPlayers.ToString() + ", pendCP: " + pendingCurPlayer.ToString() + ", pendNP: " + pendingNumPlayers.ToString());
		int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		int dataNumPlayers = trainerModuleScript.gameController.masterTrainer.NumPlayers;
		if(dataNumPlayers > 1) {   // if one player then no change
			dataCurPlayer++;
			if(dataCurPlayer > dataNumPlayers) {
				dataCurPlayer = 1;
			}
			trainerModuleScript.gameController.masterTrainer.CurPlayer = dataCurPlayer;
			UpdateUIWithCurrentData();
		}
	}
	
	public void ClickPrevPlayer() {
		DebugBot.DebugFunctionCall("TPlayersUI; ClickPrevPlayer(); ", debugFunctionCalls);
		//Debug.Log ("PlayersButtonPrevPlayer, dataCP: " + dataCurPlayer.ToString() + ", dataNP: " + dataNumPlayers.ToString() + ", pendCP: " + pendingCurPlayer.ToString() + ", pendNP: " + pendingNumPlayers.ToString());
		int dataCurPlayer = trainerModuleScript.gameController.masterTrainer.CurPlayer;
		int dataNumPlayers = trainerModuleScript.gameController.masterTrainer.NumPlayers;
		if(dataNumPlayers > 1) {   // if one player then no change
			dataCurPlayer--;
			if(dataCurPlayer < 1) {
				dataCurPlayer = dataNumPlayers;
			}
			trainerModuleScript.gameController.masterTrainer.CurPlayer = dataCurPlayer;
			UpdateUIWithCurrentData();
		}
	}

	public void ClickApply() {
		DebugBot.DebugFunctionCall("TPlayersUI; ClickApply(); ", debugFunctionCalls);
		applyPressed = true;
		UpdateUIElementStates();  // change background color to indicate pending changes
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		// CHECK for removing an "active" player, need to give pop-up warning in that case
		if(trainer.CurPlayer > pendingNumPlayers) {
			trainer.CurPlayer = pendingNumPlayers;
			//DebugFunctionCall("ClickApply(), Deleted One or More Players!");
		}
		// !!!!!!!!!!! TEMPORARY !!!!!! Replace this code once play/pause/fastMode etc. are in and the Trainer class will trigger this when ApplyCriteria are met
		if(trainer.applyPanelPlayers) {  // if apply criteria are met currently:
			SetTrainerDataFromUIApply();
		}
	}
	public void ClickCancel() {
		DebugBot.DebugFunctionCall("TPlayersUI; ClickCancel(); ", debugFunctionCalls);
		InitializePanelWithTrainerData();
	}

	#endregion

}
