using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
	float headRadius {
		get {
			return transform.localScale.x;
		}
		set {
			transform.localScale = new Vector3(
				value, 
				value, 
				value
			);
		}
	}

	float headLengthToRadius {
		get {
			return cone.transform.localScale.y;
		}
		set {
			cone.transform.localScale = new Vector3(
				cone.transform.localScale.x,
				value,
				cone.transform.localScale.z
			);
		}
	}

	float stickRadiusToRadius {
		get {
			return stick.transform.localScale.x * 0.5f;
		}
		set {
			stick.transform.localScale = new Vector3(
				value * 2.0f,
				stick.transform.localScale.y,
				value * 2.0f
			);
		}
	}

	float stickLength {
		get {
			return stick.transform.localScale.y * headRadius * 2.0f;
		}
		set {
			stick.transform.localScale = new Vector3(
				stick.transform.localScale.x,
				value / headRadius / 2.0f,
				stick.transform.localScale.z
			);
			stick.transform.localPosition = new Vector3(
				stick.transform.localPosition.x,
				-value / headRadius / 2.0f,
				stick.transform.localPosition.z
			);
		}
	}

	float headLength {
		get {
			return headLengthToRadius * headRadius;
		}
		set {
			headLengthToRadius = value / headRadius;
		}
	}

	float totalLength {
		get {
			return stickLength + headLength;
		}
		set {
			stickLength = value - headLength;
		}
	}

	GameObject cone;
	GameObject stick;

	private void Awake() {
		cone = transform.Find("cone").gameObject;
		stick = transform.Find("stick").gameObject;
		headRadius = 0.003f;
		headLengthToRadius = 3.0f;
		stickRadiusToRadius = 0.5f;
	}

	void Start () {
		//stickLength = 0.1f;
		//FromTo(new Vector3(0f, 0f, 0.5f), new Vector3(0.0f, 0.0f, 1.0f));
	}
	
	void Update () {
		
	}

	public void FromTo(Vector3 from, Vector3 to) {
		Vector3 direction = to - from;
		totalLength = direction.magnitude;
		transform.position = from + direction.normalized * stickLength;
		transform.rotation = Quaternion.LookRotation(direction.normalized);
		transform.Rotate(90.0f, 0.0f, 0.0f, Space.Self);
	}

	public void FromTo(GameObject from, GameObject to) {
		RaycastHit hit;

		Vector3 center1 = from.transform.position;
		Vector3 center2 = to.transform.position;
		Vector3 point1, point2;
		bool didHit1, didHit2;

		float distance = Vector3.Distance(center1, center2);
		didHit1 = from.GetComponent<MeshCollider>().Raycast(new Ray(center2, center1 - center2), out hit, distance);
		point1 = didHit1 ? hit.point : center1;
		didHit2 = to.GetComponent<MeshCollider>().Raycast(new Ray(center1, center2 - center1), out hit, distance);
		point2 = didHit2 ? hit.point : center2;

		FromTo(point1, point2);
	}
}
