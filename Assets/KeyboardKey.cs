using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardKey : MonoBehaviour {
	public string message;

	void Start () {
		gameObject.GetComponent<MenuButton>().onClick = onClick;
	}
	
	void Update () {
		
	}

	void onClick() {
		gameObject.transform.parent.GetComponent<Numpad>()?.PressKey(message);
	}
}
