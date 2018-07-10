using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IpConfigButton : MenuButton {
	public uint maxDigits;

	private void Start() {
		onClick = OnClick;
	}

	void OnClick() {
		text = "";
		gameObject.GetComponentInParent<IpConfigurator>()?.RequestNumpad(this);
	}

	public void OnKeyPress(string message) {
		switch(message) {
			case "Back":
				if (text.Length > 0) text = text.Substring(0, text.Length - 1);
				break;
			case "Enter":
				if (text.Length == 0) text = "0";
				gameObject.GetComponentInParent<IpConfigurator>()?.ReleaseNumpad();
				break;
			default:
				if (text.Length < maxDigits) text += message;
				break;
		}
	}
}
