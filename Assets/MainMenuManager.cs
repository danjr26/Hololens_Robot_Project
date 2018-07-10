using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;

public class MainMenuManager : MenuOwner {
	void Update () {
		if(HoloInputManager.instance.startHoldThisFrame &&
			UserTransformManager.instance.transformMode == UserTransformManager.TransformMode.none && 
			GazeCursor.instance.onWhat == null && !isMenuOpen) {

			gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
			isMenuOpen = true;
		}
	}

	protected override void BuildMenu() {
		if (!isMenuOpen) return;
		/*menu.GetComponent<ObjectMenu>().AddButton(
				"Set New Origin",
				delegate () {
					UserTransformableAnchor.instance.BeginPlace();
					isMenuOpen = false;
				}
			);*/


		if (RobotInterface.instance.isConnected) {
			menu.GetComponent<ObjectMenu>().AddButton(
					"Disconnect From IP",
					delegate () {
						isMenuOpen = false;
						RobotInterface.instance.EndConnection();
					}
				);
		}
		else {
			menu.GetComponent<ObjectMenu>().AddButton(
					"Connect Over IP",
					delegate () {
						isMenuOpen = false;
						IpConfigurator.instance.Open();
					}
				);
		}

		menu.GetComponent<ObjectMenu>().AddButton(
				"Calibrate",
				delegate () {
					isMenuOpen = false;
					CalibrationToken.instance.BeginCalibration();
				}
			);
		menu.GetComponent<ObjectMenu>().AddButton(
				"Quit",
				delegate () {
					Application.Quit();
				}
			);
	}
}
