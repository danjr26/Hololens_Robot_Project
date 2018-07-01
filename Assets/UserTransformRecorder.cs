using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets {
	public class UserTransformRecorder {
		public UserTransformableRecordable target { get; private set; }
		public List<GameObject> keyframes { get; private set; }
		public List<Arrow> arrows { get; private set; }

		public UserTransformRecorder(UserTransformableRecordable target) {
			this.target = target;
			keyframes = new List<GameObject>();
			arrows = new List<Arrow>();
		}

		public void CreateSnapshot(Transform desiredTransform) {
			Transform oldTransform = target.transform;
			target.transform.SetPositionAndRotation(desiredTransform.position, desiredTransform.rotation);
			TakeSnapshot();
			target.transform.SetPositionAndRotation(oldTransform.position, oldTransform.rotation);
		}

		public void TakeSnapshot() {
			try {
				OutputText.instance.text = RobotInterface.instance.ToString();
				RobotInterface.instance.Move(new RobotInterface.MoveCommand(target.transform.position, target.transform.rotation));
			} catch (Exception e) {
				OutputText.instance.text = OutputText.instance.text + "\n\n" + e.Message + "\n" + e.StackTrace;
			}
			GameObject keyframeGhost = UnityEngine.Object.Instantiate(target.gameObject, target.gameObject.transform.parent);
			keyframeGhost.gameObject.GetComponent<UserTransformableRecordable>().enabled = false;
			keyframeGhost.gameObject.GetComponent<UserTransformableGhost>().enabled = true;
			Color color = keyframeGhost.GetComponent<MeshRenderer>().material.color;
			color.a = 0.4f;
			keyframeGhost.GetComponent<MeshRenderer>().material.color = color;
			keyframes.Add(keyframeGhost);

			if (keyframes.Count > 1) {
				Arrow arrow = UnityEngine.Object.Instantiate<Arrow>(UserTransformManager.instance.arrowPrefab, target.gameObject.transform.parent);
				color = Color.white;
				color.a = 0.4f;
				foreach (MeshRenderer renderer in arrow.GetComponentsInChildren<MeshRenderer>()) renderer.material.color = color;
				arrows.Add(arrow);

				arrow.FromTo(keyframes[keyframes.Count - 2], keyframes[keyframes.Count - 1]);
			}
		}

		public void DeleteSnapshot(GameObject objectToDelete) {
			for(int i = 0; i < keyframes.Count; i++) {
				if(keyframes[i] == objectToDelete) {
					UnityEngine.Object.Destroy(keyframes[i]);
					keyframes.RemoveAt(i);
					if (i > 0) {
						UnityEngine.Object.Destroy(arrows[i - 1].gameObject);
						arrows.RemoveAt(i - 1);
						if (arrows.Count > 0) arrows[i - 1].GetComponent<Arrow>().FromTo(
							keyframes[i - 1], keyframes[i]
						);
					} else if (keyframes.Count > 1) {
						UnityEngine.Object.Destroy(arrows[0].gameObject);
						arrows.RemoveAt(0);
					}
				}
			}
		}

		public void DeleteAllSnapshots() {
			for(int i = keyframes.Count - 1; i >= 0; i--) {
				UnityEngine.Object.Destroy(keyframes[i]);
				if (i > 0) UnityEngine.Object.Destroy(arrows[i - 1].gameObject);
			}
			keyframes.Clear();
			arrows.Clear();
		}

		public void UpdateArrowsConnectedTo(GameObject keyframe) {
			for(int i = 0; i < keyframes.Count; i++) {
				if(keyframes[i] == keyframe) {
					if (i > 0) arrows[i - 1].FromTo(keyframes[i - 1], keyframes[i]);
					if (i < keyframes.Count - 1) arrows[i].FromTo(keyframes[i], keyframes[i + 1]);
				}
			}
		}
	}
}
