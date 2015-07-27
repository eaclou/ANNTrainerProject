using UnityEngine;
using System.Collections;

public class GenomeBase
{
	public float[] genomeWeights;
	public float[] genomeBiases;
	public TransferFunctions.TransferFunction[] geneFunctions;
	
	public GenomeBase()  // constructor for new, blank genome
	{

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
