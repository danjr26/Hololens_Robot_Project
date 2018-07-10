using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class OneButtonAlert : MonoBehaviour {
	public string text {
		get {
			return gameObject.transform.Find("Text").GetComponent<Text>().text;
		}
		set {
			gameObject.transform.Find("Text").GetComponent<Text>().text = value;
		}
	}

	public MenuButton button {
		get {
			return gameObject.transform.Find("Button").GetComponent<MenuButton>();
		} 
	}

	private void Awake() {
		button.onClick = delegate () {
			Destroy(gameObject);
		};
	}

	public static GameObject Create(string text) {
		Vector3 offset = Camera.main.transform.forward;
		offset.y = 0;
		offset = offset.normalized * 0.4f;

		GameObject alert = Instantiate(
			Resources.Load<GameObject>("DefaultOneButtonAlert"),
			Camera.main.transform.position + offset,
			Quaternion.LookRotation(offset - Camera.main.transform.position, new Vector3(0, 1, 0))
		);

		alert.GetComponent<OneButtonAlert>().text = text;

		return alert;
	}
}
