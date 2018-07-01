using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeaveOnExit : MonoBehaviour {
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Camera camera = Camera.main;
		RaycastHit hit;
		if ((HoloInputManager.instance.clickThisFrame || HoloInputManager.instance.startHoldThisFrame) &&
			!gameObject.GetComponent<Collider>().Raycast(new Ray(camera.transform.position, camera.transform.forward), out hit, 10.0f)) {

			gameObject.SetActive(false);
			gameObject.GetComponentInChildren<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
		}
    }
}
