using UnityEngine;
using System.Collections;

public class Population {
	public bool debugFunctionCalls = false; // turns debug messages on/off

	public enum BrainType {
		None,
		Test,
		ANN_FF_Layered_AllToAll
	};
	// Brain Settings!
	public BrainType brainType = BrainType.None;
	public BrainBase templateBrain; // do I need this? would be used as currently selected brain for use in displaying/choosing brain settings

	public int numInputNodes = 0;
	public int numOutputNodes = 0;

	public int populationMaxSize = 4; // default value
	public int numAgents = 0;

	public Agent[] masterAgentArray;

	public bool isFunctional = false;

	// Constructor Methods:
	public Population() {
		DebugBot.DebugFunctionCall("Population; Population() Constructor!; ", debugFunctionCalls);
		ChangeTemplateBrainType(brainType);
	}

	// Public Methods!
	public void InitializeMasterAgentArray() {  // Creates a new population for the FIRST TIME!!!
		DebugBot.DebugFunctionCall("Population; InitializeMasterAgentArray(); ", debugFunctionCalls);

		// BrainType can't be set to NONE !!!!!!!!!

		masterAgentArray = new Agent[populationMaxSize];
		for(int i = 0; i < populationMaxSize; i++) {
			Agent newAgent = new Agent();
			InitializeAgentBrain(newAgent);  // create Agent's brain as proper type, and copies over templateBrain's settings
			masterAgentArray[i] = newAgent;
			numAgents++;
		}

		// THIS MEANS POPULATION NEEDS TO BE VALID FOR TESTING:
	}

	public void TempResizeMasterAgentArray() {
		Agent[] newMasterAgentArray = new Agent[populationMaxSize];
		for(int i = 0; i < populationMaxSize; i++) {
			if(i < masterAgentArray.Length) {
				newMasterAgentArray[i] = masterAgentArray[i];
			}
			else {// No agent at this index
				Agent newAgent = new Agent();
				InitializeAgentBrain(newAgent);  // create Agent's brain as proper type, and copies over templateBrain's settings
				newMasterAgentArray[i] = newAgent;
			}
		}
		numAgents = populationMaxSize;
		masterAgentArray = null;
		masterAgentArray = newMasterAgentArray;
	}

	public void ChangeTemplateBrainType(BrainType newBrainType) {
		if(brainType != newBrainType) { // only do something if the brainType has changed
			brainType = newBrainType;
			templateBrain = null;
			templateBrain = new BrainBase();
			// Test Type (manual coded brain)
			if(brainType == BrainType.Test) {
				templateBrain = new BrainTest();
			}
			// ANN Feed-Forward Layered Static All-to-All:
			if(brainType == BrainType.ANN_FF_Layered_AllToAll) {
				templateBrain = new BrainANN_FF_Layers_A2A();
			}
		}
	}

	private void InitializeAgentBrain(Agent newAgent) {  /// Configure newly-created Agent for the FIRST TIME!! to change settings on an existing agent, use a different method.
		DebugBot.DebugFunctionCall("Population; InitializeAgentInstance(); ", debugFunctionCalls);
		isFunctional = true;
		// Initialize Brain:
		if(brainType == BrainType.None) {
			newAgent.brain = new BrainBase();
			isFunctional = false;
		}
		// Test Type (manual coded brain)
		if(brainType == BrainType.Test) {
			newAgent.brain = new BrainTest();

		}
		// ANN Feed-Forward Layered Static All-to-All:
		if(brainType == BrainType.ANN_FF_Layered_AllToAll) {
			newAgent.brain = new BrainANN_FF_Layers_A2A() as BrainANN_FF_Layers_A2A;
			int[] layerDimensions = new int[2]{numInputNodes, numOutputNodes}; // Add UI support for setting layers

			Genome genome = newAgent.brain.InitializeRandomBrain(layerDimensions); // 'builds' the brain and spits out a Genome
			newAgent.genome = genome;
			newAgent.brain.SetBrainFromGenome(genome);

		}

		newAgent.brain.CopyBrainSettingsFrom(templateBrain);  // Copies settings from template brain (what has been set from UI) to new brain instance using override method
	}

	public void SetMaxPopulationSize(int size) {
		DebugBot.DebugFunctionCall("Population; SetMaxPopulationSize(); ", debugFunctionCalls);
		populationMaxSize = size;
		// Come in here and add restrictions later for shrinking pop size safely
	}

