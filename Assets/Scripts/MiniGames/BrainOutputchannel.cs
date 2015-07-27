using UnityEngine;
using System.Collections;

public class BrainOutputChannel {
	
	public float[] channelValue; // See if I can figure out how to make this dataType generic
	public bool on; // This means on/off selection choice is stored in the GameMoveToTarget1D instance
	public string channelName;
	
	// Constructor Methods!
	public BrainOutputChannel(ref float[] value, bool active, string name) {
		//channelValue = new float[value.Length];
		channelValue = value;
		on = active;
		channelName = name;
	}
}
