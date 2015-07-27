using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerDataVisUI : MonoBehaviour {

	public bool debugFunctionCalls = false;

	public enum FitnessGraphType {
		FitnessBasic,
		FitnessAgentsLastGen,
		FitnessComponents
	};

	public enum HistoryGraphType {
		HistoryAvgGenomes,
		HistoryAgentGenomes
	};

	public enum CurAgentGraphType {
		CurAgentBrainDiagram
	};

	public TrainerModuleUI trainerModuleScript;

	// FITNESS GRAPH PANEL:
	public FitnessGraphType fitnessGraphMode = FitnessGraphType.FitnessBasic;
	public GameObject panelFitnessGraph;
	public Image imageFitnessGraph;
	public Text textFitnessGraphTitle;
	public Button buttonFitnessZoomInX;
	public Button buttonFitnessZoomOutX;
	public Button buttonFitnessZoomInY;
	public Button buttonFitnessZoomOutY;
	public Button buttonFitnessGraphMode;
	public Text textFitnessReadout;
	public Text textButtonFitnessGraphMode;
	public Material matFitnessBasic;
	public Material matFitnessAgentsLastGen;
	public Material matFitnessComponents;
	private int curFitnessGraphModeIndex = 0;
	private int numFitnessGraphModes;

	// HISTORY GRAPH PANEL:
	public HistoryGraphType historyGraphMode = HistoryGraphType.HistoryAvgGenomes;
	public GameObject panelHistoryGraph;
	public Image imageHistoryGraph;
	public Text textHistoryGraphTitle;
	public Button buttonHistoryGraphMode;
	public Material matHistoryAvgGenomes;
	public Material matHistoryAgentGenomes;
	private int curHistoryGraphModeIndex = 0;
	private int numHistoryGraphModes;

	// AGENT GRAPH PANEL:
	public CurAgentGraphType curAgentGraphMode = CurAgentGraphType.CurAgentBrainDiagram;
	public GameObject panelCurrentAgentGraph;
	public Image imageCurAgentGraph;
	public Text textCurAgentGraphTitle;
	public Button buttonCurAgentGraphMode;
	public Material matCurAgentBrainDiagram;
	private int curCurAgentGraphModeIndex = 0;
	private int numCurAgentGraphModes;

	public Image bgImage;

	private Population populationRef;
	
	// UI Settings:	
	private bool panelActive = false;  // requires valid population

	void Awake() {
		numFitnessGraphModes = System.Enum.GetValues(typeof(FitnessGraphType)).Length;  // figure out how to count enums again
		numHistoryGraphModes = System.Enum.GetValues(typeof(HistoryGraphType)).Length;
		numCurAgentGraphModes = System.Enum.GetValues(typeof(CurAgentGraphType)).Length;
		//Debug.Log ("TDataVisUI; Awake(); FitnessGraphType length= " + numFitnessGraphModes.ToString());
	}

	//public Current Graph Mode:
	// -AllGensRawFitness
	// -AllGensModifiedFitness  <-- ^^^ combine these two?
	// -AllGensFitnessComponentStack - raw or modified/both?
	// -EachAgentLastGenScore -- same prob-raw or modified?
	// -PreviousAgentFitnessBreakdown
	//
	//public void SetFitnessPanelGraphType() {
	// This function would switch the current material type on the actual UI Image in order to change graph mode
	//}
	// 
	// Functions for Buttons to cycle through FitnessGraph Modes:
		
	public void InitializePanelWithTrainerData() {
		//DebugBot.DebugFunctionCall("TDataVisUI; InitializePanelWithTrainerData(); ", debugFunctionCalls);
		//CheckActivationCriteria();
		//if(panelActive) {

			// SET PENDING values from trainer data:
			
			//
		//}		

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		if(currentPlayer.graphKing != null) {
			//Debug.Log ("SET TEXTURE!");

			InitializeNewFitnessMaterial();
			InitializeNewHistoryMaterial();
			InitializeNewCurAgentMaterial();

			/*panelFitnessGraph.GetComponent<Image>().material = GetCurrentMaterial();
			panelFitnessGraph.GetComponent<Image>().material.SetTexture("_FitnessTex", currentPlayer.graphKing.texFitnessBasic);

			textFitnessGraphTitle.text = "Average Fitness Over Generations:";
			//Debug.Log ("generationData Count! " + currentPlayer.dataManager.generationDataList.Count.ToString());
			//if(currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-1] != null) {
			if(currentPlayer.dataManager.generationDataList.Count != 0) {
				textNumber.text = "Last Gen Avg: " + currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-1].avgAgentScoreRaw.ToString();

			}
			else {
				textNumber.text = "Last Gen Avg:";
			}*/

			//Debug.Log(panelFitnessGraph.GetComponent<Image>().material.GetTexture("_FitnessTex").width.ToString());
		}

		UpdateUIWithCurrentData();
	}
	
	public void CheckActivationCriteria() {  // checks which buttons/elements should be active/inactive based on the current data
		//DebugBot.DebugFunctionCall("TDataVisUI; CheckActivationCriteria(); ", debugFunctionCalls);
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
		//DebugBot.DebugFunctionCall("TDataVisUI; UpdateUIElementStates(); ", debugFunctionCalls);
		// Changing Button Displays !!
		if(panelActive) {
			panelFitnessGraph.SetActive (true);
			panelHistoryGraph.SetActive (true);
			panelCurrentAgentGraph.SetActive (true);
		}
		else {
			panelFitnessGraph.SetActive (false);
			panelHistoryGraph.SetActive (false);
			panelCurrentAgentGraph.SetActive (false);
		}
		bgImage.color = trainerModuleScript.defaultBGColor;
	}
	
	public void UpdateUIWithCurrentData() {
		//DebugBot.DebugFunctionCall("TDataVisUI; UpdateUIWithCurrentData(); ", debugFunctionCalls);
		CheckActivationCriteria();
		UpdateUIElementStates();
	}

	private Material GetCurrentFitnessMaterial() {
		fitnessGraphMode = (FitnessGraphType)curFitnessGraphModeIndex;
		if(fitnessGraphMode == FitnessGraphType.FitnessBasic) {
			//Debug.Log ("GetCurrentFitnessMaterial() FitnessGraphType.FitnessBasic");
			return matFitnessBasic;
		}
		else if(fitnessGraphMode == FitnessGraphType.FitnessAgentsLastGen) {
			//Debug.Log ("GetCurrentFitnessMaterial() FitnessGraphType.FitnessAgentsLastGen");
			return matFitnessAgentsLastGen;
		}
		else if(fitnessGraphMode == FitnessGraphType.FitnessComponents) {
			//Debug.Log ("GetCurrentFitnessMaterial() FitnessGraphType.FitnessComponents");
			return matFitnessComponents;
		}
		else {
			Debug.Log ("NO FITNESS GRAPH TYPE");
			return matFitnessBasic;
		}
	}

	public void InitializeNewFitnessMaterial() { // Based on curFitnessGraphModeIndex

		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];


		// SET MATERIAL on fitness graph panel image:
		imageFitnessGraph.GetComponent<Image>().material = GetCurrentFitnessMaterial();

		// FITNESS BASIC:
		if(fitnessGraphMode == FitnessGraphType.FitnessBasic) {
			imageFitnessGraph.GetComponent<Image>().material.SetTexture("_FitnessTex", currentPlayer.graphKing.texFitnessBasic);
			
			textFitnessGraphTitle.text = "Average Fitness Over Generations:";
			if(currentPlayer.dataManager.generationDataList.Count > 1) {
				textFitnessReadout.text = "Last Gen Avg: " + currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-2].avgAgentScoreRaw.ToString();
				
			}
			else {
				textFitnessReadout.text = "Last Gen Avg:";
			}
		}

		// FITNESS AGENTS LAST GEN:
		if(fitnessGraphMode == FitnessGraphType.FitnessAgentsLastGen) {
			imageFitnessGraph.GetComponent<Image>().material.SetTexture("_FitnessTex", currentPlayer.graphKing.texFitnessAgentsLastGen);

			textFitnessGraphTitle.text = "Agent Fitnesses Last Generation:";
			if(currentPlayer.dataManager.generationDataList.Count > 1) {
				textFitnessReadout.text = "Last Gen Avg: " + currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-2].avgAgentScoreRaw.ToString();
				imageFitnessGraph.GetComponent<Image>().material.SetInt("_NumAgents", currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count - 2].agentDataArray.Length);
				imageFitnessGraph.GetComponent<Image>().material.SetFloat("_GenAvgScore", currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-2].avgAgentScoreRaw);
				
			}
			else {
				textFitnessReadout.text = "Last Gen Avg:";
				imageFitnessGraph.GetComponent<Image>().material.SetInt("_NumAgents", currentPlayer.dataManager.generationDataList[0].agentDataArray.Length);
				imageFitnessGraph.GetComponent<Image>().material.SetFloat("_GenAvgScore", currentPlayer.dataManager.generationDataList[0].avgAgentScoreRaw);
			}
		}

		// FITNESS COMPONENTS:
		if(fitnessGraphMode == FitnessGraphType.FitnessComponents) {
			imageFitnessGraph.GetComponent<Image>().material.SetTexture("_FitnessTex", currentPlayer.graphKing.texFitnessComponents);

			textFitnessGraphTitle.text = "Avg Fitness By Components:";
			if(currentPlayer.dataManager.generationDataList.Count > 1) {
				textFitnessReadout.text = "Last Gen Avg: " + currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-2].avgAgentScoreRaw.ToString();
				imageFitnessGraph.GetComponent<Image>().material.SetInt("_NumComponents", currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count - 2].totalNumFitnessComponents);
				imageFitnessGraph.GetComponent<Image>().material.SetFloat("_GenAvgScore", currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-2].avgAgentScoreRaw);
				
			}
			else {
				textFitnessReadout.text = "Last Gen Avg:";
				imageFitnessGraph.GetComponent<Image>().material.SetInt("_NumComponents", currentPlayer.dataManager.generationDataList[0].totalNumFitnessComponents);
				imageFitnessGraph.GetComponent<Image>().material.SetFloat("_GenAvgScore", currentPlayer.dataManager.generationDataList[0].avgAgentScoreRaw);
			}
		}

		if(currentPlayer.dataManager.generationDataList.Count > 1) {
			//Debug.Log ("InitializeNewMaterial() curIndex: " + curFitnessGraphModeIndex.ToString() + "avgScore: " + currentPlayer.dataManager.generationDataList[currentPlayer.dataManager.generationDataList.Count-2].avgAgentScoreRaw.ToString());
	
		}
	}

	private Material GetCurrentHistoryMaterial() {
		historyGraphMode = (HistoryGraphType)curHistoryGraphModeIndex;
		if(historyGraphMode == HistoryGraphType.HistoryAvgGenomes) {
			//Debug.Log ("GetCurrentHistoryMaterial() HistoryGraphType.HistoryAvgGenomes");
			return matHistoryAvgGenomes;
		}
		else if(historyGraphMode == HistoryGraphType.HistoryAgentGenomes) {
			//Debug.Log ("GetCurrentHistoryMaterial() HistoryGraphType.HistoryAgentGenomes");
			return matHistoryAgentGenomes;
		}
		else {
			Debug.Log ("NO HISTORY GRAPH TYPE");
			return matHistoryAvgGenomes;
		}
	}
	
	public void InitializeNewHistoryMaterial() { // Based on curFitnessGraphModeIndex
		
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		

		// SET MATERIAL on history graph panel image:
		imageHistoryGraph.GetComponent<Image>().material = GetCurrentHistoryMaterial();
		
		// HISTORY AVG GENOMES:
		if(historyGraphMode == HistoryGraphType.HistoryAvgGenomes) {
			imageHistoryGraph.GetComponent<Image>().material.SetTexture("_DataTex", currentPlayer.graphKing.texHistoryAvgGenomes);
			
			textHistoryGraphTitle.text = "Average Genome Over Generations:";
		}

		// HISTORY AVG GENOMES:
		if(historyGraphMode == HistoryGraphType.HistoryAgentGenomes) {
			imageHistoryGraph.GetComponent<Image>().material.SetTexture("_DataTex", currentPlayer.graphKing.texHistoryAgentGenomes);
			
			textHistoryGraphTitle.text = "This Generation All Agent Genomes";
		}
	}

	private Material GetCurrentCurAgentMaterial() {
		curAgentGraphMode = (CurAgentGraphType)curCurAgentGraphModeIndex;
		if(curAgentGraphMode == CurAgentGraphType.CurAgentBrainDiagram) {
			//Debug.Log ("GetCurrentCurAgentMaterial() CurAgentGraphType.CurAgentBrainDiagram");
			return matCurAgentBrainDiagram;
		}
		else {
			Debug.Log ("NO CUR-AGENT GRAPH TYPE");
			return matCurAgentBrainDiagram;  // default
		}
	}

	public void InitializeNewCurAgentMaterial() { // Based on curFitnessGraphModeIndex
		
		Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		
		
		// SET MATERIAL on cur agent graph panel image:
		imageCurAgentGraph.GetComponent<Image>().material = GetCurrentCurAgentMaterial();
		
		// CURRENT AGENT BRAIN DIAGRAM:
		if(curAgentGraphMode == CurAgentGraphType.CurAgentBrainDiagram) {
			//Debug.Log ("graphType: " + CurAgentGraphType.CurAgentBrainDiagram.ToString());
			imageCurAgentGraph.GetComponent<Image>().material.SetTexture("_DataTex", currentPlayer.graphKing.texCurAgentBrainDiagramAgent);
			imageCurAgentGraph.GetComponent<Image>().material.SetTexture("_CurDataTex", currentPlayer.graphKing.texCurAgentBrainDiagramTick);
			imageCurAgentGraph.GetComponent<Image>().material.SetFloat("_TotalNodes", currentPlayer.graphKing.curAgentBrainDiagramNumNodes);
			imageCurAgentGraph.GetComponent<Image>().material.SetFloat("_TotalBiases", currentPlayer.graphKing.curAgentBrainDiagramNumBiases);
			imageCurAgentGraph.GetComponent<Image>().material.SetFloat("_TotalWeights", currentPlayer.graphKing.curAgentBrainDiagramNumWeights);
			
			textCurAgentGraphTitle.text = "Current Agent Brain Diagram:";
		}
	}

	public void ClickFitnessGraphMode() {
		curFitnessGraphModeIndex++;
		if(curFitnessGraphModeIndex >= numFitnessGraphModes) {
			curFitnessGraphModeIndex = 0;
		}
		
		// Set New Material:
		InitializeNewFitnessMaterial();
	}

	public void ClickFitnessZoomInX() {
		//Player currentPlayer = trainerModuleScript.gameController.masterTrainer.PlayerList[trainerModuleScript.gameController.masterTrainer.CurPlayer-1];
		float zoomFactorX = imageFitnessGraph.GetComponent<Image>().material.GetFloat("_ZoomFactorX") * 0.8f;
		imageFitnessGraph.GetComponent<Image>().material.SetFloat("_ZoomFactorX", zoomFactorX);
	}

	public void ClickFitnessZoomOutX() {
		float zoomFactorX = imageFitnessGraph.GetComponent<Image>().material.GetFloat("_ZoomFactorX") * 1.2f;
		if(zoomFactorX > 1f) { zoomFactorX = 1f; }
		imageFitnessGraph.GetComponent<Image>().material.SetFloat("_ZoomFactorX", zoomFactorX);
	}

	public void ClickFitnessZoomInY() {
		float zoomFactorY = imageFitnessGraph.GetComponent<Image>().material.GetFloat("_ZoomFactorY") * 0.8f;
		imageFitnessGraph.GetComponent<Image>().material.SetFloat("_ZoomFactorY", zoomFactorY);
	}

	public void ClickFitnessZoomOutY() {
		float zoomFactorY = imageFitnessGraph.GetComponent<Image>().material.GetFloat("_ZoomFactorY") * 1.2f;
		if(zoomFactorY > 1f) { zoomFactorY = 1f; }
		imageFitnessGraph.GetComponent<Image>().material.SetFloat("_ZoomFactorY", zoomFactorY);
	}

	public void ClickHistoryGraphMode() {
		curHistoryGraphModeIndex++;
		if(curHistoryGraphModeIndex >= numHistoryGraphModes) {
			curHistoryGraphModeIndex = 0;
		}
		
		// Set New Material:
		InitializeNewHistoryMaterial();
	}

	public void ClickHistoryGeneWeightRangeUp() {
		float multiplier = imageHistoryGraph.GetComponent<Image>().material.GetFloat("_ValueMultiplier") * 1.2f;
		if(multiplier > 10f) { multiplier = 10f; }
		imageHistoryGraph.GetComponent<Image>().material.SetFloat("_ValueMultiplier", multiplier);
	}

	public void ClickHistoryGeneWeightRangeDown() {
		float multiplier = imageHistoryGraph.GetComponent<Image>().material.GetFloat("_ValueMultiplier") * 0.8f;
		if(multiplier < 0.1f) { multiplier = 0.1f; }
		imageHistoryGraph.GetComponent<Image>().material.SetFloat("_ValueMultiplier", multiplier);
	}

	public void ClickCurAgentGraphMode() {
		curCurAgentGraphModeIndex++;
		if(curCurAgentGraphModeIndex >= numCurAgentGraphModes) {
			curCurAgentGraphModeIndex = 0;
		}
		
		// Set New Material:
		InitializeNewCurAgentMaterial();
	}
}
