using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameMoveToTarget1D : MiniGameBase {

	public static bool debugFunctionCalls = false; // turns debug messages on/off

	// Game State Data:  // CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	public float[] ownPosX = new float[1];
	public float[] targetPosX = new float[1];
	public float[] ownVelX = new float[1];
	public float[] targetVelX = new float[1];
	public float[] targetDirX = new float[1];
	// Alternate approach:  Try leaving all as floats and throw them all into an Array?

	// Fitness Component Scores:
	public float[] fitDotProduct = new float[1];
	public float[] fitTimeToTarget = new float[1];
	public float[] fitDistanceToTarget = new float[1];
	public float[] fitReachTarget = new float[1];
	public bool collision = false;
	
	// Game Settings:
	public float boundaryPosX = 1f;
	public float boundaryNegX = -1f;
	
	public float speed = 0.1f;
	public float agentRadii = 0.05f;


	// Constructor Method:
	public MiniGameMoveToTarget1D() {
		//DebugBot.DebugFunctionCall("MiniGameMoveToTarget1D; Constructor();", debugFunctionCalls);
		ownPosX[0] = 0f;
		targetPosX[0] = 0f;
		ownVelX[0] = 0f;
		targetVelX[0] = 0f;
		targetDirX[0] = 0f;

		fitDotProduct[0] = 0f;
		fitTimeToTarget[0] = 1f;
		fitDistanceToTarget[0] = 0f;
		fitReachTarget[0] = 0f;
		collision = false;

		// Brain Inputs!:
		inputChannelsList = new List<BrainInputChannel>();
		BrainInputChannel BIC_ownPosX = new BrainInputChannel(ref ownPosX, false, "ownPosX");
		inputChannelsList.Add (BIC_ownPosX); // 0
		BrainInputChannel BIC_targetPosX = new BrainInputChannel(ref targetPosX, false, "targetPosX");
		inputChannelsList.Add (BIC_targetPosX); // 1
		BrainInputChannel BIC_ownVelX = new BrainInputChannel(ref ownVelX, false, "ownVelX");
		inputChannelsList.Add (BIC_ownVelX); // 2
		BrainInputChannel BIC_targetVelX = new BrainInputChannel(ref targetVelX, false, "targetVelX");
		inputChannelsList.Add (BIC_targetVelX); // 3
		BrainInputChannel BIC_targetDirX = new BrainInputChannel(ref targetDirX, false, "targetDirX");
		inputChannelsList.Add (BIC_targetDirX); // 4
		// Brain Outputs!:

		outputChannelsList = new List<BrainOutputChannel>();
		BrainOutputChannel BOC_ownVelX = new BrainOutputChannel(ref ownVelX, false, "ownVelX");
		outputChannelsList.Add (BOC_ownVelX); // 0

		fitnessComponentList = new List<FitnessComponent>();
		FitnessComponent FC_dotProduct = new FitnessComponent(ref fitDotProduct, false, true, 1f, 1f, "Dot Product To Target", true);
		fitnessComponentList.Add (FC_dotProduct); // 0
		FitnessComponent FC_timeToTarget = new FitnessComponent(ref fitTimeToTarget, false, false, 1f, 1f, "Time To Target", true);
		fitnessComponentList.Add (FC_timeToTarget); // 1
		FitnessComponent FC_distanceToTarget = new FitnessComponent(ref fitDistanceToTarget, false, false, 1f, 1f, "Distance To Target", true);
		fitnessComponentList.Add (FC_distanceToTarget); // 2
		FitnessComponent FC_reachTarget = new FitnessComponent(ref fitReachTarget, false, true, 1f, 1f, "Reaches Target", false);
		fitnessComponentList.Add (FC_reachTarget); // 3

		Reset();
	}

	// Primary Function
	public override void Tick() {  // Runs the mini-game for a single evaluation step.
		ownPosX[0] += ownVelX[0] * speed;
		targetPosX[0] += targetVelX[0] * speed;
		CheckBoundaryConstraints();

		float targetDir = targetPosX[0] - ownPosX[0];
		if(targetDir > 0f) { targetDir = 1f; }
		else if(targetDir < 0f) { targetDir = -1f; }
		targetDirX[0] = targetDir;
		if(fitnessComponentList[0].on) { // only calculate what is needed -- dotProduct
			float ownDir = ownVelX[0];
			if(ownDir > 0f) { ownDir = 1f; }
			else if(ownDir < 0f) { ownDir = -1f; }
			float dotProduct = targetDir * ownDir;
			fitDotProduct[0] += (dotProduct + 1f) / 2f;  // get in 0-1 range
		}
		//if(fitnessComponentList[2].on) { // only calculate what is needed -- average distance
		float distance = targetPosX[0]-ownPosX[0];
		fitDistanceToTarget[0] += (distance) / 2f;  // get in 0-1 range -- max distance is sqrt(8) = 2.83
		//}
		if(distance <= agentRadii*2f) { // radius of agents
			collision = true;
			fitReachTarget[0] = 1f;
		}
		if(collision) {
			fitTimeToTarget[0] += 0f;
		} else {
			fitTimeToTarget[0] += 1f;
		}

		//VisualizeGameState();
		//DebugBot.DebugFunctionCall("MiniGameMoveToTarget1D; Tick(); ownPosX= " + ownPosX[0].ToString () + ", targPosX= " + targetPosX[0].ToString() + ", ownvelX= " + ownVelX[0].ToString () + ", targetVelX= " + targetVelX[0].ToString(), debugFunctionCalls);
	}

	public override void Reset() {
		//DebugBot.DebugFunctionCall("MiniGameMoveToTarget1D; Reset();", debugFunctionCalls);
		fitDotProduct[0] = 0f;
		fitTimeToTarget[0] = 1f;
		fitDistanceToTarget[0] = 0f;
		fitReachTarget[0] = 0f;
		collision = false;

		float initOwnPosX = UnityEngine.Random.Range(boundaryNegX, boundaryPosX);
		ownPosX[0] = initOwnPosX;
		float initTargetPosX = UnityEngine.Random.Range(boundaryNegX, boundaryPosX);
		targetPosX[0] = initTargetPosX;
		float targetDir = targetPosX[0] - ownPosX[0];
		if(targetDir > 0f) { targetDir = 1f; }
		else if(targetDir < 0f) { targetDir = -1f; }
		targetDirX[0] = targetDir;
	}

	public void CheckBoundaryConstraints() {
		if(ownPosX[0] < boundaryNegX) {
			ownPosX[0] = boundaryNegX;
		}
		if(ownPosX[0] > boundaryPosX) {
			ownPosX[0] = boundaryPosX;
		}

		if(targetPosX[0] < boundaryNegX) {
			targetPosX[0] = boundaryNegX;
		}
		if(targetPosX[0] > boundaryPosX) {
			targetPosX[0] = boundaryPosX;
		}
	}

	public override void BuildGamePieces() {
		
	}
	
	public override void DeleteGamePieces() {
		
	}

	public override void VisualizeGameState() {
		Vector3 newOwnPos = new Vector3(ownPosX[0], 0f, 0f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(0).transform.localPosition = newOwnPos;
		Vector3 newTargetPos = new Vector3(targetPosX[0], 0f, 0f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(1).transform.localPosition = newTargetPos;

		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]+1f)/2f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosY", 0.5f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]+1f)/2f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosY", 0.5f);
	}
}
