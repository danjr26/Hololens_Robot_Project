using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		RaycastHit hit;
		Camera camera = Camera.main;

		switch(UserTransformManager.instance.transformMode) {
			case UserTransformManager.TransformMode.none: {
					activeSprite = pointerSprite;
					if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 10.0f)) {
						isVisible = true;
						GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						transform.position = hit.point;
						transform.rotation = Quaternion.LookRotation(hit.normal);
						onWhat = hit.collider.gameObject;
						lastHit = hit;
					}
					else {
						isVisible = false;
						GetComponent<SpriteRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
						onWhat = null;
					}
				}
				break;
			case UserTransformManager.TransformMode.rotate:
			case UserTransformManager.TransformMode.translate: {
					Physics.Raycast(
						camera.transform.position,
						UserTransformManager.instance.focusedObject.transform.position - camera.transform.position,
						out hit, 10.0f
					);
					isVisible = true;
					GetComponent<SpriteRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
					transform.position = hit.point;
					transform.rotation = Quaternion.LookRotation(hit.normal);
					onWhat = hit.collider.gameObject;
					lastHit = hit;

					if (HoloInputManager.instance.isManipulating) {
						gameObject.transform.localScale = new Vector3(0.006f, 0.006f, 0.006f);
					}
					else {
						gameObject.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
					}

					activeSprite =
						(UserTransformManager.instance.transformMode == UserTransformManager.TransformMode.rotate) ?
						rotateSprite : moveSprite;
				}
				break;
		}
	}
}
