using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestGame {
	
	public List<TestChannel> inputChannelsList;
	public List<TestChannel> outputChannelsList;
	
	// Game State Data:  // CHANGE THESE TO BETTER/GENERIC DATA TYPES LATER!!!!!!!!!!!!!!!!!
	//TestGeneric<float> ownPosX = new TestGeneric<float>(9f);
	private float[] ownPosArray = new float[1];

	private float[] ownPosX = new float[1];
	private float[] targetPosX = new float[1];
	private float[] ownVelX = new float[1];
	private float[] targetVelX = new float[1];
	
	// Game Settings:
	public float boundaryPosX;
	public float boundaryNegX;
	
	// Constructor Method:
	public TestGame() {
		DebugBot.DebugFunctionCall("TestGame; Constructor();", true);
		ownPosX[0] = 1f;
		targetPosX[0] = 2f;
		ownVelX[0] = 3f;
		targetVelX[0] = 4f;
		// Brain Inputs!:
		inputChannelsList = new List<TestChannel>();
		TestChannel BIC_ownPosX = new TestChannel(ref ownPosX, true, "ownPosX");
		inputChannelsList.Add (BIC_ownPosX); // 0
		TestChannel BIC_targetPosX = new TestChannel(ref targetPosX, true, "targetPosX");
		inputChannelsList.Add (BIC_targetPosX); // 1
		TestChannel BIC_ownVelX = new TestChannel(ref ownVelX, false, "ownVelX");
		inputChannelsList.Add (BIC_ownVelX); // 2
		TestChannel BIC_targetVelX = new TestChannel(ref targetVelX, false, "targetVelX");
		inputChannelsList.Add (BIC_targetVelX); // 3

		// Brain Outputs!:		
		outputChannelsList = new List<TestChannel>();
		TestChannel BOC_ownVelX = new TestChannel(ref ownVelX, true, "ownVelX");
		outputChannelsList.Add (BOC_ownVelX); // 0
	}
	
	// Primary Function
	public void Tick() {  // Runs the mini-game for a single evaluation step.
		//inputChannelsList[0].channelValue += 222f;
		//ownPosX -= 57.02417f;
		//inputChannelsList[0].SetValue(222f);

		DebugBot.DebugFunctionCall("TestGame; Tick(); ownPosX= " + ownPosX[0].ToString () + ", targPosX= " + targetPosX[0].ToString() + ", ownvelX= " + ownVelX[0].ToString () + ", targetVelX= " + targetVelX[0].ToString() + ", ", true);
	}
}
