using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrossoverManager {

	// SETTINGS:
	public string tempName = "name!";

	public float masterMutationRate = 0.025f;
	public float maximumWeightMagnitude = 2.5f;
	public float mutationDriftScale = 0.5f;
	public float mutationRemoveLinkChance = 0.1f;
	public float mutationAddLinkChance = 0f;
	public float mutationFunctionChance = 0f;

	public int numSwapPositions = 1;
	public int numFactions = 1;
	public int minNumParents = 2;
	public int maxNumParents = 2;
	public bool breedWithSimilar = false;

	public float survivalRate = 0.1f;
	public bool survivalByRank = true;
	public bool survivalStochastic = false;
	public bool survivalByRaffle = false;

	public float breedingRate = 0.5f;
	public bool breedingByRank = true;
	public bool breedingStochastic = false;
	public bool breedingByRaffle = false;



	//empty constructor
	public CrossoverManager() {

	}

	public void CopyFromSourceCrossoverManager(CrossoverManager sourceManager) {

		tempName = sourceManager.tempName;
		
		masterMutationRate = sourceManager.masterMutationRate;
		maximumWeightMagnitude = sourceManager.maximumWeightMagnitude;
		mutationDriftScale = sourceManager.mutationDriftScale;
		mutationRemoveLinkChance = sourceManager.mutationRemoveLinkChance;
		mutationAddLinkChance = sourceManager.mutationAddLinkChance;
		mutationFunctionChance = sourceManager.mutationFunctionChance;

		numSwapPositions = sourceManager.numSwapPositions;
		numFactions = sourceManager.numFactions;
		minNumParents = sourceManager.minNumParents;
		maxNumParents = sourceManager.maxNumParents;
		breedWithSimilar = sourceManager.breedWithSimilar;

		survivalRate = sourceManager.survivalRate;
		survivalByRank = sourceManager.survivalByRank;
		survivalStochastic = sourceManager.survivalStochastic;
		survivalByRaffle = sourceManager.survivalByRaffle;

		breedingRate = sourceManager.breedingRate;
		breedingByRank = sourceManager.breedingByRank;
		breedingStochastic = sourceManager.breedingStochastic;
		breedingByRaffle = sourceManager.breedingByRaffle;
	}

	public void MakeRefToSourceCrossoverManager(CrossoverManager sourceManager) {
		//this = sourceManager;
	}

	public void PerformCrossover(ref Population sourcePopulation) {
		Population newPop = sourcePopulation.CopyPopulationSettings();

		if(numFactions > 1) {

			Population[] sourceFactions = sourcePopulation.SplitPopulation(numFactions);
			Population[] newFactions = new Population[numFactions];
			for(int i = 0; i < numFactions; i++) {
				// Make a Genome array of each faction
				// Then BreedAgentPool on each Array?
				// Then Add those genomes to new Population masterAgentArray?
				//newFactions[i] = sourceFactions[i].CopyPopulationSettings();
				Debug.Log ("FactionSize: " + sourceFactions[i].populationMaxSize.ToString());
				newFactions[i] = BreedPopulation(ref sourceFactions[i]);
			}
			// Add them back together!
			newPop.SetToCombinedPopulations(newFactions);

		}
		else {
			newPop = BreedPopulation(ref sourcePopulation);
		}
		sourcePopulation = newPop;
	}

	public float[][] MixFloatChromosomes(float[][] parentFloatGenes, int numOffspring) {  // takes A number of Genomes and returns new mixed up versions
		int geneArrayLength = parentFloatGenes[0].Length;
		float[][] childFloatGenes = new float[numOffspring][];

		List<int> swapPositionsList = new List<int>();
		for(int s = 0; s < numSwapPositions; s++) {
			swapPositionsList.Add(UnityEngine.Random.Range (0,geneArrayLength));
			//Debug.Log ("swapPositionsList[" + s.ToString() + "]: " + swapPositionsList[s].ToString());
		}
		swapPositionsList.Sort(); // Ordered list of indices where the parent changes
		//swapPositionsList.Contains(2);

		if(parentFloatGenes.Length == 1) {  // SINGLE PARENT
			for(int c = 0; c < numOffspring; c++) {  // for each childAgent:
				childFloatGenes[c] = new float[geneArrayLength]; // set child geneArray to proper length
				for(int i = 0; i < geneArrayLength; i++) { // iterate through genes in geneArray
					childFloatGenes[c][i] = parentFloatGenes[0][i]; // only one parent, hence index=0
					if(CheckForFloatMutation()) {
					// MMMUUUUTTTTTAAAAATTTTIIIOOOOOOONNNNNNNN!!!!!!!!!!!!
						childFloatGenes[c][i] = MutateFloat(childFloatGenes[c][i]);
					}
				}
			}
		}
		//int[] swapPositions = new int[numSwapPositions];
		//numSwapPositions
		if(parentFloatGenes.Length == 2) {  // TWO PARENTS
			int currentParentIndex = UnityEngine.Random.Range (0,2); // 2 parents
			for(int c = 0; c < numOffspring; c++) {  // for each childAgent:
				childFloatGenes[c] = new float[geneArrayLength];
				//currentParentIndex = 1 - currentParentIndex; // 2 parents, swaps at beginning of geneArray
				//Debug.Log ("currentParentIndex: " + currentParentIndex.ToString());
				for(int i = 0; i < geneArrayLength; i++) { // iterate through genes in geneArray
					// Do Crossover of Array HERE: !!!!!
					if(swapPositionsList.Contains(i)) { // if current array index is a swap position
						currentParentIndex = 1 - currentParentIndex; // 2 parents, so just swaps
						//Debug.Log ("SWAP: currentParentIndex: " + currentParentIndex.ToString() + ", biasIndex: " + i.ToString());
					}
					childFloatGenes[c][i] = parentFloatGenes[currentParentIndex][i]; // 
					if(CheckForFloatMutation()) {
						// MMMUUUUTTTTTAAAAATTTTIIIOOOOOOONNNNNNNN!!!!!!!!!!!!
						childFloatGenes[c][i] = MutateFloat(childFloatGenes[c][i]);
					}
				}
			}
			//childFloatGenes[0] = new float[geneArrayLength];  // parentA
			//childFloatGenes[1] = new float[geneArrayLength]; // parentB
		}
		if(parentFloatGenes.Length > 2) {  // THREE OR MORE PARENTS
			int currentParentIndex = UnityEngine.Random.Range (0,parentFloatGenes.Length);
			for(int c = 0; c < numOffspring; c++) {  // for each childAgent:
				childFloatGenes[c] = new float[geneArrayLength];
				//currentParentIndex = 1 - currentParentIndex; // 2 parents, swaps at beginning of geneArray
				//Debug.Log ("currentParentIndex: " + currentParentIndex.ToString());
				for(int i = 0; i < geneArrayLength; i++) { // iterate through genes in geneArray
					// Do Crossover of Array HERE: !!!!!
					if(swapPositionsList.Contains(i)) { // if current array index is a swap position
						currentParentIndex = UnityEngine.Random.Range (0,parentFloatGenes.Length); // 3 parents, so picks next Parent Randomly
						//Debug.Log ("SWAP: currentParentIndex: " + currentParentIndex.ToString() + ", biasIndex: " + i.ToString());
					}
					childFloatGenes[c][i] = parentFloatGenes[currentParentIndex][i]; // 
					if(CheckForFloatMutation()) {
						// MMMUUUUTTTTTAAAAATTTTIIIOOOOOOONNNNNNNN!!!!!!!!!!!!
						childFloatGenes[c][i] = MutateFloat(childFloatGenes[c][i]);
					}
				}
			}
		}

		//if(numSwapPositions > Array.Length) { // max number of swaps is every gene
		//	numSwapPositions = Array.Length;
		//}

		return childFloatGenes;
	}

	public bool CheckForFloatMutation() {
		float rand = UnityEngine.Random.Range(0f, 1f);
		if(rand < masterMutationRate) {
			return true;
		}
		return false;
	}

	public float MutateFloat(float sourceFloat) {
		float newFloat;
		float rand = UnityEngine.Random.Range(0f, 1f);
		if(rand < mutationRemoveLinkChance) {
			newFloat = 0f;
		}
		else {
			newFloat = (sourceFloat * (1.0f - mutationDriftScale)) + 
				(Gaussian.GetRandomGaussian()*maximumWeightMagnitude) * mutationDriftScale;
		}
		//tempBias1[b] = (genePool[i].genomeBiases[b] * (1.0 - mutationDriftScale)) + (MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyBias) * mutationDriftScale);
		//double zeroRoll = rand.NextDouble();
		//if(zeroRoll < mutationZeroBias) {
		//	tempBias1[b] = 0;
		//}
		return newFloat;
	}

	public void BreedAgentPool(ref Genome[] genomePool) {

	}

	public Population BreedPopulation(ref Population sourcePopulation) {
		for(int m = 0; m < sourcePopulation.masterAgentArray.Length; m++) {
			//sourcePopulation.masterAgentArray[m].brain.genome.PrintBiases("sourcePop " + sourcePopulation.masterAgentArray[m].fitnessScore.ToString() + ", " + m.ToString() + ", ");
			//newPop.masterAgentArray[m].brain.genome.PrintBiases("newPop " + m.ToString() + ", ");
		}
		// rank sourcePop by fitness score // maybe do this as a method of Population class?
		sourcePopulation.RankAgentArray();

		Population newPopulation = new Population();
		newPopulation = sourcePopulation.CopyPopulationSettings();

		// Calculate total fitness score:
		float totalScore = 0f;
		if(survivalByRaffle) {
			for(int a = 0; a < sourcePopulation.populationMaxSize; a++) { // iterate through all agents
				totalScore += sourcePopulation.masterAgentArray[a].fitnessScore;
			}			
		}

		// Create the Population that will hold the next Generation agentArray:
		Population newPop = sourcePopulation.CopyPopulationSettings();

		// Figure out How many Agents survive
		int numSurvivors = Mathf.RoundToInt(survivalRate * (float)newPop.populationMaxSize);

		//Depending on method, one at a time, select an Agent to survive until the max Number is reached
		int newChildIndex = 0;


		// For ( num Agents ) {
		for(int i = 0; i < numSurvivors; i++) {
			// If survival is by fitness score ranking:
			if(survivalByRank) {
				// Pop should already be ranked, so just traverse from top (best) to bottom (worst)
				newPopulation.masterAgentArray[newChildIndex] = sourcePopulation.masterAgentArray[newChildIndex];
				newChildIndex++;
			}
			// if survival is completely random, as a control:
			if(survivalStochastic) {
				int randomAgent = UnityEngine.Random.Range (0, numSurvivors-1);
				// Set next newChild slot to a completely randomly-chosen agent
				newPopulation.masterAgentArray[newChildIndex] = sourcePopulation.masterAgentArray[randomAgent];
				newChildIndex++;
			}
			// if survival is based on a fitness lottery:
			if(survivalByRaffle) {  // Try when Fitness is normalized from 0-1
				float randomSlicePosition = UnityEngine.Random.Range(0f, totalScore);
				float accumulatedFitness = 0f;
				for(int a = 0; a < sourcePopulation.populationMaxSize; a++) { // iterate through all agents
					accumulatedFitness += sourcePopulation.masterAgentArray[a].fitnessScore;
					// if accum fitness is on slicePosition, copy this Agent
					Debug.Log ("NumSurvivors: " + numSurvivors.ToString() + ", Surviving Agent " + a.ToString() + ": AccumFitness: " + accumulatedFitness.ToString() + ", RafflePos: " + randomSlicePosition.ToString() + ", TotalScore: " + totalScore.ToString() + ", newChildIndex: " + newChildIndex.ToString());
					if(accumulatedFitness >= randomSlicePosition) {
						newPopulation.masterAgentArray[newChildIndex] = sourcePopulation.masterAgentArray[a];
						newChildIndex++;
					}

				}
			}
		//		set newPop Agent to lucky sourcePop index
		//////////	Agent survivingAgent = sourcePopulation.Select
		// Fill up newPop agentArray with the surviving Agents
		// Keep track of Index, as that will be needed for new agents
		}

		// Figure out how many new agents must be created to fill up the new population:
		int numNewChildAgents = newPopulation.populationMaxSize - numSurvivors;
		int numEligibleBreederAgents = Mathf.RoundToInt(breedingRate * (float)newPop.populationMaxSize);
		int currentRankIndex = 0;

		float totalScoreBreeders = 0f;
		if(breedingByRaffle) {
			for(int a = 0; a < numEligibleBreederAgents; a++) { // iterate through all agents
				totalScoreBreeders += sourcePopulation.masterAgentArray[a].fitnessScore;
			}			
		}
		//float[][] parentAgentChromosomes = new float[][];
		// Iterate over numAgentsToCreate :
		// Change to While loop?
		int newChildrenCreated = 0;
		while(newChildrenCreated < numNewChildAgents) {
		//		Find how many parents random number btw min/max
			int numParentAgents = UnityEngine.Random.Range (minNumParents, maxNumParents);
			int numChildAgents = 1;
			if(numNewChildAgents - newChildrenCreated >= 2) {  // room for two more!
				numChildAgents = 2;
				//Debug.Log ("numNewChildAgents: " + numNewChildAgents.ToString() + " - newChildrenCreated: " + newChildrenCreated.ToString() + " = numChildAgents: " + numChildAgents.ToString());
			}
			float[][] parentAgentBiases = new float[numParentAgents][];
			float[][] parentAgentWeights = new float[numParentAgents][];
			for(int p = 0; p < numParentAgents; p++) {
		//		Iterate over numberOfParents :
		//			Depending on method, select suitable agents' genome.Arrays until the numberOfPArents is reached, collect them in an array of arrays
				// If breeding is by fitness score ranking:
				if(breedingByRank) {
					// Pop should already be ranked, so just traverse from top (best) to bottom (worst) to select parentAgents
					if(currentRankIndex >= numEligibleBreederAgents) { // if current rank index is greater than the num of eligible breeders, then restart the index to 0;
						currentRankIndex = 0;
					}
					//parentAgentChromosomes[p] = new float[sourcePopulation.masterAgentArray[currentRankIndex].genome.genomeBiases.Length];
					parentAgentBiases[p] = sourcePopulation.masterAgentArray[currentRankIndex].genome.genomeBiases;
					parentAgentWeights[p] = sourcePopulation.masterAgentArray[currentRankIndex].genome.genomeWeights;
					currentRankIndex++;
				}
				// if survival is completely random, as a control:
				if(breedingStochastic) {
					int randomAgent = UnityEngine.Random.Range (0, numEligibleBreederAgents-1); // check if minus 1 is needed
					// Set next newChild slot to a completely randomly-chosen agent
					parentAgentBiases[p] = sourcePopulation.masterAgentArray[randomAgent].genome.genomeBiases;
					parentAgentWeights[p] = sourcePopulation.masterAgentArray[randomAgent].genome.genomeWeights;
				}
				// if survival is based on a fitness lottery:
				if(breedingByRaffle) {
					float randomSlicePosition = UnityEngine.Random.Range(0f, totalScoreBreeders);
					float accumulatedFitness = 0f;
					for(int a = 0; a < numEligibleBreederAgents; a++) { // iterate through all agents
						accumulatedFitness += sourcePopulation.masterAgentArray[a].fitnessScore;
						// if accum fitness is on slicePosition, copy this Agent
						Debug.Log ("Breeding Agent " + a.ToString() + ": AccumFitness: " + accumulatedFitness.ToString() + ", RafflePos: " + randomSlicePosition.ToString() + ", totalScoreBreeders: " + totalScoreBreeders.ToString() + ", numEligibleBreederAgents: " + numEligibleBreederAgents.ToString());
						if(accumulatedFitness >= randomSlicePosition) {
							parentAgentBiases[p] = sourcePopulation.masterAgentArray[a].genome.genomeBiases;
							parentAgentWeights[p] = sourcePopulation.masterAgentArray[a].genome.genomeWeights;
						}
					}
				}
			}
			// Combine the genes in the parentArrays and return the specified number of children genomes
		//		Pass that array of parentAgent genome.Arrays into the float-based MixFloatChromosomes() function,
			float[][] childAgentBiases = MixFloatChromosomes(parentAgentBiases, numChildAgents);
			float[][] childAgentWeights = MixFloatChromosomes(parentAgentWeights, numChildAgents);

		//		It can return an Array of Arrays (of new childAgent genome.Arrays) 
		//		Iterate over ChildArray.Length :  // how many newAgents created
			for(int c = 0; c < numChildAgents; c++) { // for number of child Agents in floatArray[][]:
				for(int b = 0; b < sourcePopulation.masterAgentArray[0].genome.genomeBiases.Length; b++) {
					//Debug.Log ("ChildNumber: " + c.ToString() + ", BiasIndex: " + b.ToString() + ", biasValue: " + childAgentBiases[c][b].ToString () + ", newChildIndex: " + newChildIndex.ToString() + ", numNewChildren: " + numNewChildAgents.ToString() + ", numChildAgents: " + numChildAgents.ToString() + ", newChildrenCreated: " + newChildrenCreated.ToString());
					newPopulation.masterAgentArray[newChildIndex].genome.genomeBiases[b] = childAgentBiases[c][b];
					// weights and functions and more!
				}
				for(int w = 0; w < sourcePopulation.masterAgentArray[0].genome.genomeWeights.Length; w++) {
					//Debug.Log ("ChildNumber: " + c.ToString() + ", BiasIndex: " + b.ToString() + ", biasValue: " + childAgentBiases[c][b].ToString () + ", newChildIndex: " + newChildIndex.ToString() + ", numNewChildren: " + numNewChildAgents.ToString() + ", numChildAgents: " + numChildAgents.ToString() + ", newChildrenCreated: " + newChildrenCreated.ToString());
					newPopulation.masterAgentArray[newChildIndex].genome.genomeWeights[w] = childAgentWeights[c][w];
					// weights and functions and more!
				}
				newPopulation.masterAgentArray[newChildIndex].brain.SetBrainFromGenome(newPopulation.masterAgentArray[newChildIndex].genome);
				newChildIndex++;  // new child created!
				newChildrenCreated++;
			}
			
		}
		//newPop.isFunctional = true;
		return newPopulation;
	}



	/*
	public void PerformCrossover(ref Population sourcePopulation) {
		for(int m = 0; m < sourcePopulation.masterAgentArray.Length; m++) {
			//sourcePopulation.masterAgentArray[m].brain.genome.PrintBiases("sourcePop " + sourcePopulation.masterAgentArray[m].fitnessScore.ToString() + ", " + m.ToString() + ", ");
			//newPop.masterAgentArray[m].brain.genome.PrintBiases("newPop " + m.ToString() + ", ");
		}
		// rank sourcePop by fitness score // maybe do this as a method of Population class?
		// So for now, assume it is already ranked
		sourcePopulation.RankAgentArray();

		// create pending newPopulation = sourcePop
		Population newPop = new Population();
		newPop.SetMaxPopulationSize(sourcePopulation.populationMaxSize);
		newPop.brainType = sourcePopulation.brainType;
		newPop.templateBrain = sourcePopulation.templateBrain;
		newPop.numInputNodes = sourcePopulation.numInputNodes;
		newPop.numOutputNodes = sourcePopulation.numOutputNodes;
		newPop.numAgents = sourcePopulation.numAgents;
		newPop.InitializeMasterAgentArray();  // TEMP
		// set newPop maxSize (and a few other settings maybe) equal to sourcePop settings;

		// Depending on Crossover Method, mix and match Agents until the newPop is full of new Agents
		int spliceBias;
		int spliceWeight;
		int biasLength = sourcePopulation.masterAgentArray[0].genome.genomeBiases.Length;
		int weightLength = sourcePopulation.masterAgentArray[0].genome.genomeWeights.Length;
		int numMutations = 0;

		// OLD CROSSOVER CODE::::::::::
		for(int i = 0; i < (sourcePopulation.populationMaxSize / 2); i ++)
		{
			spliceBias = UnityEngine.Random.Range(0, biasLength);
			spliceWeight = UnityEngine.Random.Range(0, weightLength);
			//Debug.Log ("splice location: " + spliceBias + ", " + spliceWeight);
			
			float[] tempBias1 = new float[biasLength];
			float[] tempBias2 = new float[biasLength];
			TransferFunctions.TransferFunction[] tempFunctions1 = new TransferFunctions.TransferFunction[biasLength];
			TransferFunctions.TransferFunction[] tempFunctions2 = new TransferFunctions.TransferFunction[biasLength];
			float[] tempWeight1 = new float[weightLength];
			float[] tempWeight2 = new float[weightLength];


			string biasString1 = "WeightsString1: ";
			for(int g = 0; g < weightLength; g++) {
				biasString1 += sourcePopulation.masterAgentArray[i].brain.genome.genomeWeights[g] + ", ";
			}
			Debug.Log (biasString1);
			string biasString2 = "WeightsString2: ";
			for(int g = 0; g < weightLength; g++) {
				biasString2 += sourcePopulation.masterAgentArray[i+1].brain.genome.genomeWeights[g] + ", ";
			}
			Debug.Log (biasString2);

	
	// BIAS CROSSOVER
	for( int b = 0; b < biasLength; b++) {
		if( b < spliceBias)	{
			float mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempBias1[b] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempBias1[b] = sourcePopulation.masterAgentArray[i].genome.genomeBiases[b];
			}
			mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempBias2[b] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempBias2[b] = sourcePopulation.masterAgentArray[i+1].genome.genomeBiases[b];
			}
		}
		else {
			float mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempBias1[b] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempBias1[b] = sourcePopulation.masterAgentArray[i+1].genome.genomeBiases[b];
			}
			mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempBias2[b] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempBias2[b] = sourcePopulation.masterAgentArray[i].genome.genomeBiases[b];
			}
		}
	}
	// WEIGHT CROSSOVER
	for( int w = 0; w < weightLength; w++) {
		if( w < spliceWeight) {
			// Offspring A:
			float mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempWeight1[w] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempWeight1[w] = sourcePopulation.masterAgentArray[i].genome.genomeWeights[w];
			}
			// Offspring B:
			mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempWeight2[w] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempWeight2[w] = sourcePopulation.masterAgentArray[i+1].genome.genomeWeights[w];
			}
		}
		else {
			// Offspring A:
			float mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempWeight1[w] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else {
				tempWeight1[w] = sourcePopulation.masterAgentArray[i+1].genome.genomeWeights[w];
			}
			// Offspring B:
			mutateRoll = UnityEngine.Random.Range(0f, 1f);
			if(mutateRoll < mutationRate) {
				tempWeight2[w] = Gaussian.GetRandomGaussian();
				numMutations++;
			}
			else
			{
				tempWeight2[w] = sourcePopulation.masterAgentArray[i].genome.genomeWeights[w];
			}
		}
	}			
	//Debug.Log ("numMutations: " + numMutations);			
	//Add to new Genome Array:			
	newPop.masterAgentArray[2*i].genome.genomeBiases = tempBias1;
	//newPop.masterAgentArray[2*i].brain.genome.geneFunctions = tempFunctions1;
	newPop.masterAgentArray[2*i].genome.genomeWeights = tempWeight1;
	newPop.masterAgentArray[2*i].brain.SetBrainFromGenome(newPop.masterAgentArray[2*i].genome);
	//newPop.masterAgentArray[2*i].brain.genome.PrintBiases();
	
	newPop.masterAgentArray[2*i+1].genome.genomeBiases = tempBias2;
	//newPop.masterAgentArray[2*i+1].brain.genome.geneFunctions = tempFunctions2;
	newPop.masterAgentArray[2*i+1].genome.genomeWeights = tempWeight2;
	newPop.masterAgentArray[2*i+1].brain.SetBrainFromGenome(newPop.masterAgentArray[2*i+1].genome);
	//newPop.masterAgentArray[2*i+1].brain.genome.PrintBiases();
	

			string newBiasString1 = "newWeightsString1: ";
			for(int g = 0; g < weightLength; g++) {
				newBiasString1 += newPop.masterAgentArray[2*i].brain.genome.genomeWeights[g] + ", ";
			}
			Debug.Log (newBiasString1);
			string newBiasString2 = "newWeightsString2: ";
			for(int g = 0; g < weightLength; g++) {
				newBiasString2 += newPop.masterAgentArray[2*i+1].brain.genome.genomeWeights[g] + ", ";
			}
			Debug.Log (newBiasString2);

}

//Debug.Log ("NumMutations: " + numMutations.ToString ());

newPop.isFunctional = true;
sourcePopulation = newPop;
// Back the other way!!
//sourcePopulation.masterAgentArray = newPop.masterAgentArray;
//for(int x = 0; x < sourcePopulation.masterAgentArray.Length; x++) {
//
//}

		string rankedAfterString = "RankedAgentArrayAfter: ";
		for(int h = 0; h < sourcePopulation.masterAgentArray.Length; h++) {
			rankedAfterString += "Fit: " + sourcePopulation.masterAgentArray[h].fitnessScore.ToString () + ", " + sourcePopulation.masterAgentArray[h].brain.genome.genomeWeights[0].ToString() + ", ";
		}
		Debug.Log (rankedAfterString);

}
*/

	/*void Crossover()
	{
		int spliceBias;
		int spliceWeight;
		int biasLength = genePool [0].genomeBiases.Length;
		int weightLength = genePool [0].genomeWeights.Length;
		int numMutations = 0;
		
		
		
		Genome[] newPop = new Genome[numAgents];
		
		for(int i = 0; i < (numAgents / 2); i ++)
		{
			spliceBias = rand.Next(0, biasLength);
			spliceWeight = rand.Next(0, weightLength);
			//Debug.Log ("splice location: " + spliceBias + ", " + spliceWeight);
			
			double[] tempBias1 = new double[biasLength];
			double[] tempBias2 = new double[biasLength];
			double[] tempWeight1 = new double[weightLength];
			double[] tempWeight2 = new double[weightLength];
			
			// BIAS CROSSOVER
			for( int b = 0; b < biasLength; b++)
			{
				if( b < spliceBias)
				{
					double mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						tempBias1[b] = (genePool[i].genomeBiases[b] * (1.0 - mutationDriftScale)) + (MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyBias) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempBias1[b] = 0;
						}
						//tempBias2[b] = (genePool[i+1].genomeBiases[b] * (1.0 - mutationDriftScale)) + (Gaussian.GetRandomGaussian() * mutationDriftScale);
					}
					else
					{
						tempBias1[b] = genePool[i].genomeBiases[b];
						//tempBias2[b] = genePool[i+1].genomeBiases[b];
					}
					mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						//tempBias1[b] = (genePool[i].genomeBiases[b] * (1.0 - mutationDriftScale)) + (Gaussian.GetRandomGaussian() * mutationDriftScale);
						tempBias2[b] = (genePool[i+1].genomeBiases[b] * (1.0 - mutationDriftScale)) + (MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyBias) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempBias2[b] = 0;
						}
					}
					else
					{
						//tempBias1[b] = genePool[i].genomeBiases[b];
						tempBias2[b] = genePool[i+1].genomeBiases[b];
					}
				}
				else
				{
					double mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						tempBias1[b] = (genePool[i+1].genomeBiases[b] * (1.0 - mutationDriftScale)) + (MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyBias) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempBias1[b] = 0;
						}
						//tempBias2[b] = (genePool[i].genomeBiases[b] * (1.0 - mutationDriftScale)) + (Gaussian.GetRandomGaussian() * mutationDriftScale);
					}
					else
					{
						tempBias1[b] = genePool[i+1].genomeBiases[b];
						//tempBias2[b] = genePool[i].genomeBiases[b];
					}
					mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						//tempBias1[b] = (genePool[i+1].genomeBiases[b] * (1.0 - mutationDriftScale)) + (Gaussian.GetRandomGaussian() * mutationDriftScale);
						tempBias2[b] = (genePool[i].genomeBiases[b] * (1.0 - mutationDriftScale)) + (MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyBias) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempBias2[b] = 0;
						}
					}
					else
					{
						//tempBias1[b] = genePool[i+1].genomeBiases[b];
						tempBias2[b] = genePool[i].genomeBiases[b];
					}
				}
			}
			// WEIGHT CROSSOVER
			for( int w = 0; w < weightLength; w++)
			{
				
				
				if( w < spliceWeight)
				{
					// Offspring A:
					double mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						// weighted avg of current value and random weight, so it can't move it too far
						tempWeight1[w] = (genePool[i].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
							(MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyWeight) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempWeight1[w] = 0;
						}
						//tempWeight2[w] = (genePool[i+1].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
						//	(Gaussian.GetRandomGaussian() * mutationDriftScale);
					}
					else
					{
						tempWeight1[w] = genePool[i].genomeWeights[w];
						//tempWeight2[w] = genePool[i+1].genomeWeights[w];
					}
					// Offspring B:
					mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						// weighted avg of current value and random weight, so it can't move it too far
						//tempWeight1[w] = (genePool[i].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
						//	(Gaussian.GetRandomGaussian() * mutationDriftScale);
						tempWeight2[w] = (genePool[i+1].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
							(MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyWeight) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempWeight2[w] = 0;
						}
					}
					else
					{
						//tempWeight1[w] = genePool[i].genomeWeights[w];
						tempWeight2[w] = genePool[i+1].genomeWeights[w];
					}
				}
				else
				{
					// Offspring A:
					double mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						// weighted avg of current value and random weight, so it can't move it too far
						tempWeight1[w] = (genePool[i+1].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
							(MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyWeight) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempWeight1[w] = 0;
						}
						//tempWeight2[w] = (genePool[i].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
						//				   (Gaussian.GetRandomGaussian() * mutationDriftScale);
					}
					else
					{
						tempWeight1[w] = genePool[i+1].genomeWeights[w];
						//tempWeight2[w] = genePool[i].genomeWeights[w];
					}
					// Offspring B:
					mutateRoll = rand.NextDouble();
					if(mutateRoll < mutationRate)
					{
						numMutations++;
						// weighted avg of current value and random weight, so it can't move it too far
						//tempWeight1[w] = (genePool[i+1].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
						//	(Gaussian.GetRandomGaussian() * mutationDriftScale);
						tempWeight2[w] = (genePool[i].genomeWeights[w] * (1.0 - mutationDriftScale)) + 
							(MutateZeroBias(Gaussian.GetRandomGaussian()*maxPreyWeight) * mutationDriftScale);
						double zeroRoll = rand.NextDouble();
						if(zeroRoll < mutationZeroBias) {
							tempWeight2[w] = 0;
						}
					}
					else
					{
						//tempWeight1[w] = genePool[i+1].genomeWeights[w];
						tempWeight2[w] = genePool[i].genomeWeights[w];
					}
				}
			}
			
			//Debug.Log ("numMutations: " + numMutations);
			
			//Add to new Genome Array:
			newPop[2*i] = new Genome(layerSizes);
			newPop[2*i+1] = new Genome(layerSizes);
			
			newPop[2*i].genomeBiases = tempBias1;
			newPop[2*i].genomeWeights = tempWeight1;
			newPop[2*i+1].genomeBiases = tempBias2;
			newPop[2*i+1].genomeWeights = tempWeight2;
		}
		
		//set last member of the population to the genome of the current record-holder
		//newPop[numAgents - 1].genomeBiases = recordGenome.genomeBiases;
		//newPop[numAgents - 1].genomeWeights = recordGenome.genomeWeights;
		
		//Update genePool!!!!!!!! FINALLY!
		genePool = newPop;
	}
	*/
}
