using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainerArenaUI : MonoBehaviour {

	public TrainerModuleUI trainerModuleScript;
	public Image bgImage;

	public bool isRotating = false;
	private float RotationSpeed = 250f;

	void Update() {
		if(isRotating) {
			//ArenaGroup.arenaGroupStatic.gameObject.transform.Rotate((Input.GetAxis("Mouse Y") * -RotationSpeed * Time.deltaTime), (Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime), 0, Space.World);
		}
	}

	public void ArenaUIBeginDrag() {
		//Debug.Log ("TrainerArenaUI + ArenaUIBeginDrag; MouseX: " + Input.GetAxis("Mouse X").ToString() + ", MouseY: " + Input.GetAxis("Mouse Y").ToString());
		//isRotating = true;
	}

	public void ArenaUIEndDrag() {
		//Debug.Log ("TrainerArenaUI + ArenaUIEndDrag; MouseX: " + Input.GetAxis("Mouse X").ToString() + ", MouseY: " + Input.GetAxis("Mouse Y").ToString());
		//isRotating = false;
	}

	public void ArenaUIDrag() {
		//Debug.Log ("TrainerArenaUI + ArenaUIDrag; MouseX: " + Input.GetAxis("Mouse X").ToString() + ", MouseY: " + Input.GetAxis("Mouse Y").ToString());
		ArenaCameraController.arenaCameraControllerStatic.PanLeftRight(Input.GetAxis("Mouse X"));
		ArenaCameraController.arenaCameraControllerStatic.PanUpDown(Input.GetAxis("Mouse Y"));
		//isRotating = true;
	}
}
