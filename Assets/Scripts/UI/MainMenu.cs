using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public GameController gameController;

	public GameObject trainerMenu;
	public GameObject analyzerMenu;
	public GameObject settingsMenu;

	void Start() {
		// TEMP !!! Using for testing population functions!
		/*Debug.Log ("MainMenu START()!");
		Population testPopOne = new Population();
		testPopOne.populationMaxSize = 10;
		//testPopOne.PrintSettings();
		testPopOne.ChangeTemplateBrainType(Population.BrainType.ANN_FF_Layered_AllToAll);
		testPopOne.numInputNodes = 2;
		testPopOne.numOutputNodes = 2;
		testPopOne.InitializeMasterAgentArray(); // Should this be rolled into Population constructor so nothing is = null? even if its 4 agents with None braintype?

		//testPopOne.PrintSettings();
		testPopOne.masterAgentArray[0].fitnessScore = 2.33331314f;


		//testPopOne.PrintSettings();

		//Debug.Log ("!!!!! REF POPULATION !!!!!:");
		//Population testPopTwo = testPopOne.RefPopulation();
		//testPopTwo.PrintSettings();

		//testPopTwo.numInputNodes = 5;
		//testPopTwo.brainType = Population.BrainType.Test;
		//testPopTwo.PrintSettings();
		//Debug.Log ("pop One:");
		//testPopOne.PrintSettings();


		//Debug.Log ("!!!!! COPY POPULATION !!!!!:");
		Population testPopThree = testPopOne.RefPopulation();
		//Debug.Log ("pop Three:");
		//testPopThree.PrintSettings();
		
		//testPopThree.numInputNodes = 13;
		//testPopThree.brainType = Population.BrainType.Test;
		Debug.Log ("pop Three:");
		testPopThree.PrintSettings();
		Debug.Log ("pop One:");
		testPopOne.PrintSettings();
		Population[] popArray = testPopThree.SplitPopulation(3);
		Debug.Log ("Split Pops:");
		for(int i = 0; i < popArray.Length; i++) {
			popArray[i].PrintSettings();
		}*/
	}

	public void InitializeMainMenu() {
		gameObject.SetActive (true); // UnHide main menu
		
		gameController.mainMenuOn = true;  // keep track of what module the game is currently in
		gameController.trainerModuleOn = false;
		gameController.analyzerModuleOn = false;
		gameController.settingsModuleOn = false;
		DebugFunctionCall("InitializeMainMenu()");
	}

	public void BackToMainMenu(Object module) {  // Called by buttons in other modules that pass their canvas through the function
		GameObject moduleMenu = module as GameObject;
		gameObject.SetActive (true); // UnHide main menu
		moduleMenu.SetActive(false);  // Hide menu where user came from

		gameController.mainMenuOn = true;  // keep track of what module the game is currently in
		gameController.trainerModuleOn = false;
		gameController.analyzerModuleOn = false;
		gameController.settingsModuleOn = false;
		DebugFunctionCall("BackToMainMenu()");
	}

	public void ClickTrainer() {
		trainerMenu.SetActive (true);
		gameObject.SetActive(false);
		analyzerMenu.SetActive (false);
		settingsMenu.SetActive (false);

		gameController.mainMenuOn = false;  // keep track of what module the game is currently in
		gameController.trainerModuleOn = true;
		gameController.analyzerModuleOn = false;
		gameController.settingsModuleOn = false;
		DebugFunctionCall("ClickTrainer()");
	}

	public void ClickAnalyzer() {
		analyzerMenu.SetActive (true);
		gameObject.SetActive(false);
		trainerMenu.SetActive (false);
		settingsMenu.SetActive (false);

		gameController.mainMenuOn = false;  // keep track of what module the game is currently in
		gameController.trainerModuleOn = false;
		gameController.analyzerModuleOn = true;
		gameController.settingsModuleOn = false;
		DebugFunctionCall("ClickAnalyzer()");
	}

	public void ClickSettings() {
		settingsMenu.SetActive (true);
		gameObject.SetActive(false);
		trainerMenu.SetActive (false);
		analyzerMenu.SetActive (false);

		gameController.mainMenuOn = false;  // keep track of what module the game is currently in
		gameController.trainerModuleOn = false;
		gameController.analyzerModuleOn = false;
		gameController.settingsModuleOn = true;
		DebugFunctionCall("ClickSettings()");
	}

	public void ClickQuit() {
		DebugFunctionCall("ClickQuit()");
		//Application.Quit;
	}

	private void DebugFunctionCall(string functionName) {
		if(debugFunctionCalls) {
			Debug.Log (functionName);
		}
	}
}
