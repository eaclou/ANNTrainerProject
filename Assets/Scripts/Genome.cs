using UnityEngine;
using System.Collections;

public class Genome
{
	public float[] genomeWeights;
	public float[] genomeBiases;
	public TransferFunctions.TransferFunction[] geneFunctions;

	public int[] layerSizes;
	
	public Genome()  // constructor for new, blank genome
	{
		// Arrays initialized inside the specific Brain Classes
	}

	public void ZeroGenome() {
		for(int i = 0; i < genomeBiases.Length; i++) {
			genomeBiases[i] = 0f;
		}
		for(int j = 0; j < genomeWeights.Length; j++) {
			genomeWeights[j] = 0f;
		}
	}

	public void PrintBiases(string prefix) {
		string biases = "";
		for(int i = 0; i < genomeBiases.Length; i++) {
			biases += genomeBiases[i].ToString() + ", ";
		}
		DebugBot.DebugFunctionCall (prefix + biases, true);
	}
}