	public void RankAgentArray() { // brute force sort

		/*scoreArray = new float[numAgents];
		scoreHunterArray = new float[numAgents];
		geneRankArray = new Genome[numAgents];
		geneRankHunterArray = new Genome[numAgents];
		float swapTimeA = new float();
		float swapTimeB = new float();
		Genome swapGeneA = new Genome(layerSizes);
		Genome swapGeneB = new Genome(layerSizes);
		
		scoreArray = avoidanceTimes;
		scoreHunterArray = minHunterDistances;
		geneRankArray = genePool;
		geneRankHunterArray = genePoolHunter;
		*/
		Agent[] rankedAgentArray = new Agent[populationMaxSize];
		rankedAgentArray = masterAgentArray;
		//Debug.Log("agent0 fitness: " + rankedAgentArray[0].fitnessScore.ToString());
		Agent swapAgentA = new Agent();
		Agent swapAgentB = new Agent();
		Genome swapGenomeA = new Genome();
		Genome swapGenomeB = new Genome();
		float[] swapBiasesA = new float[rankedAgentArray[0].genome.genomeBiases.Length];
		float[] swapBiasesB = new float[rankedAgentArray[0].genome.genomeBiases.Length];

		/*
		string masterString = "MasterAgentArrayB4: ";
		for(int g = 0; g < populationMaxSize; g++) {
			masterString += "Fit: " + masterAgentArray[g].fitnessScore.ToString () + ", " +  masterAgentArray[g].brain.genome.genomeWeights[0].ToString() + ", ";
		}
		Debug.Log (masterString);
		string rankedString = "RankedAgentArrayB4: ";
		for(int h = 0; h < populationMaxSize; h++) {
			rankedString += "Fit: " + rankedAgentArray[h].fitnessScore.ToString () + ", " + rankedAgentArray[h].brain.genome.genomeWeights[0].ToString() + ", ";
		}
		Debug.Log (rankedString);
		*/


		for(int i = 0; i < populationMaxSize - 1; i++)
		{
			for(int j = 0; j < populationMaxSize - 1; j++)
			{
				if(rankedAgentArray[j].fitnessScore < rankedAgentArray[j+1].fitnessScore)  // if lower index holds a smaller time, swap places
				{
					// NOT WORKING!!!!!!
					//swapGenomeA = rankedAgentArray[j].brain.genome;
					//swapGenomeB = rankedAgentArray[j+1].brain.genome;
					swapBiasesA = rankedAgentArray[j].genome.genomeBiases;
					swapBiasesB = rankedAgentArray[j+1].genome.genomeBiases;
					swapAgentA = rankedAgentArray[j];
					swapAgentB = rankedAgentArray[j+1];

					//rankedAgentArray[j].brain.genome = swapGenomeB;
					//rankedAgentArray[j+1].brain.genome = swapGenomeA;
					rankedAgentArray[j].genome.genomeBiases = swapBiasesA;
					rankedAgentArray[j+1].genome.genomeBiases = swapBiasesB;
					rankedAgentArray[j] = swapAgentB;
					rankedAgentArray[j+1] = swapAgentA;
				}
			}
		}	
		/*
		string rankedAfterString = "RankedAgentArrayAfter: ";
		for(int h = 0; h < populationMaxSize; h++) {
			rankedAfterString += "Fit: " + rankedAgentArray[h].fitnessScore.ToString () + ", " + rankedAgentArray[h].brain.genome.genomeWeights[0].ToString() + ", ";
		}
		Debug.Log (rankedAfterString);
		*/
		masterAgentArray = rankedAgentArray;
	}

	public Population RefPopulation() {  // returns a copy (same ref memory) of the current Population instance calling the function i.e. Population newPop = new Population(); newPop = sourcePop.CopyPopulation();
		Population populationRef = new Population();
		populationRef.PrintSettings();
		populationRef = this;
		return populationRef;
	}

	public Population CopyPopulationSettings() {  // returns a copy of the current Population instance calling the function i.e. Population newPop = new Population(); newPop = sourcePop.CopyPopulation();
		Population populationCopy = new Population();
		populationCopy.SetMaxPopulationSize(populationMaxSize);
		populationCopy.brainType = brainType;
		populationCopy.templateBrain = templateBrain;
		populationCopy.numInputNodes = numInputNodes;
		populationCopy.numOutputNodes = numOutputNodes;
		populationCopy.numAgents = numAgents;
		populationCopy.InitializeMasterAgentArray();
		//populationCopy = this;
		return populationCopy;
	}

