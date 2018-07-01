using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using System;

public class UserTransformableAnchor : UserTransformable {
	public static UserTransformableAnchor instance;

	public AnchorManager manager;

	private void Start() {
		instance = this;
	}

	public override void OnLoseFocus() {
		manager.StopEdit();
		manager.SaveAnchor();
	}

	public void BeginPlace() {
		isFocused = true;
		GetComponent<MeshCollider>().enabled = true;
		GetComponent<MeshRenderer>().enabled = true;

		gameObject.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.4f;

		UserTransformManager.instance.transformMode = UserTransformManager.TransformMode.translate;

		manager.StartEdit();
	}
}
