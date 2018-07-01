using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestButtonScript : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.GetComponent<Text>().text = "Clicked";
        gameObject.GetComponent<Text>().color = Color.red;
        Invoke("Reset", 1.0f);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Reset()
    {
        gameObject.GetComponent<Text>().text = "Button";
    }
}
