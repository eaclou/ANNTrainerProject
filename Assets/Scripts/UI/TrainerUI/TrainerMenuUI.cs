using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerMenuUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public TrainerModuleUI trainerModuleScript;
	public Button buttonPopulation;
	public Button buttonLoadPopulation;
	public Button buttonSavePopulation;
	public Button buttonMainMenu;
	public Image bgImage;

	private bool populationActive = false;
	private bool loadPopulationActive = false;
	private bool savePopulationActive = false;
	private bool mainMenuActive = true;


	public void InitializePanelWithTrainerData() {
		DebugBot.DebugFunctionCall("TMenuUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	public void CheckActivationCriteria() {
		DebugBot.DebugFunctionCall("TMenuUI; CheckActivationCriteria(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		// Initialize values as false:
		populationActive = false;
		loadPopulationActive = false;
		savePopulationActive = false;

		// Calculate Criteria:		
		//Population:
		if(!trainer.IsPlaying) {  // Can't be in the middle of a game
			populationActive = true;
		}
		//Load Population:
		loadPopulationActive = false;  // Change this when Implementing Loading!
		//Save Population:
		if(trainer.PlayerList != null) {
			int curPlayer = trainer.CurPlayer;
			if(trainer.PlayerList[curPlayer-1].masterPopulation != null) {
				if(trainer.PlayerList[curPlayer-1].masterPopulation.isFunctional == true) {
					savePopulationActive = true;
				}
			}
			else {
				// Debug Class Message! Population is NULL!
			}
		}
		else {
			// Debug Class Message! PlayerList is NULL!
		}
		mainMenuActive = true;  // Maybe change this in the future if needed, for now, always active.
	}

	public void UpdateUIElementStates() {
		DebugBot.DebugFunctionCall("TMenuUI; UpdateUIElementStates(); ", debugFunctionCalls);
		Trainer trainer = trainerModuleScript.gameController.masterTrainer;
		// Changing Button Displays !!
		// Menu Button:

		// Population Button:
		if(populationActive) {
			buttonPopulation.interactable = true;
		}
		else {
			buttonPopulation.interactable = false;
		}
		// Load Population Button:
		if(loadPopulationActive) {
			buttonLoadPopulation.interactable = true;
		}
		else {
			buttonLoadPopulation.interactable = false;
		}
		// Save Population Button:
		if(savePopulationActive) {
			buttonSavePopulation.interactable = true;
		}
		else {
			buttonSavePopulation.interactable = false;
		}
		buttonMainMenu.interactable = true;
	}
}
