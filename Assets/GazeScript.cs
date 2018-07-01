using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoloToolkit.Unity.InputModule;

public class GazeScript : MonoBehaviour {
    public Camera camera;
    public GameObject cursor;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 10.0f)) {
            cursor.SetActive(true);
            cursor.transform.position = hit.point;
            cursor.transform.rotation = Quaternion.LookRotation(hit.normal);
        } else {
            cursor.SetActive(false);
        }
	}
}
