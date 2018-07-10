using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Numpad : MonoBehaviour {
	public Action<string> onKeyPress = null;
	
	public void PressKey(string message) {
		onKeyPress?.Invoke(message);
	}
}
