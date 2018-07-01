using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputText : MonoBehaviour {
	public static OutputText instance;

	public string text {
		get {
			return gameObject.GetComponent<TextMesh>().text;
		}
		set {
			gameObject.GetComponent<TextMesh>().text = value;
		}
	}

	private void Awake() {
		instance = this;
	}

	private void Start() {
		instance = this;
	}
}
