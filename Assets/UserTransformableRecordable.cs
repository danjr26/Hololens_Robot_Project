﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UserTransformableRecordable : UserTransformable {
	protected override void BuildMenu() {
		if (!isMenuOpen) return;
		if (UserTransformManager.instance.isRecording) {
			if (UserTransformManager.instance.recordEnvironment.target == this) {
				// we are recording
				menu.GetComponent<ObjectMenu>().AddButton(
					"Take Snapshot",
					delegate () {
						UserTransformManager.instance.recordEnvironment.CreateSnapshot(gameObject.transform);
						gameObject.transform.Translate(Camera.main.transform.right * 0.01f);
						isMenuOpen = false;
					}
				);
				menu.GetComponent<ObjectMenu>().AddButton(
					"Run Recording",
					delegate () {
						UserTransformManager.instance.recordEnvironment.recording.Execute(transform);
						isMenuOpen = false;
					}
				);
				menu.GetComponent<ObjectMenu>().AddButton(
					"Stop Recording",
					delegate () {
						UserTransformManager.instance.StopRecording();
						isMenuOpen = false;
					}
				);
				menu.GetComponent<ObjectMenu>().AddButton(
					"Save Recording",
					delegate () {
						UserTransformManager.instance.SaveRecording("test.hrpsav");
						isMenuOpen = false;
					}
				);
			}
			else {
				// someone else is recording
				menu.GetComponent<ObjectMenu>().AddButton(
					"Switch Recording To This",
					delegate () {
						UserTransformManager.instance.StopRecording();
						UserTransformManager.instance.StartNewRecording(this);
						isMenuOpen = false;
					}
				);
			}
		}
		else {
			// nobody is recording
			menu.GetComponent<ObjectMenu>().AddButton(
				"New Recording",
				delegate () {
					UserTransformManager.instance.StartNewRecording(this);
					isMenuOpen = false;
					isMenuOpen = true;
				}
			);

			menu.GetComponent<ObjectMenu>().AddButton(
				"Load Recording",
				delegate () {
					try {
						UserTransformManager.instance.LoadRecording(this, "test.hrpsav");
						isMenuOpen = false;
					} catch(Exception e) {
						OutputText.instance.text = e.Message + "\n" + e.StackTrace;
					}
				}
			);
		}
	}
}
