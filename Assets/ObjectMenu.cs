using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMenu : MonoBehaviour {
	public GameObject boundObject { get; private set; }

	public GameObject buttonPrefab;

	private GameObject panel;

	public List<GameObject> buttons { get; private set; }

	public Vector2 dimensions {
		get {
			return (gameObject.transform as RectTransform).sizeDelta;
		}
		set {
			(gameObject.transform as RectTransform).sizeDelta = value;
			gameObject.GetComponent<BoxCollider>().size = 
				new Vector3(value.x, value.y, gameObject.GetComponent<BoxCollider>().size.z);
			gameObject.GetComponent<BoxCollider>().center =
				new Vector3(0, -value.y / 2.0f, gameObject.GetComponent<BoxCollider>().center.z);
		}
	}

	public float width {
		get {
			return dimensions.x;
		}
		set {
			dimensions = new Vector2(value, dimensions.y);
		}
	}

	public float height {
		get {
			return dimensions.y;
		}
		set {
			dimensions = new Vector2(dimensions.x, value);
		}
	}

	const float yStart = -0.005f;
	const float yDifference = -0.035f;
	const float yEnd = 0.0f;

	void Awake() {
		buttons = new List<GameObject>();
		boundObject = null;
		panel = transform.Find("Panel").gameObject;
	}
	
	void Update () {
		RaycastHit hit;
		Collider collider = gameObject.GetComponent<Collider>();
		Camera camera = Camera.main;
		Ray ray = new Ray(camera.transform.position, camera.transform.forward);

		if ((HoloInputManager.instance.clickThisFrame || HoloInputManager.instance.startHoldThisFrame) &&
			!gameObject.GetComponent<Collider>().Raycast(ray, out hit, 10.0f)) {

			if (boundObject == null) {
				Destroy(this);
			}
			else {
				boundObject.GetComponent<MenuOwner>().isMenuOpen = false;
			}
		}
	}

	public void Bind(GameObject objectToBind) {
		boundObject = objectToBind;
		gameObject.SetActive(true);
		Camera camera = Camera.main;

		RaycastHit hit = new RaycastHit();
		MeshCollider collider = objectToBind.GetComponent<MeshCollider>();
		bool didHit = 
			(collider == null) ? false : 
			collider.Raycast(new Ray(camera.transform.position, objectToBind.transform.position - camera.transform.position), out hit, 10.0f);

		Vector3 point = (didHit) ? hit.point : objectToBind.transform.position;

		gameObject.transform.SetPositionAndRotation(
			point + (camera.transform.position - point).normalized * 0.05f + new Vector3(0, 0.013f, 0),
			Quaternion.LookRotation((point - camera.transform.position).normalized)
		);
	}

	public void AddButton(string text, Action onClick) {
		GameObject newButton = Instantiate(
			buttonPrefab, 
			gameObject.transform.position + gameObject.transform.TransformDirection(new Vector3(0f, yStart + yDifference * buttons.Count, 0f) * gameObject.transform.localScale.y), 
			gameObject.transform.rotation, 
			gameObject.transform
			);
		buttons.Add(newButton);
		newButton.GetComponent<MenuButton>().text = text;
		newButton.GetComponent<MenuButton>().onClick = onClick;
		height = -(yStart + yDifference * buttons.Count + yEnd);

	}
}
