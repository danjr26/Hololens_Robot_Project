using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;
using UnityEngine.EventSystems;

public class OnTextClickScript : MonoBehaviour, IInputClickHandler, IInputHandler, IPointerEnterHandler, IPointerExitHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnInputClicked(InputClickedEventData eventData) {
        Text text = gameObject.GetComponent<Text>();
        text.text = "Click";
    }

    public void OnInputDown(InputEventData eventData) {
        Text text = gameObject.GetComponent<Text>();
        text.color = Color.cyan;
    }

    public void OnInputUp(InputEventData eventData) {
        Text text = gameObject.GetComponent<Text>();
        text.color = Color.yellow;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Text text = gameObject.GetComponent<Text>();
        text.text = "Enter";
    }

    public void OnPointerExit(PointerEventData eventData) {
        Text text = gameObject.GetComponent<Text>();
        text.text = "Exit";
    }
}
