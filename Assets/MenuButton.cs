using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {
	public Color normalButtonColor;
	public Color normalTextColor;
	public Color highlightButtonColor;
	public Color highlightTextColor;
	public Color clickButtonColor;
	public Color clickTextColor;
	public float highlightFadeTime;
	public float clickFadeTime;

	public Color buttonColor {
		get {
			return GetComponent<Image>().color;
		}
		private set {
			GetComponent<Image>().color = value;
		}
	}

	public Color textColor {
		get {
			return GetComponentInChildren<Text>().color;
		}
		private set {
			GetComponentInChildren<Text>().color = value;
		}
	}

	public string text {
		get {
			return gameObject.GetComponentInChildren<Text>().text;
		}
		set {
			gameObject.GetComponentInChildren<Text>().text = value;
		}
	}

	public Action onClick;

	private bool _isClicking;
	public bool isClicking {
		get {
			return _isClicking;
		}
		private set {
			if(!_isClicking && value) {
				StartCoroutine("FadeFromClick");
				onClick();
			}
			_isClicking = value;
		}
	}

	private bool _isHovering;
	public bool isHovering {
		get {
			return _isHovering;
		}
		private set {
			if(!_isHovering && value) {
				StartCoroutine("FadeToHighlight");
			} else if (_isHovering && !value) {
				StartCoroutine("FadeFromHighlight");
			}
			_isHovering = value;
		}
	}

	void Start () {
		buttonColor = normalButtonColor;
		textColor = normalTextColor;

		_isClicking = false;
		_isHovering = false;
	}
	
	void Update () {
		if(GazeCursor.instance.onWhat == gameObject) {
			if (HoloInputManager.instance.clickThisFrame) isClicking = true;
			else isClicking = false;
			isHovering = true;
		} else {
			isClicking = false;
			isHovering = false;
		}
	}

	IEnumerator FadeToHighlight() {
		for(float t = 0.0f; t < highlightFadeTime; t += Time.deltaTime) {
			buttonColor = Color.Lerp(normalButtonColor, highlightButtonColor, t / highlightFadeTime);
			textColor = Color.Lerp(normalTextColor, highlightTextColor, t / highlightFadeTime);
			yield return null;
		}
	}

	IEnumerator FadeFromHighlight() {
		for (float t = 0.0f; t < highlightFadeTime; t += Time.deltaTime) {
			buttonColor = Color.Lerp(highlightButtonColor, normalButtonColor, t / highlightFadeTime);
			textColor = Color.Lerp(highlightTextColor, normalTextColor, t / highlightFadeTime);
			yield return null;
		}
	}

	IEnumerator FadeFromClick() {
		for (float t = 0.0f; t < clickFadeTime; t += Time.deltaTime) {
			buttonColor = Color.Lerp(clickButtonColor, normalButtonColor, t / highlightFadeTime);
			textColor = Color.Lerp(clickTextColor, normalTextColor, t / highlightFadeTime);
			yield return null;
		}
	}
}
