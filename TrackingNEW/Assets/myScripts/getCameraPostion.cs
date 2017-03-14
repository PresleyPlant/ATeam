﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Vuforia;

public class getCameraPostion : MonoBehaviour{
	/*VARIABLES*/
	private float cameraHeight = 200;
	private float CameraSpeed = 15f;
	private bool isTracking = false;

	//VRCamera variables
	private Camera VRCamera;
	private Vector3 VRCameraPos;
	private Vector3 VRCameraPrevPos;
	private Vector3 newCameraPos;
	private Quaternion VRCameraPrevRot;
	private Quaternion VRCameraRot;

	//GameCamera variables
	public Camera sceneCamera;
	private Vector3 cameraPos;
	private Quaternion cameraRot;

	// Use this for initialization
	void Start () {
		//Set camera pos/rot first time
		VRCamera = Camera.main; 
		VRCameraPrevPos = VRCamera.transform.position;
		VRCameraPrevRot = VRCamera.transform.rotation;

		//Disable the VRCamera to show the sceneCamera feed
		VRCamera.enabled = false;
		sceneCamera.enabled = true;
	
		
	}

	// Update is called once per frame
	void Update () {
		//Get the tracking state from the ImageTarget component
		isTracking = GameObject.Find("ImageTarget").GetComponent<trackingState>().isTracking;
			
		/*******CAMERA MOVEMENT*******/
		//Get VRCamera position
		VRCameraPos = VRCamera.transform.position;
		//Get VRCamera Rotation
		VRCameraRot = VRCamera.transform.rotation;

		print ("Local: "+sceneCamera.transform.localRotation);
		print ("World: "+sceneCamera.transform.rotation);
		print ("---------------------");

		if (!isTracking) {
			VRCameraRot = VRCameraPrevRot;
			
		} else {
			/*******POSITION*******/
			//Checks if the distance moved is bigger than a value to stop jittering
			if (checkDistance (VRCameraPrevPos, VRCameraPos)) {//KANSKE INTE BEHÖVS
				//Move with the CameraSpeed and set height
				newCameraPos = new Vector3 (VRCameraPos.x * CameraSpeed, cameraHeight, VRCameraPos.z * -CameraSpeed);
				//Interpolate between old and new position
				cameraPos = sceneCamera.transform.position;
				cameraPos.Set (cameraPos.x, cameraHeight, cameraPos.z);
				sceneCamera.transform.position = Vector3.Lerp (cameraPos, newCameraPos, Time.deltaTime * 3f);
			}
			//Update previous position
			VRCameraPrevPos = VRCameraPos;

			/*******ROTATION*******/
			//Checks if the rotation is bigger than a value to stop jittering
			if (checkAngle (VRCameraPrevRot.eulerAngles, VRCameraRot.eulerAngles)) {//KANSKE INTE BEHÖVS
				//Convert camera rotaion to degrees
				Vector3 VRCameraRotEuler = VRCameraRot.eulerAngles;
				//Remove x & z rotation
				VRCameraRotEuler.Set(90f, VRCameraRotEuler.y, 0f);
				Quaternion newRotaion = Quaternion.Euler (VRCameraRotEuler);

				Quaternion currentRot = sceneCamera.transform.rotation;

				sceneCamera.transform.rotation = Quaternion.Slerp (currentRot, newRotaion, Time.deltaTime * 5f);
			}

			//Update previous rotation
			VRCameraPrevRot = VRCameraRot;
		}
	}

	/******FUNCTIONS******/

	//Function checking if the difference between two angles are greater than a value
	bool checkAngle(Vector3 prevAngle, Vector3 newAngle){
		/*Variables*/
		float angleLimit = 0f;
		//Calculate the angle of rotation
		float deltaAngle = Mathf.Abs (prevAngle.y - newAngle.y);
	
		//If angle is larger than limit return true
		if (deltaAngle > angleLimit) {
			return true;
		}

		return false;
	}

	//Function checking distance between two positions
	bool checkDistance(Vector3 prevPos, Vector3 newPos){
		/*Variables*/
		float distLimit = 0.00f;
		//Calculate distance between two positions
		float deltaDist = Mathf.Abs(Mathf.Pow((prevPos.x - newPos.x),2) + Mathf.Pow((prevPos.z - newPos.z),2));


		if ( deltaDist > distLimit) {
			return true;
		}

		return false;

	}
		
}
