using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	// Public Variables:
	public bool debugFunctionCalls = false; // turns on/off debug log messages

	public MainMenu mainMenu;  // This is the main menu panel script
	public TrainerModuleUI trainerUI;  // This is the script that handles all of the Trainer module UI
	public Trainer masterTrainer;  // This is the Trainer object that handles all of the data for the Trainer Module


	// Module States:
	public bool mainMenuOn = true;
	public bool trainerModuleOn = false;
	public bool analyzerModuleOn = false;
	public bool settingsModuleOn = false;

	private int fixedUpdateCalls = 0;

	void Awake () {
		DebugBot.DebugFunctionCall("GameController; Awake(); ", debugFunctionCalls);
		Time.timeScale = 0.00f;
		mainMenu.InitializeMainMenu();
	}

	// Use this for initialization
	void Start () {
		DebugBot.DebugFunctionCall("GameController; Start(); ", debugFunctionCalls);
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//CheckForPendingChangesAllowed();
		if(fixedUpdateCalls > 0) {
			masterTrainer.CalculateOneStep();
		}
		if(masterTrainer != null) {
			if(masterTrainer.IsPlaying) {

				/*if(masterTrainer.FastModeOn) {  // playing ON and fastmode ON
					//play FastMode
					masterTrainer.PlayFastModeChunk(8000);
				}
				else {   // playing ON and fastmode OFF

					masterTrainer.PlayRealTimeStep();
				}*/
			}
		}
		fixedUpdateCalls++;
	}

	void Update () {
		//updateCalls++;
		//Debug.Log ("UPDATE! #: " + updateCalls.ToString());
	}

	public void OpenTrainerModule() {
		DebugBot.DebugFunctionCall("GameController; OpenTrainerModule(); ", debugFunctionCalls);
		if(masterTrainer == null) {  // first time running program
			masterTrainer = new Trainer();
			masterTrainer.gameControllerRef = this;
		}
		trainerUI.InitializeModuleUI();  // Sets up all panel visibility, activa/inactive states, and UI element values when module is opened

	}
}
