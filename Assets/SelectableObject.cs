using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableObject : MonoBehaviour, IPointerClickHandler {
    public GameObject activateOnClick;

    public void OnPointerClick(PointerEventData eventData) {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        Color color = (new Color(1.0f, 1.0f, 1.0f) - renderer.material.GetColor("_Color")) / 4.0f;
        color = new Color(-1, -1, -1);
        renderer.material.SetColor("_EmissionColor", color);
        renderer.material.EnableKeyword("_EMISSION");
        activateOnClick.SetActive(true);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
