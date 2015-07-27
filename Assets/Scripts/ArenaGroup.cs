using UnityEngine;
using System.Collections;

public class ArenaGroup : MonoBehaviour {

	public static ArenaGroup arenaGroupStatic;



	void Awake () {
		//Debug.Log ("ArenaGroup AWAKE()");
		arenaGroupStatic = this;

		ArenaViewOff();
		this.enabled = false;
	}



	public void ArenaViewOn() {
		this.gameObject.SetActive(true);
		this.enabled = true;
	}

	public void ArenaViewOff() {
		this.gameObject.SetActive(false);
		this.enabled = false;
	}
}
