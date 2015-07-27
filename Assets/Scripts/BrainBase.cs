using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrainBase {

	//public Genome genome;  // The coding for the current brain

	public string Name = "Default";

	public List<FitnessComponent> brainFitnessComponentList;

	public float[][] layerOutput;  // first array specifies the layer, second specifies the nodes within that layer
	public float[][] layerInput;  // first array specifies the layer, second specifies the nodes within that layer 

	//public BrainType brainType = BrainType.None;
	public BrainBase() {
		brainFitnessComponentList = new List<FitnessComponent>();
	}

	public virtual void BrainMasterFunction(ref float[][] inputArray, ref float[][] outputArray) {
		Debug.Log("BrainBase: BrainMasterFunction! ... its brain appears to be ... empty!");
	}

	public virtual void CopyBrainSettingsFrom(BrainBase sourceBrain) {

	}

	public virtual Genome InitializeRandomBrain(int[] layerSizes) {
		Genome genome = new Genome();
		genome.layerSizes = layerSizes;
		return genome;
	}

	public virtual void SetBrainFromGenome(Genome genome) {
		
	}

	public virtual void InitializeBrainFromGenome(Genome genome) { // might not need this one ...

	}
	
}
