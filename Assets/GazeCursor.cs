using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GazeCursor : MonoBehaviour {
	public static GazeCursor instance { get; private set; }

	Sprite activeSprite {
		get {
			return GetComponent<SpriteRenderer>().sprite;
		}
		set {
			GetComponent<SpriteRenderer>().sprite = value;
		}
	}
	public Sprite pointerSprite;
	public Sprite readySprite;
	public Sprite clickSprite;
	public Sprite negativeSprite;
	public Sprite moveSprite;
	public Sprite rotateSprite;

	public RaycastHit lastHit { get; private set; }
	public GameObject onWhat { get; private set; }

	public bool isVisible {
		get {
			return GetComponent<SpriteRenderer>().enabled;
		}
		private set {
			GetComponent<SpriteRenderer>().enabled = value;
		}
	}

	void Start() {
		instance = this;
		onWhat = null;
		activeSprite = pointerSprite;
		transform.position = new Vector3(0, 0, 1);
		isVisible = false;
	}

	void Update() {
		int stepN = 0;
		RaycastHit hit;
		Camera camera = Camera.main;
		SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
		if(camera == null || renderer == null) {
			OutputText.instance.text = "null: " + camera.ToString() + " " + renderer.ToString();
		}
		try {
			switch (UserTransformManager.instance.transformMode) {
				case UserTransformManager.TransformMode.none: {
						stepN = 1;
						activeSprite = pointerSprite;
						if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 10.0f)) {
							stepN = 2;
							isVisible = true;
							renderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
							transform.position = hit.point;
							transform.rotation = Quaternion.LookRotation(hit.normal);
							onWhat = hit.collider.gameObject;
							lastHit = hit;
							stepN = 3;
						}
						else {
							stepN = 4;
							isVisible = false;
							renderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
							onWhat = null;
							stepN = 5;
						}
					}
					break;
				case UserTransformManager.TransformMode.rotate:
				case UserTransformManager.TransformMode.translate: {
						if(UserTransformManager.instance.focusedObject == null) {
							UserTransformManager.instance.transformMode = UserTransformManager.TransformMode.none;
							return;
						}
						stepN = 6;
						Physics.Raycast(
							camera.transform.position,
							UserTransformManager.instance.focusedObject.transform.position - camera.transform.position,
							out hit, 10.0f
						);
						stepN = 7;
						isVisible = true;
						renderer.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						transform.position = hit.point;
						transform.rotation = Quaternion.LookRotation(hit.normal);
						onWhat = hit.collider.gameObject;
						lastHit = hit;
						stepN = 8;
						if (HoloInputManager.instance.isManipulating) {
							gameObject.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);
						}
						else {
							gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
						}
						stepN = 9;
						activeSprite =
							(UserTransformManager.instance.transformMode == UserTransformManager.TransformMode.rotate) ?
							rotateSprite : moveSprite;
						stepN = 10;
					}
					break;
				default:
					OutputText.instance.text = "Something has gone terribly wrong.";
					break;
			}
		} catch (Exception e) {
			OutputText.instance.text = e.Message + " " + stepN.ToString() + "\n" + e.StackTrace;
		}
	}
}
