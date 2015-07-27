using UnityEngine;
using System.Collections;

public static class DebugBot {

	// This is a custom class to help with custom debug logs

	public static void DebugFunctionCall(string debugText, bool debugOn) {
		if(debugOn) {
			Debug.Log (debugText);
		}
	}
}
