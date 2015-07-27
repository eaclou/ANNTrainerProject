using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameMoveToTarget2D : MiniGameBase {


	public static bool debugFunctionCalls = true; // turns debug messages on/off
	
	// Game State Data:  // CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	public float[] ownPosX = new float[1];
	public float[] ownPosY = new float[1];
	public float[] targetPosX = new float[1];
	public float[] targetPosY = new float[1];
	public float[] ownVelX = new float[1];
	public float[] ownVelY = new float[1];
	public float[] targetVelX = new float[1];
	public float[] targetVelY = new float[1];
	public float[] targetDirX = new float[1];
	public float[] targetDirY = new float[1];

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

	public float speed = 0.08f;
	public float preySpeed = 0.06f;
	public float agentRadii = 0.05f;

	private float totalRandX = 0f;
	private float totalRandY = 0f;


	
	// Constructor Method:
	public MiniGameMoveToTarget2D() {
		DebugBot.DebugFunctionCall("MiniGameMoveToTarget2D; Constructor();", debugFunctionCalls);
		ownPosX[0] = 0f;
		ownPosY[0] = 0f;
		targetPosX[0] = 0f;
		targetPosY[0] = 0f;
		ownVelX[0] = 0f;
		ownVelY[0] = 0f;
		targetVelX[0] = 0f;
		targetVelY[0] = 0f;

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
		BrainInputChannel BIC_targetPosX = new BrainInputChannel(ref targetPosX, false, "targetPosX");
		inputChannelsList.Add (BIC_targetPosX); // 2
		BrainInputChannel BIC_targetPosY = new BrainInputChannel(ref targetPosY, false, "targetPosY");
		inputChannelsList.Add (BIC_targetPosY); // 3
		BrainInputChannel BIC_ownVelX = new BrainInputChannel(ref ownVelX, false, "ownVelX");
		inputChannelsList.Add (BIC_ownVelX); // 4
		BrainInputChannel BIC_ownVelY = new BrainInputChannel(ref ownVelY, false, "ownVelY");
		inputChannelsList.Add (BIC_ownVelY); // 5
		BrainInputChannel BIC_targetVelX = new BrainInputChannel(ref targetVelX, false, "targetVelX");
		inputChannelsList.Add (BIC_targetVelX); // 6
		BrainInputChannel BIC_targetVelY = new BrainInputChannel(ref targetVelY, false, "targetVelY");
		inputChannelsList.Add (BIC_targetVelY); // 7
		BrainInputChannel BIC_targetDirX = new BrainInputChannel(ref targetDirX, false, "targetDirX");
		inputChannelsList.Add (BIC_targetDirX); // 6
		BrainInputChannel BIC_targetDirY = new BrainInputChannel(ref targetDirY, false, "targetDirY");
		inputChannelsList.Add (BIC_targetDirY); // 7
		// Brain Outputs!:
		
		outputChannelsList = new List<BrainOutputChannel>();
		BrainOutputChannel BOC_ownVelX = new BrainOutputChannel(ref ownVelX, false, "ownVelX");
		outputChannelsList.Add (BOC_ownVelX); // 0
		BrainOutputChannel BOC_ownVelY = new BrainOutputChannel(ref ownVelY, false, "ownVelY");
		outputChannelsList.Add (BOC_ownVelY); // 1

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
		ownPosY[0] += ownVelY[0] * speed;
		//targetPosX[0] += targetVelX[0] * speed;
		//targetPosY[0] += targetVelY[0] * speed;
		// +++++++++==== TEMP TARGET MOVEMENT ====+++++++++++
		Vector2 hunterPosition = new Vector2(ownPosX[0], ownPosY[0]);
		Vector2 preyPosition = new Vector2(targetPosX[0], targetPosY[0]);
		Vector2 hunterDir = hunterPosition - preyPosition;
		float hunterDistance = (hunterPosition-preyPosition).sqrMagnitude;
		hunterDistance = 1f/hunterDistance;
		Vector2 originDir = -preyPosition;
		float wallProximity = preyPosition.sqrMagnitude;
		float lerpValue = wallProximity*hunterDistance;
		float randX = UnityEngine.Random.Range(-0.25f, 0.25f);
		float randY = UnityEngine.Random.Range(-0.25f, 0.25f);
		totalRandX += Mathf.Lerp (totalRandX, randX, 0.8f)*0.4f; // smoothes out the random noise? constant multiplier controls how quickly influence drops over time
		totalRandY += Mathf.Lerp (totalRandY, randY, 0.8f)*0.4f;
		Vector2 randDir = new Vector2(totalRandX, totalRandY);
		Vector2 moveDir = Vector2.Lerp(-hunterDir, originDir*1.6f, lerpValue);
		moveDir = Vector2.Lerp(moveDir, randDir, lerpValue*0.4f).normalized;
		targetVelX[0] = moveDir.x;
		targetVelY[0] = moveDir.y;
		targetPosX[0] += targetVelX[0] * preySpeed;
		targetPosY[0] += targetVelY[0] * preySpeed;
		CheckBoundaryConstraints();



		// fitness as accumulated distance:
		//fitnessScore += (targetPosX[0] - ownPosX[0])*(targetPosX[0] - ownPosX[0]) + (targetPosY[0] - ownPosY[0])*(targetPosY[0] - ownPosY[0]);
		//fitness as dot-product:
		//Vector2 targetDir = new Vector2(targetPosX[0] - ownPosX[0], targetPosY[0] - ownPosY[0]).normalized;
		Vector2 targetDir = new Vector2(targetPosX[0] - ownPosX[0], targetPosY[0] - ownPosY[0]).normalized;
		targetDirX[0] = targetDir.x;
		targetDirY[0] = targetDir.y;
		if(fitnessComponentList[0].on) { // only calculate what is needed -- dotProduct
			Vector2 ownDir = new Vector2(ownVelX[0], ownVelY[0]).normalized;
			float dotProduct = targetDir.x * ownDir.x + targetDir.y * ownDir.y;
			fitDotProduct[0] += (dotProduct + 1f) / 2f;  // get in 0-1 range
		}
		//if(fitnessComponentList[2].on) { // only calculate what is needed -- average distance
		Vector2 offset = new Vector2(targetPosX[0]-ownPosX[0], targetPosY[0]-ownPosY[0]);
		float distance = offset.magnitude;
		fitDistanceToTarget[0] += (distance) / 2.83f;  // get in 0-1 range -- max distance is sqrt(8) = 2.83
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
		//Debug.Log ("fitDotProduct[0]: " + fitDotProduct[0].ToString() + ", fitnessComponentList[0]: " + fitnessComponentList[0].componentScore[0].ToString() + ", outputChannel: " + outputChannelsList[0].channelValue[0].ToString());

		//fitnessScore += dotProduct; // OBSOLETE SOON!!!!
		/*DebugBot.DebugFunctionCall("MiniGameMoveToTarget2D; Tick(); ownPosX= " + ownPosX[0].ToString () + 
		                           ", ownPosY= " + ownPosY[0].ToString() + 
		                           ", targetPosX= " + targetPosX[0].ToString () + 
		                           ", targetPosY= " + targetVelY[0].ToString() +
		                           ", ownvelX= " + ownVelX[0].ToString () + 
		                           ", ownvelY= " + ownVelY[0].ToString () + 
		                           ", targetVelX= " + targetVelX[0].ToString () + 
		                           ", targetVelY= " + targetVelY[0].ToString (), debugFunctionCalls);*/
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
		//DebugBot.DebugFunctionCall("MiniGameMoveToTarget1D; Reset();", debugFunctionCalls);
		float initOwnPosX = UnityEngine.Random.Range(boundaryNegX, boundaryPosX);
		ownPosX[0] = initOwnPosX;
		float initTargetPosX = UnityEngine.Random.Range(boundaryNegX, boundaryPosX);
		targetPosX[0] = initTargetPosX;
		float initOwnPosY = UnityEngine.Random.Range(boundaryNegY, boundaryPosY);
		ownPosY[0] = initOwnPosY;
		float initTargetPosY = UnityEngine.Random.Range(boundaryNegY, boundaryPosY);
		targetPosY[0] = initTargetPosY;
		Vector2 targetDir = new Vector2(targetPosX[0] - ownPosX[0], targetPosY[0] - ownPosY[0]).normalized;
		targetDirX[0] = targetDir.x;
		targetDirY[0] = targetDir.y;
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
		if(ownPosY[0] < boundaryNegY) {
			ownPosY[0] = boundaryNegY;
		}
		if(ownPosY[0] > boundaryPosY) {
			ownPosY[0] = boundaryPosY;
		}
		
		if(targetPosY[0] < boundaryNegY) {
			targetPosY[0] = boundaryNegY;
		}
		if(targetPosY[0] > boundaryPosY) {
			targetPosY[0] = boundaryPosY;
		}
	}

	public override void BuildGamePieces() {

	}

	public override void DeleteGamePieces() {

	}
	
	public override void VisualizeGameState() {
		Vector3 newOwnPos = new Vector3(ownPosX[0], ownPosY[0], 0f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(0).transform.localPosition = newOwnPos;
		Vector3 newTargetPos = new Vector3(targetPosX[0], targetPosY[0], 0f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(1).transform.localPosition = newTargetPos;

		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]+1f)/2f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]+1f)/2f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]+1f)/2f);
		ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]+1f)/2f);
	}
}
