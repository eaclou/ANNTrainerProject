using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniGamePhysXWorm : MiniGameBase {

	public static bool debugFunctionCalls = true; // turns debug messages on/off

	public float[][] wormSegmentArray_PosX;
	public float[][] wormSegmentArray_PosY;
	public float[][] wormSegmentArray_PosZ;
	public float[][] wormSegmentArray_Angle;
	public float[][] wormSegmentArray_MotorTarget;
	public float[][] wormSegmentArray_Length;

	public float[] targetPosX;
	public float[] targetPosY;
	public float[] targetPosZ;
	public float[] targetDirX;
	public float[] targetDirY;
	public float[] targetDirZ;


	// Game Settings:
	public float[] armTotalLength = new float[1];
	public float armSegmentMaxBend = 60f;
	public int numberOfSegments = 8;
	public float wormSegmentThickness = 0.075f;
	public float jointMotorForce = 30f;
	public float jointMotorSpeed = 100f;
	public float viscosityDrag = 2000f;
	public float maxScoreDistance = 4f;
	public float targetRadius = 0.4f;

	// Fitness Component Scores:
	public float[] fitDistFromOrigin = new float[1];
	public float[] fitEnergySpent = new float[1];
	public float[] fitDistToTarget = new float[1];
	public float[] fitTimeToTarget = new float[1];

	// Game Pieces!
	private GameObject[] GOwormSegments;
	private GameObject GOtargetSphere;

	// Surface Areas for each pair of faces (neg x will be same as pos x):
	private float[] sa_x;
	private float[] sa_y;
	private float[] sa_z;

	// Constructor!!
	public MiniGamePhysXWorm() {
		piecesBuilt = false;  // This refers to if the pieces have all their components and are ready for Simulation, not simply instantiated empty GO's
		gameInitialized = false;  // Reset() is Initialization
		gameTicked = false;  // Has the game been ticked on its current TimeStep
		gameUpdatedFromPhysX = false;  // Did the game just get updated from PhysX Simulation?
		gameCurrentTimeStep = 0;  // Should only be able to increment this if the above are true (which means it went through gameLoop for this timeStep)

		sa_x = new float[numberOfSegments];
		sa_y = new float[numberOfSegments];
		sa_z = new float[numberOfSegments];
		targetPosX = new float[1];
		targetPosY = new float[1];
		targetPosZ = new float[1];
		targetDirX = new float[1];
		targetDirY = new float[1];
		targetDirZ = new float[1];
		targetDirX[0] = 0f;
		targetDirY[0] = 0f;
		targetDirZ[0] = 0f;

		armTotalLength[0] = 2.0f;
		GOwormSegments = new GameObject[numberOfSegments];
		wormSegmentArray_PosX = new float[numberOfSegments][];
		wormSegmentArray_PosY = new float[numberOfSegments][];
		wormSegmentArray_PosZ = new float[numberOfSegments][];
		wormSegmentArray_Angle = new float[numberOfSegments][];;
		wormSegmentArray_MotorTarget = new float[numberOfSegments][];;
		wormSegmentArray_Length = new float[numberOfSegments][];;

		//GOtargetSphere = new GameObject("GOtargetSphere");
		//GOtargetSphere.transform.localScale = new Vector3(targetRadius, targetRadius, targetRadius);

		for(int i = 0; i < numberOfSegments; i++) {
			//string name = "GOwormSegment" + i.ToString();
			//GOwormSegments[i] = new GameObject(name);
			//GOwormSegments[i].transform.localPosition = new Vector3(0f, 0f, 0f); // RE-EVALUATE!!!
			wormSegmentArray_PosX[i] = new float[1];
			wormSegmentArray_PosY[i] = new float[1];
			wormSegmentArray_PosZ[i] = new float[1];
			wormSegmentArray_Angle[i] = new float[1];
			wormSegmentArray_MotorTarget[i] = new float[1];
			wormSegmentArray_Length[i] = new float[1];
			// Calculate surface areas for each face:
			wormSegmentArray_Length[i][0] = armTotalLength[0]/(float)numberOfSegments;
			//GOwormSegments[i].transform.localScale = new Vector3(wormSegmentArray_Length[i][0], wormSegmentThickness, wormSegmentThickness*2f);
			//sa_x[i] = GOwormSegments[i].transform.localScale.y * GOwormSegments[i].transform.localScale.z;
			//sa_y[i] = GOwormSegments[i].transform.localScale.x * GOwormSegments[i].transform.localScale.z;
			//sa_z[i] = GOwormSegments[i].transform.localScale.x * GOwormSegments[i].transform.localScale.y;
		}

		fitDistFromOrigin[0] = 0f;
		fitEnergySpent[0] = 0f;
		fitDistToTarget[0] = 0f;
		fitTimeToTarget[0] = 0f;

		// Brain Inputs!:
		inputChannelsList = new List<BrainInputChannel>();
		BrainInputChannel BIC_targetDirX = new BrainInputChannel(ref targetDirX, false, "TargetDir X");
		inputChannelsList.Add (BIC_targetDirX);
		BrainInputChannel BIC_targetDirY = new BrainInputChannel(ref targetDirY, false, "TargetDir Y");
		inputChannelsList.Add (BIC_targetDirY);
		BrainInputChannel BIC_targetDirZ = new BrainInputChannel(ref targetDirZ, false, "TargetDir Z");
		inputChannelsList.Add (BIC_targetDirZ);
		// Brain Outputs!:		
		outputChannelsList = new List<BrainOutputChannel>();
		
		for(int bc = 0; bc < numberOfSegments; bc++) {
			string inputChannelName = "Worm Segment " + bc.ToString() + " Angle";
			BrainInputChannel BIC_wormSegmentAngle = new BrainInputChannel(ref wormSegmentArray_Angle[bc], false, inputChannelName);
			inputChannelsList.Add (BIC_wormSegmentAngle);
			
			string outputChannelName = "Worm Segment " + bc.ToString() + " Motor Target";
			BrainOutputChannel BOC_wormSegmentAngleVel = new BrainOutputChannel(ref wormSegmentArray_MotorTarget[bc], false, outputChannelName);
			outputChannelsList.Add (BOC_wormSegmentAngleVel);
		}
		
		fitnessComponentList = new List<FitnessComponent>();
		FitnessComponent FC_distFromOrigin = new FitnessComponent(ref fitDistFromOrigin, true, true, 1f, 1f, "Distance From Origin", true);
		fitnessComponentList.Add (FC_distFromOrigin); // 0
		FitnessComponent FC_energySpent = new FitnessComponent(ref fitEnergySpent, true, false, 1f, 1f, "Energy Spent", true);
		fitnessComponentList.Add (FC_energySpent); // 1
		FitnessComponent FC_distToTarget = new FitnessComponent(ref fitDistToTarget, true, false, 1f, 1f, "Distance To Target", false);
		fitnessComponentList.Add (FC_distToTarget); // 2
		FitnessComponent FC_timeToTarget = new FitnessComponent(ref fitTimeToTarget, true, false, 1f, 1f, "Time To Target", true);
		fitnessComponentList.Add (FC_timeToTarget); // 3
		
		//Reset();
	}

	public void ApplyViscosityForces(GameObject body, int segmentIndex, float drag) {
		Rigidbody rigidBod = body.GetComponent<Rigidbody>();
		// Cache positive axis vectors:
		Vector3 forward = body.transform.forward;
		Vector3 up = body.transform.up;
		Vector3 right = body.transform.right;
		// Find centers of each of box's faces
		Vector3 xpos_face_center = (right * body.transform.localScale.x / 2f) + body.transform.position;
		Vector3 ypos_face_center = (up * body.transform.localScale.y / 2f) + body.transform.position;
		Vector3 zpos_face_center = (forward * body.transform.localScale.z / 2f) + body.transform.position;
		Vector3 xneg_face_center = -(right * body.transform.localScale.x / 2f) + body.transform.position;
		Vector3 yneg_face_center = -(up * body.transform.localScale.y / 2f) + body.transform.position;
		Vector3 zneg_face_center = -(forward * body.transform.localScale.z / 2f) + body.transform.position;

		// FRONT (posZ):
		Vector3 pointVelPosZ = rigidBod.GetPointVelocity (zpos_face_center); // Get velocity of face's center (doesn't catch torque around center of mass)
		float velPosZ = Vector3.Dot (forward, pointVelPosZ) * pointVelPosZ.sqrMagnitude;   // get the proportion of the velocity vector in the direction of face's normal (0 - 1) times magnitude squared
		Vector3 fluidDragVecPosZ = -forward *    // in the direction opposite the face's normal
									velPosZ *    // 
									sa_z[segmentIndex] * viscosityDrag;  // multiplied by face's surface area, and user-defined multiplier
		rigidBod.AddForceAtPosition (fluidDragVecPosZ*2f, zpos_face_center);  // Apply force at face's center, in the direction opposite the face normal
		// TOP (posY):
		Vector3 pointVelPosY = rigidBod.GetPointVelocity (ypos_face_center);
		float velPosY = Vector3.Dot (up, pointVelPosY) * pointVelPosY.sqrMagnitude;   // get the proportion of the velocity vector in the direction of face's normal (0 - 1) times magnitude squared
		Vector3 fluidDragVecPosY = -up * velPosY * sa_y[segmentIndex] * viscosityDrag;  
		rigidBod.AddForceAtPosition (fluidDragVecPosY*2f, ypos_face_center);
		// RIGHT (posX):
		//Vector3 pointVelPosX = rigidBod.GetPointVelocity (xpos_face_center);
		//Vector3 fluidDragVecPosX = -right * Vector3.Dot (right, pointVelPosX) * sa_x[segmentIndex] * viscosityDrag;  
		//rigidBod.AddForceAtPosition (fluidDragVecPosX*2f, xpos_face_center);

	}
	
	public override void Tick() {  // Runs the mini-game for a single evaluation step.
		//Debug.Log ("Tick()");
		// THIS IS ALL PRE- PHYS-X!!! ::

		for(int w = 0; w < numberOfSegments; w++) {
			if(GOwormSegments[w].GetComponent<HingeJoint>() != null) {
				JointMotor motor = new JointMotor();
				motor.force = jointMotorForce;
				motor.targetVelocity = wormSegmentArray_MotorTarget[w][0] * jointMotorSpeed;
				GOwormSegments[w].GetComponent<HingeJoint>().motor = motor;
			}
			ApplyViscosityForces(GOwormSegments[w], w, viscosityDrag);
		}

		// FITNESS COMPONENTS!
		Vector3 avgPos = new Vector3(0f, 0f, 0f);
		for(int e = 0; e < numberOfSegments; e++) {
			avgPos += new Vector3(wormSegmentArray_PosX[e][0], wormSegmentArray_PosY[e][0], wormSegmentArray_PosZ[e][0]);
			fitEnergySpent[0] += Mathf.Abs (wormSegmentArray_MotorTarget[e][0])/(float)numberOfSegments;
		}
		avgPos /= (float)numberOfSegments;
		ArenaCameraController.arenaCameraControllerStatic.focusPosition = avgPos;
		Vector3 targetDirection = new Vector3(targetPosX[0] - avgPos.x, targetPosY[0] - avgPos.y, targetPosZ[0] - avgPos.z);
		float distToTarget = targetDirection.magnitude;
		fitDistToTarget[0] = distToTarget / maxScoreDistance;
		fitDistFromOrigin[0] += avgPos.magnitude / maxScoreDistance;

		targetDirX[0] = targetDirection.x;
		targetDirY[0] = targetDirection.y;
		targetDirZ[0] = targetDirection.z;

		if(distToTarget < targetRadius) {
			fitTimeToTarget[0] += 0f;
		}
		else {
			fitTimeToTarget[0] += 1f;
		}

		gameTicked = true;
	}

	public override void Reset() {
		//Debug.Log ("Reset()");
		float xOffset = armTotalLength[0] * 0.5f;
		wormSegmentArray_PosX[0][0] = 0f; // origin of arm at (0, 0);
		wormSegmentArray_PosY[0][0] = 0f;
		wormSegmentArray_PosZ[0][0] = 0f;

		Vector3 randDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
		randDir = (randDir * 2f) / randDir.magnitude;

		targetPosX[0] = randDir.x;
		targetPosY[0] = randDir.y;
		targetPosZ[0] = randDir.z;

		// Game Pieces!!!!! // maybe put into their own function eventually?
		for(int w = 0; w < numberOfSegments; w++) {
			wormSegmentArray_Length[w][0] = armTotalLength[0]/(float)numberOfSegments;
			wormSegmentArray_Angle[w][0] = 0f;
			wormSegmentArray_MotorTarget[w][0] = 0f;

			if(w != 0) { // if not the root segment:
				wormSegmentArray_PosX[w][0] = wormSegmentArray_PosX[w-1][0] + wormSegmentArray_Length[w-1][0]*0.5f + wormSegmentArray_Length[w][0]*0.5f;
			}
			wormSegmentArray_PosY[w][0] = 0f;
			wormSegmentArray_PosZ[w][0] = 0f;

			//Debug.Log ("Reset() segmentPos: " + new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]).ToString());
		
			//GOwormSegments[i].transform.localScale = new Vector3(wormSegmentArray_Length[i][0], wormSegmentThickness, wormSegmentThickness*2f);
			sa_x[w] = wormSegmentThickness * wormSegmentThickness*2f;  // Y * Z
			sa_y[w] = wormSegmentArray_Length[w][0] * wormSegmentThickness*2f;  // X * Z
			sa_z[w] = wormSegmentArray_Length[w][0] * wormSegmentThickness; // X * Y
		}

		// FITNESS COMPONENTS!:
		fitDistFromOrigin[0] = 0f;
		fitEnergySpent[0] = 0f;
		fitDistToTarget[0] = 0f;
		fitTimeToTarget[0] = 0f;

		gameInitialized = true;
		gameTicked = false;
		gameUpdatedFromPhysX = false;
		gameCurrentTimeStep = 0;  // reset to 0
	}

	public override void GameTimeStepCompleted() {
		gameTicked = false;
		gameUpdatedFromPhysX = false;
		gameCurrentTimeStep++;  // reset to 0
	}

	public override void UpdateGameStateFromPhysX() {
		//Debug.Log ("UpdateGameStateFromPhysX()");
		// PhysX simulation happened after miniGame Tick(), so before passing gameStateData to brain, need to update gameStateData from the rigidBodies
		// SO that the correct updated input values can be sent to the brain
		for(int w = 0; w < numberOfSegments; w++) {
			if(GOwormSegments[w].GetComponent<Rigidbody>() != null) {
				wormSegmentArray_PosX[w][0] = GOwormSegments[w].GetComponent<Rigidbody>().position.x;
				wormSegmentArray_PosY[w][0] = GOwormSegments[w].GetComponent<Rigidbody>().position.y;
				wormSegmentArray_PosZ[w][0] = GOwormSegments[w].GetComponent<Rigidbody>().position.z;
				//Debug.Log ("UpdateGameStateFromPhysX() rigidBodPos: " + new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]));

				if(w < (numberOfSegments - 1)) {  // if not the final 'Tail' segment:
					wormSegmentArray_Angle[w][0] = GOwormSegments[w].GetComponent<HingeJoint>().angle;
					// Motor force? think that should be a one-way path, set from gameData
				}
			}
		}

		gameUpdatedFromPhysX = true;
	}

	public override void BuildGamePieces() { // COMPONENTS ONLY RIGHT NOW!
		//Debug.Log ("Build GamePieces()");
		GOtargetSphere.AddComponent<GamePiecePhysXTestsBall>().InitGamePiece();
		GOtargetSphere.GetComponent<Rigidbody>().useGravity = false;
		GOtargetSphere.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);

		for(int w = 0; w < numberOfSegments; w++) {
			GOwormSegments[w].transform.localScale = new Vector3(wormSegmentArray_Length[w][0], wormSegmentThickness, wormSegmentThickness*2f);
			//GOwormSegments[w].transform.localPosition = new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]); // RE-EVALUATE!!!
			GOwormSegments[w].transform.position = new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]); // RE-EVALUATE!!!
			GOwormSegments[w].transform.localRotation = Quaternion.identity;
			Debug.Log ("BuildGamePieces() position: " + GOwormSegments[w].transform.position.ToString());

			GOwormSegments[w].AddComponent<GamePiecePhysXWormSegment>().InitGamePiece();
			GOwormSegments[w].GetComponent<Rigidbody>().useGravity = false;
			// Set starting position:
			//Vector3 newPos = new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]);
			//GOwormSegments[w].GetComponent<Rigidbody>().position = newPos;
			//Debug.Log ("Build GamePieces() newPos: " + newPos.ToString());
			//GOwormSegments[w].GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
			//GOwormSegments[w].GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
			if(w < (numberOfSegments - 1)) {  // if not the final 'Tail' segment:

			}
			if(w > 0) {  // if not the first root segment:
				HingeJoint hingeJoint = GOwormSegments[w-1].AddComponent<HingeJoint>();
				hingeJoint.autoConfigureConnectedAnchor = false;
				hingeJoint.connectedBody = GOwormSegments[w].GetComponent<Rigidbody>(); // connected bod of previous segment is THIS segment
				hingeJoint.anchor = new Vector3(0.5f, 0f, 0f);
				if(w % 2 == 0) {
					hingeJoint.axis = new Vector3(0f, 0f, 1f);
				}
				else {
					hingeJoint.axis = new Vector3(0f, 1f, 0f);
				}
				hingeJoint.connectedAnchor = new Vector3(-0.5f, 0f, 0f);
				JointLimits jointLimits = new JointLimits();
				jointLimits.max = armSegmentMaxBend;
				jointLimits.min = -armSegmentMaxBend;
				hingeJoint.limits = jointLimits;
				hingeJoint.useLimits = true;
				hingeJoint.useMotor = true;
				JointMotor motor = new JointMotor();
				motor.force = jointMotorForce;
				hingeJoint.motor = motor;

			}
			GOwormSegments[w].transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);
			Material segmentMaterial = new Material (Shader.Find("Diffuse"));
			GOwormSegments[w].GetComponent<Renderer>().material = segmentMaterial;
		}

		piecesBuilt = true;
	}
	
	public override void DeleteGamePieces() { // COMPONENTS ONLY RIGHT NOW!
		//Debug.Log ("DeleteGamePieces()");
		for(int w = 0; w < numberOfSegments; w++) {
			//GameObject.DestroyObject(GOwormSegments[w]);  // destroy entire GameObject

			if(GOwormSegments[w].GetComponent<Rigidbody>() != null) {
				GOwormSegments[w].GetComponent<Rigidbody>().isKinematic = true;
				if(GOwormSegments[w].GetComponent<HingeJoint>() != null) {
					GameObject.DestroyImmediate(GOwormSegments[w].GetComponent<HingeJoint>());
				}
				GameObject.Destroy(GOwormSegments[w].GetComponent<GamePiecePhysXWormSegment>());
				GameObject.Destroy(GOwormSegments[w].GetComponent<Rigidbody>());
				//GOwormSegments[w].GetComponent<Rigidbody>().isKinematic = false;
			}	
			GameObject.Destroy(GOtargetSphere.GetComponent<GamePiecePhysXTestsBall>());
			GameObject.Destroy(GOtargetSphere.GetComponent<Rigidbody>());
		}
		piecesBuilt = false;
	}

	public override void InstantiateGamePieces() {
		GOtargetSphere = new GameObject("GOtargetSphere");
		GOtargetSphere.transform.localScale = new Vector3(targetRadius, targetRadius, targetRadius);
		GOtargetSphere.transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);
		
		for(int i = 0; i < numberOfSegments; i++) {
			string name = "GOwormSegment" + i.ToString();
			GOwormSegments[i] = new GameObject(name);

			GOwormSegments[i].transform.SetParent(ArenaGroup.arenaGroupStatic.gameObject.transform);
		}
	}
	
	public override void UninstantiateGamePieces() {
		
	}
	
	public override void BuildGamePieceComponents() {
		//Debug.Log ("BuildGamePieceComponents()");
		GOtargetSphere.AddComponent<GamePiecePhysXTestsBall>().InitGamePiece();
		GOtargetSphere.AddComponent<Rigidbody>().useGravity = false;
		//GOtargetSphere.GetComponent<Rigidbody>()

		Material segmentMaterial = new Material (Shader.Find("Diffuse"));
		for(int w = 0; w < numberOfSegments; w++) {
			GOwormSegments[w].AddComponent<GamePiecePhysXWormSegment>().InitGamePiece();
			GOwormSegments[w].GetComponent<Renderer>().material = segmentMaterial;
			GOwormSegments[w].AddComponent<Rigidbody>().useGravity = false;

			if(w < (numberOfSegments - 1)) {  // if not the final 'Tail' segment:
				
			}
			if(w > 0) {  // if not the first root segment:
				HingeJoint hingeJoint = GOwormSegments[w-1].AddComponent<HingeJoint>();
				hingeJoint.autoConfigureConnectedAnchor = false;
				hingeJoint.connectedBody = GOwormSegments[w].GetComponent<Rigidbody>(); // connected bod of previous segment is THIS segment
				hingeJoint.anchor = new Vector3(0.5f, 0f, 0f);
				if(w % 2 == 0) {
					hingeJoint.axis = new Vector3(0f, 0f, 1f);
				}
				else {
					hingeJoint.axis = new Vector3(0f, 1f, 0f);
				}
				hingeJoint.connectedAnchor = new Vector3(-0.5f, 0f, 0f);
				JointLimits jointLimits = new JointLimits();
				jointLimits.max = armSegmentMaxBend;
				jointLimits.min = -armSegmentMaxBend;
				hingeJoint.limits = jointLimits;
				hingeJoint.useLimits = true;
				hingeJoint.useMotor = true;
				JointMotor motor = new JointMotor();
				motor.force = jointMotorForce;
				hingeJoint.motor = motor;				
			}

		}		
		piecesBuilt = true;
	}
	
	public override void DestroyGamePieceComponents() {
		//Debug.Log ("DestroyGamePieceComponents()");
		for(int w = 0; w < numberOfSegments; w++) {
			//GameObject.DestroyObject(GOwormSegments[w]);  // destroy entire GameObject
			
			if(GOwormSegments[w].GetComponent<Rigidbody>() != null) {
				GOwormSegments[w].GetComponent<Rigidbody>().isKinematic = true;
				if(GOwormSegments[w].GetComponent<HingeJoint>() != null) {
					GameObject.DestroyImmediate(GOwormSegments[w].GetComponent<HingeJoint>());
				}
				GameObject.DestroyImmediate(GOwormSegments[w].GetComponent<GamePiecePhysXWormSegment>());
				GameObject.DestroyImmediate(GOwormSegments[w].GetComponent<Rigidbody>());
				//GOwormSegments[w].GetComponent<Rigidbody>().isKinematic = false;
			}	
			GameObject.DestroyImmediate(GOtargetSphere.GetComponent<GamePiecePhysXTestsBall>());
			GameObject.DestroyImmediate(GOtargetSphere.GetComponent<Rigidbody>());
		}
		piecesBuilt = false;
	}

	public override void SetGamePieceTransformsFromData() {
		for(int w = 0; w < numberOfSegments; w++) {
			GOwormSegments[w].transform.localScale = new Vector3(wormSegmentArray_Length[w][0], wormSegmentThickness, wormSegmentThickness*2f);
			//GOwormSegments[w].transform.localPosition = new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]); // RE-EVALUATE!!!
			GOwormSegments[w].transform.position = new Vector3(wormSegmentArray_PosX[w][0], wormSegmentArray_PosY[w][0], wormSegmentArray_PosZ[w][0]); // RE-EVALUATE!!!
			GOwormSegments[w].transform.localRotation = Quaternion.identity;
			//Debug.Log ("SetGamePieceTransformsFromData() position: " + GOwormSegments[w].transform.position.ToString());
		}

		GOtargetSphere.transform.localPosition = new Vector3(targetPosX[0], targetPosY[0], targetPosZ[0]);
		GOtargetSphere.transform.localScale = new Vector3(targetRadius, targetRadius, targetRadius);
	}
	
	public override void VisualizeGameState() {
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosX", (ownPosX[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosY", (ownPosY[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_OwnPosZ", (ownPosZ[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosX", (targetPosX[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosY", (targetPosY[0]));
		//GOboundaryWalls.GetComponent<Renderer>().material.SetFloat("_TargetPosZ", (targetPosZ[0]));
	}
}
