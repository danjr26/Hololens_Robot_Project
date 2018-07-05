using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTransformable : MenuOwner {

	private void Awake() {
		gameObject.layer = LayerMask.NameToLayer("UserTransformable");
	}

	public bool isFocused {
		get {
			return UserTransformManager.instance.focusedObject == this;
		}
		set {
			if (value) {
				UserTransformManager.instance.focusedObject = this;
			}
			else if (isFocused) {
				UserTransformManager.instance.focusedObject = null;
			}
		}
	}

	public bool[] isModeEnabled;

	protected void Update() {
		HoloInputManager inputManager = HoloInputManager.instance;
		UserTransformManager transformManager = UserTransformManager.instance;

		if (!isFocused) {
			if (transformManager.focusedObject == null && GazeCursor.instance.onWhat == gameObject) {
				if (inputManager.startHoldThisFrame) {
					isMenuOpen = true;
				}
				if (inputManager.clickThisFrame) {
					isFocused = true;
					UserTransformManager.instance.transformMode = UserTransformManager.TransformMode.translate;
				}
			}
			return;
		}

		if (inputManager.clickThisFrame) {
			UserTransformManager.TransformMode nextMode = GetNextTransformMode();
			switch (transformManager.transformMode) {
				case UserTransformManager.TransformMode.none:
					if (GazeCursor.instance.onWhat == gameObject) transformManager.transformMode = nextMode;
					break;
				case UserTransformManager.TransformMode.translate:
					UserTransformManager.instance.transformMode = nextMode;
					break;
				case UserTransformManager.TransformMode.rotate:
					UserTransformManager.instance.transformMode = nextMode;
					break;
			}
			if(nextMode == UserTransformManager.TransformMode.none) isFocused = false;
			return;
		}

		switch (transformManager.transformMode) {
			case UserTransformManager.TransformMode.rotate:
				if (inputManager.updateManipulationThisFrame) {
					Vector3 objectToHand = inputManager.handPosition - gameObject.transform.position;
					float objectToHandDistance = objectToHand.magnitude;
					Vector3 objectToHandNormal = objectToHand.normalized;

					Vector3 perpendicularDelta = inputManager.manipulationFrameDelta - Vector3.Project(inputManager.manipulationFrameDelta, objectToHandNormal);
					float perpendicularDeltaDistance = perpendicularDelta.magnitude;
					Vector3 perpendicularDeltaNormal = perpendicularDelta.normalized;

					Vector3 axis = Vector3.Cross(objectToHandNormal, perpendicularDeltaNormal);
					float radians = inputManager.manipulationFrameDelta.magnitude / objectToHandDistance;

					gameObject.transform.Rotate(axis, radians * Mathf.Rad2Deg, Space.World);
				}
				break;
			case UserTransformManager.TransformMode.translate:
				if (inputManager.updateManipulationThisFrame) {
					gameObject.transform.Translate(inputManager.manipulationFrameDelta, Space.World);
				}
				break;
		}
	}

	private UserTransformManager.TransformMode GetNextTransformMode() {
		UserTransformManager transformManager = UserTransformManager.instance;
		int returnMode;
		for (
			returnMode = ((int)transformManager.transformMode + 1) % (int)UserTransformManager.TransformMode.num_options;
			returnMode != (int)transformManager.transformMode && !isModeEnabled[returnMode];
			returnMode = (returnMode + 1) % 3
			) ;
		return (UserTransformManager.TransformMode)returnMode;
	}

	public virtual void OnGainFocus() { }

	public virtual void OnLoseFocus() { }
}
