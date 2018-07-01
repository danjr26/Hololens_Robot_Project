using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;


public class CursorScript : MonoBehaviour, IInputHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<SpriteRenderer>().material.color = Color.white;

        if (Input.GetMouseButton(0) ||
            Input.GetMouseButton(1) ||
            Input.GetMouseButton(2))
        {

            gameObject.GetComponent<SpriteRenderer>().material.color = Color.yellow;
        }

        if (Input.GetMouseButtonDown(0) ||
            Input.GetMouseButtonDown(1) ||
            Input.GetMouseButtonDown(2))
        {

            gameObject.GetComponent<SpriteRenderer>().material.color = Color.cyan;
        }

        if (Input.GetMouseButtonUp(0) ||
            Input.GetMouseButtonUp(1) ||
            Input.GetMouseButtonUp(2))
        {

            gameObject.GetComponent<SpriteRenderer>().material.color = Color.green;
        }
    }

    public void OnInputDown(InputEventData eventData)
    {
        gameObject.GetComponent<SpriteRenderer>().material.color = Color.black;
    }

    public void OnInputUp(InputEventData eventData)
    {
        gameObject.GetComponent<SpriteRenderer>().material.color = Color.white;
    }
}
