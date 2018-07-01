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

		menu.GetComponent<ObjectMenu>().AddButton(
				"Calibrate",
				delegate () {
					CalibrationToken.instance.BeginCalibration();
					isMenuOpen = false;
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
