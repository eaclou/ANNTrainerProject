using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGameRobotArm : MiniGameBase {

	public static bool debugFunctionCalls = true; // turns debug messages on/off

	// Game State Data:  // CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	public float[] armSegmentA_PosX = new float[1];
	public float[] armSegmentA_PosY = new float[1];
	public float[] armSegmentA_Angle = new float[1];
	public float[] armSegmentA_AngleVel = new float[1];
	public float[] armSegmentA_Length = new float[1];

	public float[] armSegmentB_PosX = new float[1];
	public float[] armSegmentB_PosY = new float[1];
	public float[] armSegmentB_Angle = new float[1];
	public float[] armSegmentB_AngleVel = new float[1];
	public float[] armSegmentB_Length = new float[1];

	public float[] armSegmentC_PosX = new float[1];
	public float[] armSegmentC_PosY = new float[1];
	public float[] armSegmentC_Angle = new float[1];
	public float[] armSegmentC_AngleVel = new float[1];
	public float[] armSegmentC_Length = new float[1];

	public float[] armTip_PosX = new float[1];
	public float[] armTip_PosY = new float[1];

	public float[] armTotalLength = new float[1];

	public float[][] armSegmentArray_PosX;
	public float[][] armSegmentArray_PosY;
	public float[][] armSegmentArray_Angle;
	public float[][] armSegmentArray_AngleVel;
	public float[][] armSegmentArray_Length;
	public float armSegmentMaxBend = Mathf.PI/2f;
	public int numberOfSegments = 6;

	public float[] target_PosX = new float[1];
	public float[] target_PosY = new float[1];
	public float[] target_Angle = new float[1];
	public float[] target_Distance = new float[1];
	
	// Fitness Component Scores:
	public float[] fitTimeToTarget = new float[1];
	public float[] fitDistanceToTarget = new float[1];
	public float[] fitEnergySpent = new float[1];
	public float[] fitGapClosed = new float[1];
	public float[] fitTimeInTarget = new float[1];
	public float[] fitArmReachDistance = new float[1];
	public float[] fitDotProduct = new float[1];
	private float startTargetDistance;
	private float closestTargetDistance;
	public bool collision = false;
	
	// Game Settings:
	
	public float armSpeed = 0.08f;
	public float armThicknessBase = 0.1f;
	public float armThicknessTip = 0.033f;
	public float targetRadius = 0.15f;
	public float targetMaxDistance = 1f;

	private GameObject[] GOarmSegments;

	GameObject GOtargetBall;
	GameObject GOarmSegmentA;
	GameObject GOarmSegmentB;
	GameObject GOarmSegmentC;
	GameObject GOarmTip;

	// Constructor!!
	public MiniGameRobotArm() {
		GOarmSegments = new GameObject[numberOfSegments];
		armSegmentArray_PosX = new float[numberOfSegments][];
		armSegmentArray_PosY = new float[numberOfSegments][];
		armSegmentArray_Angle = new float[numberOfSegments][];;
		armSegmentArray_AngleVel = new float[numberOfSegments][];;
		armSegmentArray_Length = new float[numberOfSegments][];;
		for(int i = 0; i < numberOfSegments; i++) {
			armSegmentArray_PosX[i] = new float[1];
			armSegmentArray_PosY[i] = new float[1];
			armSegmentArray_Angle[i] = new float[1];
			armSegmentArray_AngleVel[i] = new float[1];
			armSegmentArray_Length[i] = new float[1];
		}
		armTip_PosX[0] = 0f;
		armTip_PosY[0] = 0f;
		armTotalLength[0] = 1.6f;

		/*
		armSegmentA_PosX[0] = 0f;
		armSegmentA_PosY[0] = 0f;
		armSegmentA_Length[0] = 0.333334f;
		armSegmentA_Angle[0] = 0f;
		armSegmentA_AngleVel[0] = 0f;
		armSegmentB_PosX[0] = 0f;
		armSegmentB_PosY[0] = 0f;
		armSegmentB_Angle[0] = 0f;
		armSegmentB_AngleVel[0] = 0f;
		armSegmentB_Length[0] = 0.333334f;
		armSegmentC_PosX[0] = 0f;
		armSegmentC_PosY[0] = 0f;
		armSegmentC_Angle[0] = 0f;
		armSegmentC_AngleVel[0] = 0f;
		armSegmentC_Length[0] = 0.333334f;
		
		armTip_PosX[0] = 0f;
		armTip_PosY[0] = 0f;

		target_PosX[0] = 0f;
		target_PosY[0] = 0f;
		target_Angle[0] = 0f;
		target_Distance[0] = 0f;
		*/

		fitTimeToTarget[0] = 1f;
		fitDistanceToTarget[0] = 0f;
		fitEnergySpent[0] = 0f;
		fitGapClosed[0] = 0f;
		fitTimeInTarget[0] = 0f;
		fitArmReachDistance[0] = 0f;
		fitDotProduct[0] = 0f;
		collision = false;
		
		// Brain Inputs!:
		inputChannelsList = new List<BrainInputChannel>();
		// Brain Outputs!:		
		outputChannelsList = new List<BrainOutputChannel>();

		BrainInputChannel BIC_target_Angle = new BrainInputChannel(ref target_Angle, false, "target_Angle");
		inputChannelsList.Add (BIC_target_Angle); // 0
		BrainInputChannel BIC_target_Distance = new BrainInputChannel(ref target_Distance, false, "target_Distance");
		inputChannelsList.Add (BIC_target_Distance); // 1

		for(int bc = 0; bc < numberOfSegments; bc++) {
			string inputChannelName = "Arm Segment " + bc.ToString() + " Angle";
			BrainInputChannel BIC_armSegmentAngle = new BrainInputChannel(ref armSegmentArray_Angle[bc], false, inputChannelName);
			inputChannelsList.Add (BIC_armSegmentAngle);

			string outputChannelName = "Arm Segment " + bc.ToString() + " Angle Vel";
			BrainOutputChannel BOC_armSegmentAngleVel = new BrainOutputChannel(ref armSegmentArray_AngleVel[bc], false, outputChannelName);
			outputChannelsList.Add (BOC_armSegmentAngleVel);
		}

		//BrainInputChannel BIC_armSegmentA_Angle = new BrainInputChannel(ref armSegmentA_Angle, false, "armSegmentA_Angle");
		//inputChannelsList.Add (BIC_armSegmentA_Angle); // 0
		//BrainInputChannel BIC_armSegmentB_Angle = new BrainInputChannel(ref armSegmentB_Angle, false, "armSegmentB_Angle");
		//inputChannelsList.Add (BIC_armSegmentB_Angle); // 1
		//BrainInputChannel BIC_armSegmentC_Angle = new BrainInputChannel(ref armSegmentC_Angle, false, "armSegmentC_Angle");
		//inputChannelsList.Add (BIC_armSegmentC_Angle); // 2

		//BrainOutputChannel BOC_armSegmentA_AngleVel = new BrainOutputChannel(ref armSegmentA_AngleVel, false, "armSegmentA_AngleVel");
		//outputChannelsList.Add (BOC_armSegmentA_AngleVel); // 0
		//BrainOutputChannel BOC_armSegmentB_AngleVel = new BrainOutputChannel(ref armSegmentB_AngleVel, false, "armSegmentB_AngleVel");
		//outputChannelsList.Add (BOC_armSegmentB_AngleVel); // 1
		//BrainOutputChannel BOC_armSegmentC_AngleVel = new BrainOutputChannel(ref armSegmentC_AngleVel, false, "armSegmentC_AngleVel");
		//outputChannelsList.Add (BOC_armSegmentC_AngleVel); // 2
		
		fitnessComponentList = new List<FitnessComponent>();
		FitnessComponent FC_timeToTarget = new FitnessComponent(ref fitTimeToTarget, false, false, 1f, 1f, "Time To Target", true);
		fitnessComponentList.Add (FC_timeToTarget); // 0
		FitnessComponent FC_distanceToTarget = new FitnessComponent(ref fitDistanceToTarget, false, false, 1f, 1f, "Distance To Target", true);
		fitnessComponentList.Add (FC_distanceToTarget); // 1
		FitnessComponent FC_energySpent = new FitnessComponent(ref fitEnergySpent, false, false, 1f, 1f, "Energy Spent", true);
		fitnessComponentList.Add (FC_energySpent); // 2
		FitnessComponent FC_gapClosed = new FitnessComponent(ref fitGapClosed, false, true, 1f, 1f, "Gap Closed", false);
		fitnessComponentList.Add (FC_gapClosed); // 3
		FitnessComponent FC_timeInTarget = new FitnessComponent(ref fitTimeInTarget, false, true, 1f, 1f, "Time Inside Target", true);
		fitnessComponentList.Add (FC_timeInTarget); // 4
		FitnessComponent FC_armReachDistance = new FitnessComponent(ref fitArmReachDistance, false, false, 1f, 1f, "Arm Reach Distance", false);
		fitnessComponentList.Add (FC_armReachDistance); // 5
		FitnessComponent FC_dotProduct = new FitnessComponent(ref fitDotProduct, false, true, 1f, 1f, "Dot Product", false);
		fitnessComponentList.Add (FC_dotProduct); // 6
		
		Reset();
	}
	
	public override void Tick() {  // Runs the mini-game for a single evaluation step.

		float totalAngle = 0f;
		for(int i = 0; i < numberOfSegments; i++) {
			armSegmentArray_Angle[i][0] += armSegmentArray_AngleVel[i][0] * armSpeed;
			totalAngle += armSegmentArray_Angle[i][0];
			if((i+1) < numberOfSegments) {  // not the last armSegment
				armSegmentArray_PosX[i+1][0] = armSegmentArray_Length[i][0] * Mathf.Cos (totalAngle) + armSegmentArray_PosX[i][0];
				armSegmentArray_PosY[i+1][0] = armSegmentArray_Length[i][0] * Mathf.Sin (totalAngle) + armSegmentArray_PosY[i][0];
			}
			else {
				// Arm Tip!
				armTip_PosX[0] = armSegmentArray_Length[i][0] * Mathf.Cos (totalAngle) + armSegmentArray_PosX[i][0];
				armTip_PosY[0] = armSegmentArray_Length[i][0] * Mathf.Sin (totalAngle) + armSegmentArray_PosY[i][0];
			}
		}

		// Rotate arm segments based on their angular momentum (eventually it will be real momentum):
		//armSegmentA_Angle[0] += armSegmentA_AngleVel[0] * armSpeed;
		//if(armSegmentA_Angle[0] > Mathf.PI) {
		//	armSegmentA_Angle[0] -= Mathf.PI * 2f;
		//}
		//if(armSegmentA_Angle[0] < -Mathf.PI) {
		//	armSegmentA_Angle[0] += Mathf.PI * 2f;
		//}
		//armSegmentB_Angle[0] += armSegmentB_AngleVel[0] * armSpeed;
		//if(armSegmentB_Angle[0] > Mathf.PI) {
		//	armSegmentB_Angle[0] -= Mathf.PI * 2f;
		//}
		//if(armSegmentB_Angle[0] < -Mathf.PI) {
		//	armSegmentB_Angle[0] += Mathf.PI * 2f;
		//}
		//armSegmentC_Angle[0] += armSegmentC_AngleVel[0] * armSpeed;
		//if(armSegmentC_Angle[0] > Mathf.PI) {
		//	armSegmentC_Angle[0] -= Mathf.PI * 2f;
		//}
		//if(armSegmentC_Angle[0] < -Mathf.PI) {
		//	armSegmentC_Angle[0] += Mathf.PI * 2f;
		//}
		// calculate position of armSegmentB based on segmentA settings:
		//armSegmentB_PosX[0] = armSegmentA_Length[0] * Mathf.Cos (armSegmentA_Angle[0]) + armSegmentA_PosX[0];
		//armSegmentB_PosY[0] = armSegmentA_Length[0] * Mathf.Sin (armSegmentA_Angle[0]) + armSegmentA_PosY[0];
		// calculate position of armSegmentC based on segmentA&B settings:
		//float totalAngle = armSegmentA_Angle[0] + armSegmentB_Angle[0];
		//armSegmentC_PosX[0] = armSegmentB_Length[0] * Mathf.Cos (totalAngle) + armSegmentB_PosX[0];
		//armSegmentC_PosY[0] = armSegmentB_Length[0] * Mathf.Sin (totalAngle) + armSegmentB_PosY[0];
		// calculate position of armTip:
		//totalAngle += armSegmentC_Angle[0];
		//armTip_PosX[0] = armSegmentC_Length[0] * Mathf.Cos (totalAngle) + armSegmentC_PosX[0];
		//armTip_PosY[0] = armSegmentC_Length[0] * Mathf.Sin (totalAngle) + armSegmentC_PosY[0];

		// calculate target angle in case that helps
		float deltaX = (target_PosX[0] - armSegmentArray_PosX[0][0]);
		if(deltaX > 0) {
			target_Angle[0] = Mathf.Atan ((target_PosY[0] - armSegmentArray_PosY[0][0]) / deltaX);
		}
		else if(deltaX < 0) {
			target_Angle[0] = Mathf.Atan ((target_PosY[0] - armSegmentArray_PosY[0][0]) / deltaX) + Mathf.PI;
		}
		//target_Angle[0] = Mathf.Atan ((target_PosY[0] - armSegmentA_PosY[0]) / (target_PosX[0] - armSegmentA_PosX[0]));
		//if(target_Angle[0] > Mathf.PI) {
		//	target_Angle[0] -= Mathf.PI * 2f;
		//}
		//if(target_Angle[0] < -Mathf.PI) {
		//	target_Angle[0] += Mathf.PI * 2f;
		//}

		//if((target_PosX[0] - armSegmentA_PosX[0]) != 0f) {
		//	target_Angle[0] = Mathf.Atan ((target_PosY[0] - armSegmentA_PosY[0]) / (target_PosX[0] - armSegmentA_PosX[0]));
		//}
		//else {
		//	if((target_PosY[0] - armSegmentA_PosY[0]) > 0f) {
		//		target_Angle[0] = Mathf.PI / 2f;
		//	}
		//	else {
		//		target_Angle[0] = -Mathf.PI / 2f;
		//	}
		//}
		target_Distance[0] = new Vector2(target_PosX[0]-armSegmentArray_PosX[0][0], target_PosY[0]-armSegmentArray_PosY[0][0]).magnitude;

		// FITNESS COMPONENTS

		float tipDistanceToTarget = new Vector2(target_PosX[0]-armTip_PosX[0], target_PosY[0]-armTip_PosY[0]).magnitude;
		fitDistanceToTarget[0] += (tipDistanceToTarget) / (targetMaxDistance + armTotalLength[0]);  // get in 0-1 range
		if(tipDistanceToTarget < closestTargetDistance) {
			closestTargetDistance = tipDistanceToTarget;
		}
		fitGapClosed[0] = (startTargetDistance - closestTargetDistance) / 2f;

		for(int e = 0; e < numberOfSegments; e++) {
			fitEnergySpent[0] += Mathf.Abs (armSegmentArray_AngleVel[e][0])/(float)numberOfSegments;
		}
		//fitEnergySpent[0] += (Mathf.Abs (armSegmentA_AngleVel[0]) + Mathf.Abs (armSegmentB_AngleVel[0]) + Mathf.Abs (armSegmentC_AngleVel[0]))/3f;

		//fitArmReachDistance
		float originToTipDistance = new Vector2(armTip_PosX[0] - armSegmentArray_PosX[0][0], armTip_PosY[0] - armSegmentArray_PosY[0][0]).magnitude;
		//fitArmReachDistance[0] += Mathf.Abs (originToTipDistance - target_Distance[0]) / (2f*targetMaxDistance);
		fitArmReachDistance[0] = Mathf.Abs (originToTipDistance - target_Distance[0]) / (targetMaxDistance + armTotalLength[0]);

		//fitDotProduct  between tipVector and targetVector
		float dotProduct = ((target_PosX[0]-armSegmentArray_PosX[0][0]) * (armTip_PosX[0]-armSegmentArray_PosX[0][0])) + ((target_PosY[0]-armSegmentArray_PosY[0][0]) * (armTip_PosY[0]-armSegmentArray_PosY[0][0]));
		//fitDotProduct[0] += (dotProduct + 1f) / 2f;  // get in 0-1 range
		fitDotProduct[0] = (dotProduct + 1f) / 2f;  // get in 0-1 range


		// FitTimeInTarget
		if(tipDistanceToTarget <= targetRadius) { // radius of agents
			fitTimeInTarget[0] += 1f;
			collision = true;
			//fitReachTarget[0] = 1f;
		}
		if(collision) {
			fitTimeToTarget[0] += 0f;
		} else {
			fitTimeToTarget[0] += 1f;
		}
	}

	public override void Reset() {
		//Debug.Log ("PrevTargetAngle= " + target_Angle[0].ToString() + ", armA_Angle= " + armSegmentA_Angle[0].ToString() + ", armA_AngleVel= " + armSegmentA_AngleVel[0].ToString() + ", TimeInTarget= " + fitTimeInTarget[0].ToString());

		armSegmentArray_PosX[0][0] = 0f; // origin of arm at (0, 0);
		armSegmentArray_PosY[0][0] = 0f;
		//float totalPosX = armSegmentArray_PosX[0][0];
		//float totalPosY = armSegmentArray_PosY[0][0];
		float totalAngle = 0f;
		for(int segment = 0; segment < numberOfSegments; segment++) {
			armSegmentArray_Length[segment][0] = armTotalLength[0]/(float)numberOfSegments;
			armSegmentArray_Angle[segment][0] = UnityEngine.Random.Range (-armSegmentMaxBend, armSegmentMaxBend);
			//armSegmentArray_Angle[segment][0] = 0f;
			totalAngle += armSegmentArray_Angle[segment][0];
			if((segment+1) < numberOfSegments) {  // not the last armSegment

				// hypoteneuse * cos(angle) + offset
				armSegmentArray_PosX[segment+1][0] = armSegmentArray_Length[segment][0] * Mathf.Cos (totalAngle) + armSegmentArray_PosX[segment][0];
				armSegmentArray_PosY[segment+1][0] = armSegmentArray_Length[segment][0] * Mathf.Sin (totalAngle) + armSegmentArray_PosY[segment][0];
				//totalPosX = armSegmentArray_PosX[segment+1][0];
				//totalPosY = armSegmentArray_PosY[segment+1][0];
			}
			else {
				// Arm Tip!
				armTip_PosX[0] = armSegmentArray_Length[segment][0] * Mathf.Cos (totalAngle) + armSegmentArray_PosX[segment][0];
				armTip_PosY[0] = armSegmentArray_Length[segment][0] * Mathf.Sin (totalAngle) + armSegmentArray_PosY[segment][0];
			}

		}
		//targetIsMoving[0] = 0f;
		/*
		armSegmentA_PosX[0] = 0f;
		armSegmentA_PosY[0] = 0f;
		armSegmentA_Angle[0] = UnityEngine.Random.Range (0f, 2f*Mathf.PI);
		armSegmentA_AngleVel[0] = 0f;
		armSegmentB_PosX[0] = 0f;
		armSegmentB_PosY[0] = 0f;
		armSegmentB_Angle[0] = 0f; //UnityEngine.Random.Range (0f, 2f*Mathf.PI);
		armSegmentB_AngleVel[0] = 0f;
		armSegmentC_PosX[0] = 0f;
		armSegmentC_PosY[0] = 0f;
		armSegmentC_Angle[0] = 0f; // UnityEngine.Random.Range (0f, 2f*Mathf.PI);
		armSegmentC_AngleVel[0] = 0f;
		*/
		fitTimeToTarget[0] = 1f;
		fitDistanceToTarget[0] = 0f;
		fitEnergySpent[0] = 0f;
		fitGapClosed[0] = 0f;
		fitTimeInTarget[0] = 0f;
		fitArmReachDistance[0] = 0f;
		fitDotProduct[0] = 0f;
		collision = false;

		closestTargetDistance = 0f;



		float initTargetDir = UnityEngine.Random.Range (-Mathf.PI, Mathf.PI);
		float initTargetDistance = UnityEngine.Random.Range (targetMaxDistance*0.2f, targetMaxDistance);
		//float initTargetDistance = targetMaxDistance;

		target_Angle[0] = initTargetDir;
		target_Distance[0] = initTargetDistance;
		target_PosX[0] = initTargetDistance * Mathf.Cos (initTargetDir);
		target_PosY[0] = initTargetDistance * Mathf.Sin (initTargetDir);
		//target_Angle[0] = initTargetDir;

		// calculate position of armSegmentB based on segmentA settings:
		//armSegmentB_PosX[0] = armSegmentA_Length[0] * Mathf.Cos (armSegmentA_Angle[0]) + armSegmentA_PosX[0];
		//armSegmentB_PosY[0] = armSegmentA_Length[0] * Mathf.Sin (armSegmentA_Angle[0]) + armSegmentA_PosY[0];
		// calculate position of armSegmentC based on segmentA&B settings:
		//float totalAngle = armSegmentA_Angle[0] + armSegmentB_Angle[0];
		//armSegmentC_PosX[0] = armSegmentB_Length[0] * Mathf.Cos (totalAngle) + armSegmentB_PosX[0];
		//armSegmentC_PosY[0] = armSegmentB_Length[0] * Mathf.Sin (totalAngle) + armSegmentB_PosY[0];
		// calculate position of armTip:
		//totalAngle += armSegmentC_Angle[0];
		//armTip_PosX[0] = armSegmentC_Length[0] * Mathf.Cos (totalAngle) + armSegmentC_PosX[0];
		//armTip_PosY[0] = armSegmentC_Length[0] * Mathf.Sin (totalAngle) + armSegmentC_PosY[0];

		startTargetDistance = new Vector2(armSegmentArray_PosX[0][0]-armTip_PosX[0], armSegmentArray_PosY[0][0]-armTip_PosY[0]).magnitude;
	}
	
	public void CheckBoundaryConstraints() {

	}

	public override void BuildGamePieces() {

		// Create a gameObject with the specified gamePiece script on it (saved as a prefab) and parent it under ArenaGroup
		GOtargetBall = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/RobotArm/GamePieceRobotArmTarget", typeof(GameObject))) as GameObject;
		GOtargetBall.GetComponent<GamePieceRobotArmTarget>().InitGamePiece();
		GOtargetBall.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		GOarmTip = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/RobotArm/GamePieceRobotArmTarget", typeof(GameObject))) as GameObject;
		GOarmTip.GetComponent<GamePieceRobotArmTarget>().InitGamePiece();
		GOarmTip.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		// Arm Segments:
		for(int i = 0; i < numberOfSegments; i++) {
			GOarmSegments[i] = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/RobotArm/GamePieceRobotArmCube", typeof(GameObject))) as GameObject;
			GOarmSegments[i].GetComponent<GamePieceRobotArmCube>().InitGamePiece();
			GOarmSegments[i].transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);
		}

		/*
		GOarmSegmentA = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/RobotArm/GamePieceRobotArmCube", typeof(GameObject))) as GameObject;
		GOarmSegmentA.GetComponent<GamePieceRobotArmCube>().InitGamePiece();
		GOarmSegmentA.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		GOarmSegmentB = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/RobotArm/GamePieceRobotArmCube", typeof(GameObject))) as GameObject;
		GOarmSegmentB.GetComponent<GamePieceRobotArmCube>().InitGamePiece();
		GOarmSegmentB.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		GOarmSegmentC = MonoBehaviour.Instantiate(Resources.Load ("MiniGames/RobotArm/GamePieceRobotArmCube", typeof(GameObject))) as GameObject;
		GOarmSegmentC.GetComponent<GamePieceRobotArmCube>().InitGamePiece();
		GOarmSegmentC.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);
		*/



		piecesBuilt = true;

	}
	
	public override void DeleteGamePieces() {
		
	}
	
	public override void VisualizeGameState() {


		Vector3 newTargetPos = new Vector3(target_PosX[0], target_PosY[0], 0f);
		GOtargetBall.transform.localPosition = newTargetPos;
		GOtargetBall.transform.localScale = new Vector3(targetRadius*1f, targetRadius*1f, targetRadius*1f);

		Vector3 armTip_Pos = new Vector3(armTip_PosX[0], armTip_PosY[0], 0f);
		GOarmTip.transform.localPosition = armTip_Pos;
		GOarmTip.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

		// Arm Segments:
		float totalAngle = 0f;
		totalAngle = armSegmentArray_Angle[0][0];
		for(int i = 0; i < numberOfSegments; i++) {
			//Debug.Log ("BEFORE-segmentArrayAngle " + i.ToString() + ": " + armSegmentArray_Angle[i][0].ToString() + ", totalAngle: " + totalAngle.ToString() + ", localRotation: " + GOarmSegments[i].transform.localRotation.eulerAngles.ToString());
			Vector3 segmentPos = new Vector3(armSegmentArray_PosX[i][0], armSegmentArray_PosY[i][0], 0f);
			GOarmSegments[i].transform.localPosition = segmentPos;
			float armTaper = (float)i/(float)numberOfSegments;
			GOarmSegments[i].transform.localScale = new Vector3(armSegmentArray_Length[i][0]*1.1f, Mathf.Lerp (armThicknessBase, armThicknessTip, armTaper), Mathf.Lerp (armThicknessBase, armThicknessTip, armTaper));
			//GOarmSegments[i].transform.localScale = new Vector3(armThickness, armThickness, armThickness);
			GOarmSegments[i].transform.localRotation = Quaternion.Euler(0f, 0f, totalAngle*180f/Mathf.PI);
			if((i+1) < numberOfSegments) {
				totalAngle = armSegmentArray_Angle[i+1][0] + totalAngle;
			}
			//Debug.Log ("AFTER-segmentArrayAngle " + i.ToString() + ": " + armSegmentArray_Angle[i][0].ToString() + ", totalAngle: " + totalAngle.ToString() + ", localRotation: " + GOarmSegments[i].transform.localRotation.eulerAngles.ToString());

		}

		/*
		Vector3 segmentA_Pos = new Vector3(armSegmentA_PosX[0], armSegmentA_PosY[0], 0f);
		GOarmSegmentA.transform.localPosition = segmentA_Pos;
		GOarmSegmentA.transform.localScale = new Vector3(armSegmentA_Length[0]*1f, armThickness, armThickness);
		GOarmSegmentA.transform.localRotation = Quaternion.Euler(0f, 0f, armSegmentA_Angle[0]*180f/Mathf.PI);

		Vector3 segmentB_Pos = new Vector3(armSegmentB_PosX[0], armSegmentB_PosY[0], 0f);
		GOarmSegmentB.transform.localPosition = segmentB_Pos;
		GOarmSegmentB.transform.localScale = new Vector3(armSegmentB_Length[0]*1f, armThickness, armThickness);
		GOarmSegmentB.transform.localRotation = Quaternion.Euler(0f, 0f, (armSegmentA_Angle[0] + armSegmentB_Angle[0])*180f/Mathf.PI);

		Vector3 segmentC_Pos = new Vector3(armSegmentC_PosX[0], armSegmentC_PosY[0], 0f);
		GOarmSegmentC.transform.localPosition = segmentC_Pos;
		GOarmSegmentC.transform.localScale = new Vector3(armSegmentC_Length[0]*1f, armThickness, armThickness);
		GOarmSegmentC.transform.localRotation = Quaternion.Euler(0f, 0f, (armSegmentA_Angle[0] + armSegmentB_Angle[0] + armSegmentC_Angle[0])*180f/Mathf.PI);
		*/

		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosZ", (ownPosZ[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosZ", (targetPosZ[0]));
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]+1f)/2f);
		//ArenaGroup.arenaGroupStatic.gameObject.transform.GetChild(2).GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]+1f)/2f);
	}
}
