using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TwoButtonAlert : MonoBehaviour {
	public string text {
		get {
			return gameObject.transform.Find("Text").GetComponent<Text>().text;
		}
		set {
			gameObject.transform.Find("Text").GetComponent<Text>().text = value;
		}
	}

	public MenuButton button1 {
		get {
			return gameObject.transform.Find("Button1").GetComponent<MenuButton>();
		}
	}

	public MenuButton button2 {
		get {
			return gameObject.transform.Find("Button2").GetComponent<MenuButton>();
		}
	}

	public static GameObject Create(string text) {
		Vector3 offset = Camera.main.transform.forward;
		offset.y = 0;
		offset = offset.normalized * 0.4f;

		GameObject alert = Instantiate(
			Resources.Load<GameObject>("DefaultTwoButtonAlert"),
			Camera.main.transform.position + offset,
			Quaternion.LookRotation(offset - Camera.main.transform.position, new Vector3(0, 1, 0))
		);

		alert.GetComponent<TwoButtonAlert>().text = text;

		return alert;
	}
}
