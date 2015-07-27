using UnityEngine;
using System.Collections;

public class Agent {

	public bool debugFunctionCalls = false; // turns debug messages on/off

	public float fitnessScore;

	public BrainBase brain; // This is where the magic happens
	public Genome genome; // encodes the brain network in long single-dimension arrays

	// Constructor Methods:
	public Agent() {
		DebugBot.DebugFunctionCall("Agent; Agent() Constructor!; ", debugFunctionCalls);
	}

	public void InitializeRandomGenome(int[] layerSizes) {
		//genome = new Genome(layerSizes); // instantiate a new random Genome
		brain.InitializeRandomBrain(layerSizes);
	}

	//public void ApplyBrainSettings() {
	//
	//}

	public Agent RefAgent() {
		Agent agentRef = new Agent();
		agentRef = this;
		return agentRef;
	}

	public Agent CopyAgent() {
		Agent agentCopy = new Agent();

		return agentCopy;
	}
}
