using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;

public class BrainANN_FF_Layers_A2A : BrainBase {

	// Specific Braintype options:
	public int numHiddenLayers = 0;
	public int[] numNodesEachLayer;



	// Layer Count doesn't include Input Layer, since it's simply a buffer that is not manipulated
	private int layerCount;  // number of 'real' layers (excluding input layer)
	private int inputSize;  // the number of input layer nodes
	private int[] layerSize;  // the number of nodes within each of the 'real' layers
	//public TransferFunction[] transferFunction;  // determines the transfer function to be used on each layer/node

	// MOVED TO BASE CLASS:
	//public float[][] layerOutput;  // first array specifies the layer, second specifies the nodes within that layer
	//public float[][] layerInput;  // first array specifies the layer, second specifies the nodes within that layer 

	private float[][] bias;  // the bias term to be added to the weights before being evaluated by the transfer function
	private TransferFunctions.TransferFunction[][] transferFunctions;
	private float[][] delta;  // the delta between the output value for that node and the target value
	private float[][] previousBiasDelta; // the delta of the bias term for the previous epoch - will be used when calculating Momentum for the back-propogation algorithm
	
	private float[][][] weight;   // first array is the layer, second is the source node, third is the target node
	private float[][][] previousWeightDelta;  // weights of the preceding epoch, to be used for calculating momentum

	// Fitness Components:
	public float[] fitConnectionCost = new float[1];

	public BrainANN_FF_Layers_A2A() {
		Name = "BrainANN_FF_Layers_A2A";

		// nothing
		//Debug.Log("BrainANN_FF_Layers_A2A() Constructor!");
		//temp:
		fitConnectionCost[0] = 0f;

		brainFitnessComponentList = new List<FitnessComponent>();
		FitnessComponent FC_connectionCost = new FitnessComponent(ref fitConnectionCost, false, false, 1f, 1f, "Connection Cost", false);
		// Turn back on when Fitness Panel can handle it!
		//brainFitnessComponentList.Add (FC_connectionCost); // 0

	}

	public override void BrainMasterFunction(ref float[][] inputArray, ref float[][] outputArray) {
		// Make sure we have enough data
		
		//Dimension
		//output = new float[layerSize[layerCount - 1]];  // set output size to that of last layer (output)



		
		// Run the network! 
		for (int l = 0; l < layerCount; l++)
		{
			for (int j = 0; j < layerSize[l]; j++)  // for each node in the current layer l,
			{
				float sum = 0.0f;
				for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++) { // for each node in the previous layer l-1, 
					sum += weight[l][i][j] * (l == 0 ? inputArray[i][0] : layerOutput[l - 1][i]);  // sum for that node is the sum of all inputs multiplied by weights from previous layer
					//float value = weight[l][i][j] * (l == 0 ? inputArray[i][0] : layerOutput[l - 1][i]);
					//Debug.Log("BrainANN_FF_Layers_A2A() BrainMasterFunction! inputArray[" + i.ToString() + "][0]= " + inputArray[i][0].ToString () + " weight= " + weight[l][i][j].ToString () + " value= " + value.ToString() + " sum= " + sum.ToString ());
				}
				sum += bias[l][j];  // add bias value to this node
				layerInput[l][j] = sum;  // store layerInput values for this layer
				
				layerOutput[l][j] = TransferFunctions.Evaluate(transferFunctions[l][j], sum); // pass the sum into the transferFunction for that layer
				//Debug.Log("BrainANN_FF_Layers_A2A() BrainMasterFunction! bias= " + bias[l][j].ToString () + " weight= " + weight[l][0][j].ToString () + " sum= " + sum.ToString () + " layerOutput[][]= " + layerOutput[l][j].ToString());
			}
		}
		
		//Copy the output to the output array
		string outputString = "";
		for (int m = 0; m < layerSize[layerCount - 1]; m++) {
			outputArray[m][0] = layerOutput[layerCount - 1][m];  // check for value-type pass problems
			outputString += outputArray[m][0].ToString() + ", ";
		}

