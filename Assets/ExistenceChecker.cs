using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExistenceChecker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		OutputText.instance.text = "ExistenceChecker: Started";
	}
	
	// Update is called once per frame
	void Update () {
		OutputText.instance.text = "ExistenceChecker: Updating";
	}
}
