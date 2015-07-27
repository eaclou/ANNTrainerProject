using UnityEngine;
using System.Collections;

public class GenerationData {

	// GenerationData
	//     Holds data for all Agents for this generation
	//     Holds value for number of Agents in this gen
	public AgentData[] agentDataArray;
	public float totalAgentScoresRaw;
	public float totalAgentScoresWeighted;
	public float avgAgentScoreRaw;
	public float avgAgentScoreWeighted;
	public int totalNumFitnessComponents;
	public Genome genAvgGenome;
	
	public GenerationData(int numAgents) {
		agentDataArray = new AgentData[numAgents];
		genAvgGenome = new Genome();
	}
}
