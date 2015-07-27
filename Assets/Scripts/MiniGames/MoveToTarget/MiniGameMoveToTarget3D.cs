using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameMoveToTarget3D : MiniGameBase {

	public static bool debugFunctionCalls = true; // turns debug messages on/off

	// Game State Data:  // CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	public float[] ownPosX = new float[1];
	public float[] ownPosY = new float[1];
	public float[] ownPosZ = new float[1];
	public float[] targetPosX = new float[1];
	public float[] targetPosY = new float[1];
	public float[] targetPosZ = new float[1];
	public float[] ownVelX = new float[1];
	public float[] ownVelY = new float[1];
	public float[] ownVelZ = new float[1];
	public float[] targetVelX = new float[1];
	public float[] targetVelY = new float[1];
	public float[] targetVelZ = new float[1];
	public float[] targetDirX = new float[1];
	public float[] targetDirY = new float[1];
	public float[] targetDirZ = new float[1];
	
	// Fitness Component Scores:
	public float[] fitDotProduct = new float[1];
	public float[] fitTimeToTarget = new float[1];
	public float[] fitDistanceToTarget = new float[1];
	public float[] fitReachTarget = new float[1];
	public bool collision = false;
	
	// Game Settings:
	public float boundaryPosX = 1f;
	public float boundaryNegX = -1f;
	public float boundaryPosY = 1f;
	public float boundaryNegY = -1f;
	public float boundaryPosZ = 1f;
	public float boundaryNegZ = -1f;
	
	public float speed = 0.06f;
	public float preySpeed = 0.07f;
	public float agentRadii = 0.1f;

	private float totalRandX = 0f;
	private float totalRandY = 0f;
	private float totalRandZ = 0f;

	GameObject GOtargetBall;
	GameObject GOagentBall;
	GameObject GOboundaryWalls;

	// Constructor!!
	public MiniGameMoveToTarget3D() {
		ownPosX[0] = 0f;
		ownPosY[0] = 0f;
		ownPosZ[0] = 0f;
		targetPosX[0] = 0f;
		targetPosY[0] = 0f;
		targetPosZ[0] = 0f;
		ownVelX[0] = 0f;
		ownVelY[0] = 0f;
		ownVelZ[0] = 0f;
		targetVelX[0] = 0f;
		targetVelY[0] = 0f;
		targetVelZ[0] = 0f;
		
		fitDotProduct[0] = 0f;
		fitTimeToTarget[0] = 1f;
		fitDistanceToTarget[0] = 0f;
		fitReachTarget[0] = 0f;
		collision = false;
		
		// Brain Inputs!:
		inputChannelsList = new List<BrainInputChannel>();
		BrainInputChannel BIC_ownPosX = new BrainInputChannel(ref ownPosX, false, "ownPosX");
		inputChannelsList.Add (BIC_ownPosX); // 0
		BrainInputChannel BIC_ownPosY = new BrainInputChannel(ref ownPosY, false, "ownPosY");
		inputChannelsList.Add (BIC_ownPosY); // 1
		BrainInputChannel BIC_ownPosZ = new BrainInputChannel(ref ownPosZ, false, "ownPosZ");
		inputChannelsList.Add (BIC_ownPosZ); // 2
		BrainInputChannel BIC_targetPosX = new BrainInputChannel(ref targetPosX, false, "targetPosX");
		inputChannelsList.Add (BIC_targetPosX); // 3
		BrainInputChannel BIC_targetPosY = new BrainInputChannel(ref targetPosY, false, "targetPosY");
		inputChannelsList.Add (BIC_targetPosY); // 4
		BrainInputChannel BIC_targetPosZ = new BrainInputChannel(ref targetPosZ, false, "targetPosZ");
		inputChannelsList.Add (BIC_targetPosZ); // 5
		BrainInputChannel BIC_ownVelX = new BrainInputChannel(ref ownVelX, false, "ownVelX");
		inputChannelsList.Add (BIC_ownVelX); // 6
		BrainInputChannel BIC_ownVelY = new BrainInputChannel(ref ownVelY, false, "ownVelY");
		inputChannelsList.Add (BIC_ownVelY); // 7
		BrainInputChannel BIC_ownVelZ = new BrainInputChannel(ref ownVelZ, false, "ownVelZ");
		inputChannelsList.Add (BIC_ownVelZ); // 8
		BrainInputChannel BIC_targetDirX = new BrainInputChannel(ref targetDirX, false, "targetDirX");
		inputChannelsList.Add (BIC_targetDirX); // 9
		BrainInputChannel BIC_targetDirY = new BrainInputChannel(ref targetDirY, false, "targetDirY");
		inputChannelsList.Add (BIC_targetDirY); // 10
		BrainInputChannel BIC_targetDirZ = new BrainInputChannel(ref targetDirZ, false, "targetDirZ");
		inputChannelsList.Add (BIC_targetDirZ); // 11
		// Brain Outputs!:
		
		outputChannelsList = new List<BrainOutputChannel>();
		BrainOutputChannel BOC_ownVelX = new BrainOutputChannel(ref ownVelX, false, "ownVelX");
		outputChannelsList.Add (BOC_ownVelX); // 0
		BrainOutputChannel BOC_ownVelY = new BrainOutputChannel(ref ownVelY, false, "ownVelY");
		outputChannelsList.Add (BOC_ownVelY); // 1
		BrainOutputChannel BOC_ownVelZ = new BrainOutputChannel(ref ownVelZ, false, "ownVelZ");
		outputChannelsList.Add (BOC_ownVelZ); // 1
		
		fitnessComponentList = new List<FitnessComponent>();
		FitnessComponent FC_dotProduct = new FitnessComponent(ref fitDotProduct, true, true, 1f, 1f, "Dot Product To Target", true);
		fitnessComponentList.Add (FC_dotProduct); // 0
		FitnessComponent FC_timeToTarget = new FitnessComponent(ref fitTimeToTarget, true, false, 1f, 1f, "Time To Target", true);
		fitnessComponentList.Add (FC_timeToTarget); // 1
		FitnessComponent FC_distanceToTarget = new FitnessComponent(ref fitDistanceToTarget, true, false, 1f, 1f, "Distance To Target", true);
		fitnessComponentList.Add (FC_distanceToTarget); // 2
		FitnessComponent FC_reachTarget = new FitnessComponent(ref fitReachTarget, true, true, 1f, 1f, "Reaches Target", false);
		fitnessComponentList.Add (FC_reachTarget); // 3
		
		Reset();
	}
	
	public override void Tick() {  // Runs the mini-game for a single evaluation step.
		//DebugBot.DebugFunctionCall("MiniGameMoveToTarget3D; Tick();", debugFunctionCalls);
		ownPosX[0] += ownVelX[0] * speed;
		ownPosY[0] += ownVelY[0] * speed;
		ownPosZ[0] += ownVelZ[0] * speed;
		//targetPosX[0] += targetVelX[0] * speed;
		//targetPosY[0] += targetVelY[0] * speed;
		//targetPosZ[0] += targetVelZ[0] * speed;
		// +++++++++==== TEMP TARGET MOVEMENT ====+++++++++++
		Vector3 hunterPosition = new Vector3(ownPosX[0], ownPosY[0], ownPosZ[0]);
		Vector3 preyPosition = new Vector3(targetPosX[0], targetPosY[0], targetPosZ[0]);
		Vector3 hunterDir = hunterPosition - preyPosition;
		float hunterDistance = (hunterPosition-preyPosition).sqrMagnitude;
		hunterDistance = 1f/hunterDistance;
		Vector3 originDir = -preyPosition;
		float wallProximity = preyPosition.sqrMagnitude*1.6f;
		float lerpValue = wallProximity*hunterDistance;
		float randX = UnityEngine.Random.Range(-0.4f, 0.4f);
		float randY = UnityEngine.Random.Range(-0.4f, 0.4f);
		float randZ = UnityEngine.Random.Range(-0.4f, 0.4f);
		totalRandX += Mathf.Lerp (totalRandX, randX, 0.8f)*0.4f; // smoothes out the random noise? constant multiplier controls how quickly influence drops over time
		totalRandY += Mathf.Lerp (totalRandY, randY, 0.8f)*0.4f;
		totalRandZ += Mathf.Lerp (totalRandZ, randZ, 0.8f)*0.4f;
		Vector3 randDir = new Vector3(totalRandX, totalRandY, totalRandZ);
		Vector3 moveDir = Vector3.Lerp(-hunterDir, originDir*2.6f, lerpValue);
		moveDir = Vector3.Lerp(moveDir, randDir, lerpValue*0.4f).normalized;
		targetVelX[0] = moveDir.x;
		targetVelY[0] = moveDir.y;
		targetVelZ[0] = moveDir.z;
		targetPosX[0] += targetVelX[0] * preySpeed;
		targetPosY[0] += targetVelY[0] * preySpeed;
		targetPosZ[0] += targetVelZ[0] * preySpeed;
		CheckBoundaryConstraints();

		Vector3 targetDir = new Vector3(targetPosX[0] - ownPosX[0], targetPosY[0] - ownPosY[0], targetPosZ[0] - ownPosZ[0]).normalized;
		targetDirX[0] = targetDir.x;
		targetDirY[0] = targetDir.y;
		targetDirZ[0] = targetDir.z;
		if(fitnessComponentList[0].on) { // only calculate what is needed -- dotProduct
			Vector3 ownDir = new Vector3(ownVelX[0], ownVelY[0], ownVelZ[0]).normalized;
			float dotProduct = targetDir.x * ownDir.x + targetDir.y * ownDir.y + targetDir.z * ownDir.z;
			fitDotProduct[0] += (dotProduct + 1f) / 2f;  // get in 0-1 range
		}
		//if(fitnessComponentList[2].on) { // only calculate what is needed -- average distance
		float distance = new Vector3(targetPosX[0]-ownPosX[0], targetPosY[0]-ownPosY[0], targetPosZ[0]-ownPosZ[0]).magnitude;
		//float distance = offset.magnitude;
		fitDistanceToTarget[0] += (distance) / 3.46f;  // get in 0-1 range -- max distance is sqrt(12) = 3.46
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
	}

	public override void Reset() {
		//Debug.Log(fitnessScore.ToString ());
		//fitnessScore = 0f;
		fitDotProduct[0] = 0f;
		fitTimeToTarget[0] = 1f;
		fitDistanceToTarget[0] = 0f;
		fitReachTarget[0] = 0f;
		collision = false;

		totalRandX = 0f;
		totalRandY = 0f;
		totalRandZ = 0f;
		//DebugBot.DebugFunctionCall("MiniGameMoveToTarget1D; Reset();", debugFunctionCalls);
		float initOwnPosX = UnityEngine.Random.Range(boundaryNegX, boundaryPosX);
		ownPosX[0] = initOwnPosX;
		float initOwnPosY = UnityEngine.Random.Range(boundaryNegY, boundaryPosY);
		//ownPosY[0] = initOwnPosY;
		ownPosY[0] = -1f;
		float initOwnPosZ = UnityEngine.Random.Range(boundaryNegZ, boundaryPosZ);
		ownPosZ[0] = initOwnPosZ;
		float initTargetPosX = UnityEngine.Random.Range(boundaryNegX, boundaryPosX);
		targetPosX[0] = initTargetPosX;
		float initTargetPosY = UnityEngine.Random.Range(boundaryNegY, boundaryPosY);
		//targetPosY[0] = initTargetPosY;
		targetPosY[0] = 1f;
		float initTargetPosZ = UnityEngine.Random.Range(boundaryNegZ, boundaryPosZ);
		targetPosZ[0] = initTargetPosZ;
		Vector3 targetDir = new Vector3(targetPosX[0] - ownPosX[0], targetPosY[0] - ownPosY[0], targetPosZ[0] - ownPosZ[0]).normalized;
		targetDirX[0] = targetDir.x;
		targetDirY[0] = targetDir.y;
		targetDirZ[0] = targetDir.z;
	}
	
	public void CheckBoundaryConstraints() {
		if(ownPosX[0] < boundaryNegX + agentRadii) {
			ownPosX[0] = boundaryNegX + agentRadii;
		}
		if(ownPosX[0] > boundaryPosX - agentRadii) {
			ownPosX[0] = boundaryPosX - agentRadii;
		}		
		if(targetPosX[0] < boundaryNegX + agentRadii) {
			targetPosX[0] = boundaryNegX + agentRadii;
		}
		if(targetPosX[0] > boundaryPosX - agentRadii) {
			targetPosX[0] = boundaryPosX - agentRadii;
		}
		if(ownPosY[0] < boundaryNegY + agentRadii) {
			ownPosY[0] = boundaryNegY + agentRadii;
		}
		if(ownPosY[0] > boundaryPosY - agentRadii) {
			ownPosY[0] = boundaryPosY - agentRadii;
		}		
		if(targetPosY[0] < boundaryNegY + agentRadii) {
			targetPosY[0] = boundaryNegY + agentRadii;
		}
		if(targetPosY[0] > boundaryPosY - agentRadii) {
			targetPosY[0] = boundaryPosY - agentRadii;
		}
		if(ownPosZ[0] < boundaryNegZ + agentRadii) {
			ownPosZ[0] = boundaryNegZ + agentRadii;
		}
		if(ownPosZ[0] > boundaryPosZ - agentRadii) {
			ownPosZ[0] = boundaryPosZ - agentRadii;
		}		
		if(targetPosZ[0] < boundaryNegZ + agentRadii) {
			targetPosZ[0] = boundaryNegZ + agentRadii;
		}
		if(targetPosZ[0] > boundaryPosZ - agentRadii) {
			targetPosZ[0] = boundaryPosZ - agentRadii;
		}
	}

	public override void BuildGamePieces() {

		// Create a gameObject with the specified gamePiece script on it (saved as a prefab) and parent it under ArenaGroup
		GOboundaryWalls = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/MoveToTarget/GamePieceMoveToTargetWalls", typeof(GameObject))) as GameObject;
		GOboundaryWalls.GetComponent<GamePieceMoveToTargetWalls>().InitGamePiece();
		GOboundaryWalls.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		GOagentBall = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/MoveToTarget/GamePieceMoveToTargetAgent", typeof(GameObject))) as GameObject;
		GOagentBall.GetComponent<GamePieceMoveToTargetAgent>().InitGamePiece();
		GOagentBall.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		GOtargetBall = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/MoveToTarget/GamePieceMoveToTargetTarget", typeof(GameObject))) as GameObject;
		GOtargetBall.GetComponent<GamePieceMoveToTargetTarget>().InitGamePiece();
		GOtargetBall.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		piecesBuilt = true;
	}
	
	public override void DeleteGamePieces() {
		
	}
	
	public override void VisualizeGameState() {
		Vector3 newOwnPos = new Vector3(ownPosX[0], ownPosY[0], ownPosZ[0]);
		GOagentBall.transform.localPosition = newOwnPos;
		GOagentBall.transform.localScale = new Vector3(agentRadii*1f, agentRadii*1f, agentRadii*1f);
		Vector3 newTargetPos = new Vector3(targetPosX[0], targetPosY[0], targetPosZ[0]);
		GOtargetBall.transform.localPosition = newTargetPos;
		GOtargetBall.transform.localScale = new Vector3(agentRadii*1f, agentRadii*1f, agentRadii*1f);

		GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]));
		GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]));
		GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosZ", (ownPosZ[0]));
		GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]));
		GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]));
		GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosZ", (targetPosZ[0]));
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]+1f)/2f);



	}
}
