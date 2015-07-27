using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class TrainerTextLogUI : MonoBehaviour {

	public TrainerModuleUI trainerModuleScript;
	public Text logText;
	public Image bgImage;
	public string textData;
	public Trainer trainer;
	public Player currentPlayer;

	public void InitializePanelWithTrainerData() {	
		trainer = trainerModuleScript.gameController.masterTrainer;
		currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];

		textData = Time.timeScale.ToString() + "\n" + "Input Nodes: " + currentPlayer.masterPopulation.numInputNodes + "\nOutput Nodes: " + currentPlayer.masterPopulation.numOutputNodes + "\n" + trainer.GetCurrentGamePlayingState();
		logText.text = textData;
		//UpdateUIWithCurrentData();
	}


	public void UpdateUIWithCurrentData() {
		logText.text = textData;	
	}

	void Update() {
		if(trainer != null && currentPlayer != null && trainer.IsPlaying) {	//  && trainer.IsPlaying
			//Debug.Log ("GOOD TO GO!");
			//logText.text = "Input Nodes: " + currentPlayer.MasterPopulation.numInputNodes + "\nOutput Nodes: " + currentPlayer.MasterPopulation.numOutputNodes + "\n" + trainer.GetCurrentGamePlayingState();
			/*string currentAgentsString = "";
			for(int i = 0; i < currentPlayer.masterPopulation.populationMaxSize; i++) {
				string genomeWeightsString = "";
				for(int j = 0; j < 4; j++) {
					genomeWeightsString += currentPlayer.masterPopulation.masterAgentArray[i].genome.genomeWeights[j].ToString() + ", ";
				}
				currentAgentsString += "Agent " + i.ToString () + ": FitnessScore: " + currentPlayer.masterPopulation.masterAgentArray[i].fitnessScore + " Weights: " + genomeWeightsString + "\n";

			}*/

			logText.text = Time.timeScale.ToString() + "\n" + trainer.GetCurrentGamePlayingState(); // + "\n" + currentAgentsString;
		}
	}
}
