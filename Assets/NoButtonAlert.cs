using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoButtonAlert : MonoBehaviour {
	public string text {
		get {
			return gameObject.transform.Find("Text").GetComponent<Text>().text;
		}
		set {
			gameObject.transform.Find("Text").GetComponent<Text>().text = value;
		}
	}

	public static GameObject Create(string text) {
		Vector3 offset = Camera.main.transform.forward;
		offset.y = 0;
		offset = offset.normalized * 0.4f;

		GameObject alert = Instantiate(
			Resources.Load<GameObject>("DefaultNoButtonAlert"),
			Camera.main.transform.position + offset,
			Quaternion.LookRotation(offset - Camera.main.transform.position, new Vector3(0, 1, 0))
		);

		alert.GetComponent<NoButtonAlert>().text = text;

		return alert;
	}
}
