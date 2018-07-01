using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA;
using UnityEngine.XR.WSA.Persistence;
using System;
using System.Threading;

public class AnchorManager : MonoBehaviour {
	public string anchorName;
	WorldAnchorStore store = null;

	void Awake () {
		/*try { 
			WorldAnchorStore.GetAsync(
				delegate (WorldAnchorStore fetchedStore) {
					store = fetchedStore;
					LoadAnchor();
				}
			);
		}
		catch (Exception e) {
			OutputText.instance.text = e.Message + "\n" + e.StackTrace;
		}*/
	}
	
	public void LoadAnchor() {
		if (store != null && !store.Load(anchorName, gameObject)) {
			gameObject.AddComponent<WorldAnchor>();
			SaveAnchor();
		}
		//OutputText.instance.text = OutputText.instance.text + gameObject.transform.position.ToString();
	}

	public void SaveAnchor() {
		store.Delete(anchorName);
		if(!store.Save(anchorName, gameObject.GetComponent<WorldAnchor>())) {
			OutputText.instance.text = "save anchor failed";
		} else {
			OutputText.instance.text = "save anchor succeeded";
		}
	}

	public void StartEdit() {
		DestroyImmediate(gameObject.GetComponent<WorldAnchor>());
	}

	public void StopEdit() {
		gameObject.AddComponent<WorldAnchor>();
	}
}
