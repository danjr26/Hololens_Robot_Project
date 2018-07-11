using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CalibrationToken : UserTransformable {
	public static CalibrationToken instance;
	Vector3[] points = {
		new Vector3(0.0f, 0.0f, -0.5f),
		new Vector3(0.5f, 0.0f, 0.0f),
		new Vector3(0.0f, 0.0f, 0.5f),
		new Vector3(-0.5f, 0.0f, 0.0f)
	};

	Vector3[] confirmedPoints = new Vector3[4];

	Transform conversion;

	uint pointIndex;

	private void Awake() {
		instance = this;
	}

	void ConfirmPoint() {
		try {
			confirmedPoints[pointIndex] = transform.position;
			if (pointIndex < 3) {
				if (RobotInterface.instance.isConnected) {
					RobotInterface.instance.Move(new RobotInterface.MoveCommand(
						points[pointIndex], Quaternion.LookRotation(points[pointIndex])
					));
				}
				UserTransformManager.instance.transformMode = UserTransformManager.TransformMode.translate;
			} else {
				CalculateConversion();
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<MeshCollider>().enabled = false;
			}
			pointIndex++;
		}
		catch (Exception e) {
			OutputText.instance.text = e.Message + "\n" + e.StackTrace;
		}
	}

	void CalculateConversion() {
		Vector3 center = new Vector3(0, 0, 0);
		Quaternion[] rotations = new Quaternion[4];

		for (int i = 0; i < 4; i++) {
			center += confirmedPoints[i];
			rotations[i] = Quaternion.FromToRotation(points[i], confirmedPoints[i]);  
		}
		center /= 4;

		transform.parent.SetPositionAndRotation(
			center,
			Quaternion.Slerp(
				Quaternion.Slerp(rotations[0], rotations[2], 0.5f),
				Quaternion.Slerp(rotations[1], rotations[3], 0.5f), 
				0.5f
			)
		);
	}

	public void BeginCalibration() {
		try {
			pointIndex = 0;

			if (RobotInterface.instance.isConnected) {
				RobotInterface.instance.Move(new RobotInterface.MoveCommand(
					points[pointIndex], Quaternion.LookRotation(points[pointIndex])
				));
			}

			gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.4f;

			gameObject.GetComponent<MeshRenderer>().enabled = true;
			gameObject.GetComponent<MeshCollider>().enabled = true;
		} catch(Exception e) {
			OutputText.instance.text = e.Message + "\n" + e.StackTrace;
		}
	}

	public void CancelCalibration() {
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		gameObject.GetComponent<MeshCollider>().enabled = false;
	}

	protected override void BuildMenu() {
		if (!isMenuOpen) return;
		menu.GetComponent<ObjectMenu>().AddButton(
				"Confirm",
				delegate () {
					try {
						isMenuOpen = false;
						ConfirmPoint();
					} catch (Exception e) {
						OutputText.instance.text = e.Message + "\n" + e.StackTrace;
					}
				}
			);
		menu.GetComponent<ObjectMenu>().AddButton(
				"Cancel",
				delegate () {					
					isMenuOpen = false;
					CancelCalibration();
				}
			);
	}
}