		string inputString = "";
		for(int n = 0; n < inputSize; n++) {
			inputString += inputArray[n][0].ToString () + ", ";
		}
		//Debug.Log("BrainANN_FF_Layers_A2A() BrainMasterFunction! inputArray= " + inputString + " -- outputArray= " + outputString);
	}

	/*
	// Run the network forwards to produce output values
	public void Run(ref float[] input, out float[] output)
	{
		// Make sure we have enough data
		if (input.Length != inputSize)
			throw new ArgumentException("Input data is not of the correct dimension.");
		
		//Dimension
		output = new float[layerSize[layerCount - 1]];  // set output size to that of last layer (output)
		
		// Run the network! 
		for (int l = 0; l < layerCount; l++)
		{
			for (int j = 0; j < layerSize[l]; j++)  // for each node in the current layer l,
			{
				float sum = 0.0f;
				for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)  // for each node in the previous layer l-1, 
					sum += weight[l][i][j] * (l == 0 ? input[i] : layerOutput[l - 1][i]);  // sum for that node is the sum of all inputs multiplied by weights from previous layer
				
				sum += bias[l][j];  // add bias value to this node
				layerInput[l][j] = sum;  // store layerInput values for this layer
				
				layerOutput[l][j] = TransferFunctions.Evaluate(transferFunctions[l][j], sum); // pass the sum into the transferFunction for that layer
			}
		}
		
		//Copy the output to the output array
		for (int i = 0; i < layerSize[layerCount - 1]; i++)
			output[i] = layerOutput[layerCount - 1][i];
	}
	*/

	
	public override void CopyBrainSettingsFrom(BrainBase sourceBrain) {
		
	}

	public override Genome InitializeRandomBrain(int[] layerSizes)
	{
		// Validate the input data


		Genome genome = new Genome();
		genome.layerSizes = layerSizes; // save the brain layer dimensions inside the agent's genome
		int biasGenomeLength = 0;
		int weightGenomeLength = 0;
		int biasIndex = 0;
		int weightIndex = 0;

		// Initialize network layers
		layerCount = layerSizes.Length - 1;  // doesn't include input layer, so one less than layerSizes
		inputSize = layerSizes[0];  // set size of input layer
		
		layerSize = new int[layerCount]; // keeps track of the number of nodes in each layer
		for (int i = 0; i < layerCount; i++) // transferring layer sizes function input (which included input layer) into new variable that does not include input layer nodes
			layerSize[i] = layerSizes[i + 1];

		// OG transfer function code:
		//transferFunction = new TransferFunction[layerCount];
		//for (int i = 0; i < layerCount; i++) // transferring transferFunction function input (which included input layer) into new variable that does not include input layer
		//	transferFunction[i] = transferFunctions[i + 1];
		
		// Start dimensioning arrays
		bias = new float[layerCount][];  // setting length of first array, which specifies number of 'real' layers
		previousBiasDelta = new float[layerCount][];
		delta = new float[layerCount][];
		layerOutput = new float[layerCount][];
		layerInput = new float[layerCount][];
		transferFunctions = new TransferFunctions.TransferFunction[layerCount][];
		
		weight = new float[layerCount][][]; // setting length of first array, which specifies number of 'real' layers
		previousWeightDelta = new float[layerCount][][];
		
		// Fill 2 dimensional arrays
		for (int l = 0; l < layerCount; l++) // iterate through second arrays and set number of nodes (second array)
		{
			bias[l] = new float[layerSize[l]];
			biasGenomeLength += layerSize[l];  // keep track of bias Genome Length

			previousBiasDelta[l] = new float[layerSize[l]];
			delta[l] = new float[layerSize[l]];
			layerOutput[l] = new float[layerSize[l]];
			layerInput[l] = new float[layerSize[l]];
			transferFunctions[l] = new TransferFunctions.TransferFunction[layerSize[l]];
			
			weight[l] = new float[l == 0 ? inputSize : layerSize[l - 1]][];  // if l (layer) is 0 (first hidden layer), then it needs weights fromt he input layer, which is not included in layer sizes 
			previousWeightDelta[l] = new float[l == 0 ? inputSize : layerSize[l - 1]][];  //.... so it uses the inputSize variable instead, which holds the nubmer of nodes int he input layer
			// .... else it uses the layerSize of the previous layer
			
			// need to set the size of the current layer's nodes that we're concerned with (third array)
			// iterates through the (second array) previous layer's nodes and sets the target nodes in the current layer
			for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)  // if l = 0, previous layer is inputLayer, else: layer - 1
			{
				weight[l][i] = new float[layerSize[l]];
				weightGenomeLength += layerSize[l];

				previousWeightDelta[l][i] = new float[layerSize[l]];
			}
		}

		//Debug.Log ("BiasGenomeLength= " + biasGenomeLength.ToString () + ", weightGenomeLength= " + weightGenomeLength.ToString ());
		genome.genomeBiases = new float[biasGenomeLength];
		genome.geneFunctions = new TransferFunctions.TransferFunction[biasGenomeLength];
		genome.genomeWeights = new float[weightGenomeLength];
		
		// Initialize the weights
		for (int l = 0; l < layerCount; l++)  // iterate through the 'real' layers
		{
			for (int j = 0; j < layerSize[l]; j++)  // for 2-dimensional arrays, iterate through current layer
			{
				bias[l][j] = Gaussian.GetRandomGaussian();  // normally distributed
				//bias[l][j] = 0f;  // start zeroed
				genome.genomeBiases[biasIndex] = bias[l][j];
				transferFunctions[l][j] = TransferFunctions.TransferFunction.RationalSigmoid;
				genome.geneFunctions[biasIndex] = transferFunctions[l][j];
				biasIndex++;

				previousBiasDelta[l][j] = 0.0f;   // init to 0
				layerOutput[l][j] = 0.0f;
				layerInput[l][j] = 0.0f;
				delta[l][j] = 0.0f;

			}
			
			for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++) // for 3-dimensional arrays, iterate through nodes in previous layer (second array)
			{
				for (int j = 0; j < layerSize[l]; j++)  // iterate through nodes in current layer
				{
					weight[l][i][j] = Gaussian.GetRandomGaussian(); // init connective weights to standard distribution around 0.0
					genome.genomeWeights[weightIndex] = weight[l][i][j];
					weightIndex++;

					previousWeightDelta[l][i][j] = 0.0f;
				}
			}
		}
		return genome;
	}

	public override void SetBrainFromGenome(Genome genome)
	{
		int genomeBiasIndex = 0;
		int genomeWeightIndex = 0;
		
		// Initialize the weights
		for (int l = 0; l < layerCount; l++)  // iterate through the 'real' layers
		{
			for (int j = 0; j < layerSize[l]; j++)  // for 2-dimensional arrays, iterate through current layer
			{
				bias[l][j] = genome.genomeBiases[genomeBiasIndex];  // normally distributed
				transferFunctions[l][j] = genome.geneFunctions[genomeBiasIndex];
				genomeBiasIndex += 1;
				
				//previousBiasDelta[l][j] = 0.0;   // init to 0
				//layerOutput[l][j] = 0.0;
				//layerInput[l][j] = 0.0;
				//delta[l][j] = 0.0;
			}
			
			for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++) // for 3-dimensional arrays, iterate through nodes in previous layer (second array)
			{
				for (int j = 0; j < layerSize[l]; j++)  // iterate through nodes in current layer
				{
					weight[l][i][j] = genome.genomeWeights[genomeWeightIndex]; // init connective weights to standard distribution around 0.0
					genomeWeightIndex += 1;
					//previousWeightDelta[l][i][j] = 0.0;
				}
			}
		}
		CalculateConnectionCost();
	}

	private void CalculateConnectionCost() {

	}

	
	public override void InitializeBrainFromGenome(Genome genome)
	{
		int genomeBiasIndex = 0;
		int genomeWeightIndex = 0;
		
		// Initialize the weights
		for (int l = 0; l < layerCount; l++)  // iterate through the 'real' layers
		{
			for (int j = 0; j < layerSize[l]; j++)  // for 2-dimensional arrays, iterate through current layer
			{
				bias[l][j] = genome.genomeBiases[genomeBiasIndex];  // normally distributed
				transferFunctions[l][j] = genome.geneFunctions[genomeBiasIndex];
				genomeBiasIndex += 1;
				
				//previousBiasDelta[l][j] = 0.0;   // init to 0
				//layerOutput[l][j] = 0.0;
				//layerInput[l][j] = 0.0;
				//delta[l][j] = 0.0;
			}
			
			for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++) // for 3-dimensional arrays, iterate through nodes in previous layer (second array)
			{
				for (int j = 0; j < layerSize[l]; j++)  // iterate through nodes in current layer
				{
					weight[l][i][j] = genome.genomeWeights[genomeWeightIndex]; // init connective weights to standard distribution around 0.0
					genomeWeightIndex += 1;
					//previousWeightDelta[l][i][j] = 0.0;
				}
			}
		}




		/*
		float[] geneBias = codingGenome.genomeBiases;
		float[] geneWeight = codingGenome.genomeWeights;
		TransferFunctions.TransferFunction[] geneFunction = codingGenome.geneFunctions;
		// Validate the input data

		// Initialize network layers
		layerCount = layerSizes.Length - 1;  // doesn't include input layer, so one less than layerSizes
		inputSize = layerSizes[0];  // set size of input layer
		
		int genomeBiasIndex = 0;
		int genomeWeightIndex = 0;
		
		layerSize = new int[layerCount]; // keeps track of the number of nodes in each layer
		for (int i = 0; i < layerCount; i++) // transferring layer sizes function input (which included input layer) into new variable that does not include input layer nodes
			layerSize[i] = layerSizes[i + 1];

		// OLD:
		//transferFunction = new TransferFunction[layerCount];
		//for (int i = 0; i < layerCount; i++) // transferring transferFunction function input (which included input layer) into new variable that does not include input layer
		//	transferFunction[i] = transferFunctions[i + 1];
		
		// Start dimensioning arrays
		bias = new float[layerCount][];  // setting length of first array, which specifies number of 'real' layers
		previousBiasDelta = new float[layerCount][];
		delta = new float[layerCount][];
		layerOutput = new float[layerCount][];
		layerInput = new float[layerCount][];
		transferFunctions = new TransferFunctions.TransferFunction[layerCount][];
		
		weight = new float[layerCount][][]; // setting length of first array, which specifies number of 'real' layers
		previousWeightDelta = new float[layerCount][][];
		
		// Fill 2 dimensional arrays
		for (int l = 0; l < layerCount; l++) // iterate through second arrays and set number of nodes (second array)
		{
			bias[l] = new float[layerSize[l]]; 
			previousBiasDelta[l] = new float[layerSize[l]];
			delta[l] = new float[layerSize[l]];
			layerOutput[l] = new float[layerSize[l]];
			layerInput[l] = new float[layerSize[l]];
			transferFunctions[l] = new TransferFunctions.TransferFunction[layerSize[l]];
			
			weight[l] = new float[l == 0 ? inputSize : layerSize[l - 1]][];  // if l (layer) is 0 (first hidden layer), then it needs weights fromt he input layer, which is not included in layer sizes 
			previousWeightDelta[l] = new float[l == 0 ? inputSize : layerSize[l - 1]][];  //.... so it uses the inputSize variable instead, which holds the nubmer of nodes int he input layer
			// .... else it uses the layerSize of the previous layer
			
			// need to set the size of the current layer's nodes that we're concerned with (third array)
			// iterates through the (second array) previous layer's nodes and sets the target nodes in the current layer
			for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++)  // if l = 0, previous layer is inputLayer, else: layer - 1
			{
				weight[l][i] = new float[layerSize[l]];
				previousWeightDelta[l][i] = new float[layerSize[l]];
			}
		}
		
		// Initialize the weights
		for (int l = 0; l < layerCount; l++)  // iterate through the 'real' layers
		{
			for (int j = 0; j < layerSize[l]; j++)  // for 2-dimensional arrays, iterate through current layer
			{
				bias[l][j] = geneBias[genomeBiasIndex]; // set biases for each node
				transferFunctions[l][j] = geneFunction[genomeBiasIndex]; // set transfer functions for each node
				genomeBiasIndex += 1;
				
				previousBiasDelta[l][j] = 0.0f;   // init to 0
				layerOutput[l][j] = 0.0f;
				layerInput[l][j] = 0.0f;
				delta[l][j] = 0.0f;
			}
			
			for (int i = 0; i < (l == 0 ? inputSize : layerSize[l - 1]); i++) // for 3-dimensional arrays, iterate through nodes in previous layer (second array)
			{
				for (int j = 0; j < layerSize[l]; j++)  // iterate through nodes in current layer
				{
					weight[l][i][j] = geneWeight[genomeWeightIndex]; 
					genomeWeightIndex += 1;
					previousWeightDelta[l][i][j] = 0.0f;
				}
			}
		}
		*/

	}
}
