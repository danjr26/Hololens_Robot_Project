using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UserTransformableRecordable : UserTransformable {
	protected override void BuildMenu() {
		if (!isMenuOpen) return;
		if (UserTransformManager.instance.isRecording
			&& UserTransformManager.instance.recordEnvironment.target == this) {

			// we are recording
			menu.GetComponent<ObjectMenu>().AddButton(
				"Take Snapshot",
				delegate () {
					isMenuOpen = false;
					UserTransformManager.instance.recordEnvironment.CreateSnapshot(gameObject.transform);
					gameObject.transform.Translate(Camera.main.transform.right * 0.01f);
				}
			);
			menu.GetComponent<ObjectMenu>().AddButton(
				"Run Recording",
				delegate () {
					isMenuOpen = false;
					UserTransformManager.instance.recordEnvironment.recording.Execute(transform);
				}
			);
			menu.GetComponent<ObjectMenu>().AddButton(
				"Stop Recording",
				delegate () {
					isMenuOpen = false;
					UserTransformManager.instance.StopRecording();
				}
			);
			menu.GetComponent<ObjectMenu>().AddButton(
				"Save Recording",
				delegate () {
					isMenuOpen = false;
					UserTransformManager.instance.SaveRecording("test.hrpsav");
				}
			);
		}
		else if (UserTransformManager.instance.isLive
			&& UserTransformManager.instance.liveEnvironment.target == this) {
			// live session
			menu.GetComponent<ObjectMenu>().AddButton(
				"Stop Live Session",
				delegate () {
					isMenuOpen = false;
					UserTransformManager.instance.StopLiveSession();
				}
			);
		}
		else {
			// nobody is recording
			menu.GetComponent<ObjectMenu>().AddButton(
				"New Recording",
				delegate () {
					isMenuOpen = false;
					UserTransformManager.instance.StartNewRecording(this);
					isMenuOpen = true;
				}
			);

			menu.GetComponent<ObjectMenu>().AddButton(
				"Load Recording",
				delegate () {
					try {
						isMenuOpen = false;
						UserTransformManager.instance.LoadRecording(this, "test.hrpsav");
					}
					catch (Exception e) {
						OutputText.instance.text = e.Message + "\n" + e.StackTrace;
					}
				}
			);

			menu.GetComponent<ObjectMenu>().AddButton(
				"New Live Session",
				delegate () {
					try {
						isMenuOpen = false;
						UserTransformManager.instance.StartLiveSession(this);
					} catch(Exception e) {
						OutputText.instance.text = e.Message + "\n" + e.StackTrace;
					}
				}
			);
		}
	}
}