	public Population[] SplitPopulation(int numFactions) {
		if(numFactions > masterAgentArray.Length) { // safety
			numFactions = masterAgentArray.Length;
		}
		Population[] populationArray = new Population[numFactions];
		float factionSize = (float)masterAgentArray.Length / (float)numFactions;
		float incrementSize = factionSize;  // start at ideal length
		float currentSplitPosition = 0f;
		int curAgentIndex = 0;
		for(int i = 0; i < numFactions; i++) {   // 4/3:
			currentSplitPosition += incrementSize;  // 1.333, 3.0, 3.67
			int blockSize = Mathf.RoundToInt(incrementSize);  // 1, 2, 1
			incrementSize += (factionSize - (float)blockSize);  // 1.67, -0.67,
			//Debug.Log ("Faction #" + (i+1).ToString () + ": " + blockSize.ToString() + " Agents, IncrementSize= " + incrementSize.ToString() + ", CurPos: " + currentSplitPosition.ToString());

			// Create new Population:
			Population factionPop = new Population();
			factionPop.populationMaxSize = blockSize;
			factionPop.brainType = brainType;
			factionPop.templateBrain = templateBrain;
			factionPop.numInputNodes = numInputNodes;
			factionPop.numOutputNodes = numOutputNodes;
			factionPop.InitializeMasterAgentArray();

			for(int j = 0; j < blockSize; j++) {  // copy over Agents from source Pop.
				factionPop.masterAgentArray[j] = masterAgentArray[curAgentIndex];
				curAgentIndex++;
			}

			populationArray[i] = factionPop;
		}

		return populationArray;
	}

	public static Population AddPopulations(Population[] sourcePopulations) {
		// Find target population size
		return sourcePopulations[0];
	}

	public void SetToCombinedPopulations(Population[] sourcePopulation) {
		int newSize = 0;
		for(int i = 0; i < sourcePopulation.Length; i++) {
			newSize += sourcePopulation[i].masterAgentArray.Length;
		}
		this.ResizePopulation(newSize);
		int masterIndex = 0;
		for(int j = 0; j < sourcePopulation.Length; j++) {
			for(int k = 0; k < sourcePopulation[j].masterAgentArray.Length; k++) {
				masterAgentArray[masterIndex] = sourcePopulation[j].masterAgentArray[k];
				masterIndex++;
			}
		}
	}

	public void ResizePopulation(int newSize) {
		this.populationMaxSize = newSize;
		Agent[] newMasterAgentArray = new Agent[populationMaxSize];
		for(int i = 0; i < populationMaxSize; i++) {
			if(i < masterAgentArray.Length) {
				newMasterAgentArray[i] = masterAgentArray[i];
			}
			else {// No agent at this index
				Agent newAgent = new Agent();
				InitializeAgentBrain(newAgent);  // create Agent's brain as proper type, and copies over templateBrain's settings
				newMasterAgentArray[i] = newAgent;
			}
		}
		numAgents = populationMaxSize;
		masterAgentArray = null;
		masterAgentArray = newMasterAgentArray;
	}

	public void PrintSettings() {
		string currentSettingsLog = "";
		currentSettingsLog += "brainType: " + brainType.ToString();
		if(templateBrain != null) {
			currentSettingsLog += ", templateBrain: " + templateBrain.ToString();
		} else {
			currentSettingsLog += ", templateBrain: null";
		}
		currentSettingsLog += ", Input/Output: " + numInputNodes.ToString() + " / " + numOutputNodes.ToString();
		currentSettingsLog += ", popMaxSize: " + populationMaxSize.ToString();
		currentSettingsLog += ", numAgents: " + numAgents.ToString();
		currentSettingsLog += ", isFunc: " + isFunctional.ToString();
		Debug.Log (currentSettingsLog);

		string agentListLog = "AgentArray! ";
		if(masterAgentArray != null) {
			agentListLog += "Length: " + masterAgentArray.Length;

			for(int i = 0; i < masterAgentArray.Length; i++) {
				agentListLog += ", A:" + i.ToString() + " Fit: " + masterAgentArray[i].fitnessScore;
				if(masterAgentArray[i].brain != null) {
					agentListLog += ", brain: " + masterAgentArray[i].brain.Name;
				} else {
					agentListLog += ", brain: null";
				}
				if(masterAgentArray[i].genome != null) {
					agentListLog += ", genome: " + masterAgentArray[i].genome.genomeWeights[0].ToString();
				} else {
					agentListLog += ", genome: null";
				}
			}

		} else {
			agentListLog += "null";
		}
		Debug.Log (agentListLog);


		//public BrainBase templateBrain; // do I need this? would be used as currently selected brain for use in displaying/choosing brain settings		
		//public Agent[] masterAgentArray;
	}

}
