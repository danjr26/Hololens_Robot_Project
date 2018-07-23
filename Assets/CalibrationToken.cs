using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CalibrationToken : UserTransformable {
	public static CalibrationToken instance;

	Vector3[] points = {
		new Vector3(0.0f, 0.0f, 0.5f),
		new Vector3(0.0f, 0.0f, -0.5f),
	};

	Vector3[] confirmedPoints = new Vector3[2];

	uint pointIndex = 0;

	private void Awake() {
		instance = this;
	}

	void ConfirmPoint() {
		try {
			confirmedPoints[pointIndex] = transform.position;
			pointIndex++;
			if (pointIndex < 2) {
				if (RobotInterface.instance.isConnectedToCommand) {

					RobotInterface.instance.QueueMove(new RobotInterface.MoveJointsCommand(
					new float[] { 0, -Mathf.PI / 2.0f, 0.01f, -Mathf.PI / 2.0f, 0, 0 }
				));

					RobotInterface.instance.QueueMove(new RobotInterface.MoveToPoseCommand(
						points[pointIndex], Quaternion.identity
					));
				}
				UserTransformManager.instance.transformMode = UserTransformManager.TransformMode.translate;
			} else {
				RobotInterface.instance.QueueMove(new RobotInterface.MoveJointsCommand(
					new float[] { 0, -Mathf.PI / 2.0f, 0.01f, -Mathf.PI / 2.0f, 0, 0 }
				));
				CalculateConversion();
				GetComponent<MeshRenderer>().enabled = false;
				GetComponent<MeshCollider>().enabled = false;
			}
		}
		catch (Exception e) {
			OutputText.instance.text = e.Message + "\n" + e.StackTrace;
		}
	}

	void CalculateConversion() {
		Vector3 center = new Vector3(0, 0, 0);

		for (int i = 0; i < 2; i++) {
			center += confirmedPoints[i];
			confirmedPoints[i].y = 0.0f;
		}
		center /= 2;

		Quaternion rotation = Quaternion.FromToRotation(points[0] - points[1], confirmedPoints[0] - confirmedPoints[1]);

		transform.parent.SetPositionAndRotation(center, rotation);
	}

	public void BeginCalibration() {
		try {
			pointIndex = 0;

			if (RobotInterface.instance.isConnectedToCommand) {

				RobotInterface.instance.QueueMove(new RobotInterface.MoveJointsCommand(
					new float[] { 0, -Mathf.PI / 2.0f, 0.01f, -Mathf.PI / 2.0f, 0, 0 }
				));

				RobotInterface.instance.QueueMove(new RobotInterface.MoveToPoseCommand(
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
