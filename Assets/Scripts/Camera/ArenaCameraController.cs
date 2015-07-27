using UnityEngine;
using System.Collections;

public class ArenaCameraController : MonoBehaviour {

	public static ArenaCameraController arenaCameraControllerStatic;
	public Vector3 focusPosition = new Vector3(0f, 0f, 0f);
	public Vector3 targetCameraPosition = new Vector3(0f, 0f, -2.7f);
	public Vector3 targetCameraDirection;
	public float targetCameraDistance = 3f;
	public float cameraSpeed = 0.1f;
	private Vector3 cameraVelocity = new Vector3(0f, 0f, 0f);

	private Vector3 aimDirection; 

	void Awake () {
		aimDirection = new Vector3(0f, 0f, 0f);
		arenaCameraControllerStatic = this;
		targetCameraDirection = (targetCameraPosition - focusPosition);
		targetCameraDirection /= targetCameraDirection.magnitude; // normalize to unit Vector
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(focusPosition, Vector3.up);
		aimDirection = this.transform.forward;

		targetCameraPosition = focusPosition + targetCameraDirection * targetCameraDistance;

		float distanceToTargetPos = new Vector3(targetCameraPosition.x - this.transform.position.x, targetCameraPosition.y - this.transform.position.y, targetCameraPosition.z - this.transform.position.z).magnitude;
		float distanceToFocusPos = new Vector3(focusPosition.x - this.transform.position.x, focusPosition.y - this.transform.position.y, focusPosition.z - this.transform.position.z).magnitude;
		//Vector3 moveInOut = new Vector3();
		//moveInOut = (aimDirection * (distanceToFocusPos - targetCameraDistance)) * cameraSpeed * distanceToTargetPos;
		//targetCameraPosition += moveInOut;
		cameraVelocity = (targetCameraPosition - this.transform.position);
		if(cameraVelocity.magnitude != 0f) {
			cameraVelocity /= cameraVelocity.magnitude;
		}
		cameraVelocity *= Mathf.Min(cameraSpeed, Mathf.Abs (distanceToTargetPos) * cameraSpeed);
		//Debug.Log (" distanceToTargetPos: " + distanceToTargetPos.ToString() + ", distanceToFocusPos: " + distanceToFocusPos.ToString() + ", moveInOut: " + moveInOut.ToString() + ", targetCamPos: " + targetCameraPosition.ToString());
		Vector3 newPos = this.transform.position + cameraVelocity;
		this.transform.position = newPos;
	}

	public void PanUpDown(float amount) {
		Vector3 moveDirection = this.transform.up * amount * cameraSpeed;
		//targetCameraPosition += aimDirection * (targetCameraPosition - this.transform.position);
		targetCameraDirection += moveDirection;
		if(targetCameraDirection.magnitude != 0f) {
			targetCameraDirection /= targetCameraDirection.magnitude; // normalize to unit Vector
		}
	}

	public void PanLeftRight(float amount) {
		Vector3 moveDirection = this.transform.right * amount * cameraSpeed;
		targetCameraDirection += moveDirection;
		if(targetCameraDirection.magnitude != 0f) {
			targetCameraDirection /= targetCameraDirection.magnitude; // normalize to unit Vector
		}
		//targetCameraPosition += aimDirection * (targetCameraPosition - this.transform.position);
	}
}
