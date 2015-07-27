using UnityEngine;
using System.Collections;

public class testPhysicsTimeController : MonoBehaviour {

	public testPhysicsUI testUI;


	private bool isPlaying = false;

	private float playbackSpeed = 1.0f;
	private float customTimeScale = 0.0f;
	private float minPlaybackSpeed = 0.01f;
	private float maxPlaybackSpeed = 100f;
	private float speedIncrement = 0.8f;

	private float totalSimulatedTimeFixedUpdate = 0.0f;
	private float totalSimulatedTimeUpdate = 0.0f;
	private float totalUnityTimeFixedUpdate = 0.0f;
	private float totalUnityTimeUpdate = 0.0f;
	private float totalUnityTimeUnscaledUpdate = 0.0f;
	private float currentUnityTime;
	private float currentUnityTimeUnscaled;
	private float currentUnityTimeFixed;
	private float currentUnityTimeScale;

	//Physics.solverIterationCount;
	public Rigidbody rigidBody;

	private int totalFixedUpdateCalls = 0;
	private int totalUpdateUpdateCalls = 0;

	void Awake() {
		Time.timeScale = customTimeScale;
	}

	// Use this for initialization
	void Start () {
		testUI.textPlaybackSpeed.text = "Playback Speed: " + playbackSpeed.ToString();

		//rigidBody.Sleep();
	}
	
	// Update is called once per frame
	void Update () {
		totalSimulatedTimeUpdate += playbackSpeed;
		totalUnityTimeUpdate += Time.deltaTime;
		totalUnityTimeUnscaledUpdate += Time.unscaledDeltaTime;
		currentUnityTime = Time.time;
		currentUnityTimeUnscaled = Time.unscaledTime;
		currentUnityTimeScale = Time.timeScale;

		totalUpdateUpdateCalls++;
		DisplayTextUpdate();
	}

	void FixedUpdate() {
		totalSimulatedTimeFixedUpdate += playbackSpeed;
		totalUnityTimeFixedUpdate += Time.fixedDeltaTime;
		currentUnityTimeFixed = Time.fixedTime;

		totalFixedUpdateCalls++;
		DisplayTextFixedUpdate();
	}

	private void DisplayTextUpdate() {
		testUI.textDisplayText01.text = "Time.deltaTime: " + Time.deltaTime.ToString();
		testUI.textDisplayText02.text = "totalUnityTimeUpdate: " + totalUnityTimeUpdate.ToString();
		testUI.textDisplayText03.text = "totalUnityTimeUnscaledUpdate: " + totalUnityTimeUnscaledUpdate.ToString();
		testUI.textDisplayText04.text = "currentUnityTime: " + currentUnityTime.ToString();
		testUI.textDisplayText05.text = "totalUpdateUpdateCalls: " + totalUpdateUpdateCalls.ToString();
		testUI.textDisplayText06.text = "currentUnityTimeScale: " + currentUnityTimeScale.ToString();
	}

	private void DisplayTextFixedUpdate() {
		testUI.textDisplayText07.text = "Time.fixedDeltaTime: " + Time.fixedDeltaTime.ToString();
		testUI.textDisplayText08.text = "totalUnityTimeFixedUpdate: " + totalUnityTimeFixedUpdate.ToString();
		testUI.textDisplayText09.text = "totalFixedUpdateCalls: " + totalFixedUpdateCalls.ToString();
	}

	public void ClickPlayPause() {
		isPlaying = !isPlaying;
		if(isPlaying) {
			customTimeScale = playbackSpeed;
			testUI.textButtonPlayPause.text = "PLAYING!";
		}
		else {
			customTimeScale = 0.0f;
			testUI.textButtonPlayPause.text = "PAUSED!";
		}
		Time.timeScale = customTimeScale;
		//Time.fixedDeltaTime = customTimeScale * 0.02f;
	}

	public void ClickFaster() {
		playbackSpeed *= (1f/speedIncrement);
		if(playbackSpeed > maxPlaybackSpeed) { playbackSpeed = maxPlaybackSpeed; }
		testUI.textPlaybackSpeed.text = "Playback Speed: " + playbackSpeed.ToString();
		customTimeScale = playbackSpeed;
		if(isPlaying) { 
			Time.timeScale = customTimeScale; 
			//Time.fixedDeltaTime = customTimeScale * 0.02f;
		}
	}

	public void ClickSlower() {
		playbackSpeed *= speedIncrement;
		if(playbackSpeed < minPlaybackSpeed) { playbackSpeed = minPlaybackSpeed; }
		testUI.textPlaybackSpeed.text = "Playback Speed: " + playbackSpeed.ToString();
		customTimeScale = playbackSpeed;
		if(isPlaying) { 
			Time.timeScale = customTimeScale; 
			//Time.fixedDeltaTime = customTimeScale * 0.02f;
		}
	}

	public void ClickRealtime() {
		playbackSpeed = 1.0f;
		testUI.textPlaybackSpeed.text = "Playback Speed: " + playbackSpeed.ToString();
		customTimeScale = playbackSpeed;
		if(isPlaying) { 
			Time.timeScale = customTimeScale; 
			//Time.fixedDeltaTime = customTimeScale * 0.02f;
		}
	}
}
