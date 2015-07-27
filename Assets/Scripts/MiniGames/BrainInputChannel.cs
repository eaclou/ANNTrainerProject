using UnityEngine;
using System.Collections;

public class BrainInputChannel {

	public float[] channelValue; // See if I can figure out how to make this dataType generic
	public bool on; // This means on/off selection choice is stored in the GameMoveToTarget1D instance
	public string channelName;

	// Constructor Methods!
	public BrainInputChannel(ref float[] value, bool active, string name) {
		//channelValue = new float[value.Length];
		channelValue = value;
		on = active;
		channelName = name;
	}

	public void CopyInputChannelsList(BrainInputChannel target) {		// Maybe update this by making it a function of BrainInputChannel that takes an instance of itself
		// Check if both are the same length?
		/*int numInputs = source.Count;
		if(numInputs == target.Count) {
			for(int i = 0; i < numInputs; i++) {
				string newName = "";
				newName = source[i].channelName;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].channelName = newName;
				float newValue = 0f;
				newValue = source[i].channelValue;  // Make sure these are allocating new memory and will be copies, not references!
				target[i].channelValue = newValue;
				bool newOn = false;
				newOn = source[i].on;   // Make sure these are allocating new memory and will be copies, not references!
				target[i].on = newOn;
			}
		}
		else {
			DebugBot.DebugFunctionCall("TMiniGameUI; CopyInputChannelsList(); Arrays of Different Length!", debugFunctionCalls);
		}*/
	}
}
