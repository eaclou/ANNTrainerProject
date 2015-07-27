using UnityEngine;
using System.Collections;

public class TestTrainer : MonoBehaviour {

	public TestGameManager testGameManager;
	public TestBrain testBrain;
	public int numInputNodes = 2;
	public int numOutputNodes = 1;


	// Use this for initialization
	void Start () {
		Debug.Log ("TestTrainer: Start() ");

		testGameManager = new TestGameManager(this);
		testBrain = new TestBrain();

		//testGameManager.brainInput[0] -= 5f;
		//testGameManager.testGameInstance.inputChannelsList[0].GetValue();





		testBrain.TestBrainFunction(testGameManager.brainInput, ref testGameManager.brainOutput);
		testGameManager.testGameInstance.Tick ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
