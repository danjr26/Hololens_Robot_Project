using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets {
	public class UserTransformRecordEnvironment {
		public UserTransformableRecordable target { get; private set; }
		public UserTransformableRecording recording { get; private set; }
		public List<UserTransformableGhost> ghosts { get; private set; }

		public UserTransformRecordEnvironment(UserTransformableRecordable target) {
			this.target = target;
			recording = new UserTransformableRecording();
			ghosts = new List<UserTransformableGhost>();
		}

		public UserTransformRecordEnvironment(UserTransformableRecordable target, string filename) {
			this.target = target;
			recording = UserTransformableRecording.Load(filename);
			for(int i = 0; i < recording.keyframes.Count; i++) {
				ghosts.Add(recording.keyframes[i].CreateGhost(target));
				IncorporateGhost(i);
			}
		}

		public void CreateSnapshot(Transform desiredTransform) {
			UserTransformKeyframe keyframe = recording.AddKeyframe(desiredTransform);
			UserTransformableGhost ghost = keyframe.CreateGhost(target);
			ghosts.Add(ghost);
			IncorporateGhost(ghosts.Count - 1);
		}

		void IncorporateGhost(int index) {
			UserTransformableGhost ghost = ghosts[index];
			if(index > 0) {
				UserTransformableGhost prevGhost = ghosts[index - 1];
				prevGhost.MakePredecessorTo(ghost);
				ghost.MakeSuccessorTo(prevGhost);
			}
			if(index < ghosts.Count - 1) {
				UserTransformableGhost nextGhost = ghosts[index + 1];
				ghost.MakePredecessorTo(nextGhost);
				nextGhost.MakeSuccessorTo(ghost);
			}
		}

		public void DeleteSnapshot(GameObject objectToDelete) {
			UserTransformableGhost ghost = objectToDelete.GetComponent<UserTransformableGhost>();
			if (ghost == null) return;

			int index = ghosts.FindIndex(delegate(UserTransformableGhost g) { return g == ghost; });
			if (index < 0) return;

			if(index > 0) {
				ghost.MakeSuccessorTo(null);
				if (index < ghosts.Count - 1) {
					ghosts[index - 1].MakePredecessorTo(ghosts[index + 1]);
					ghosts[index + 1].MakeSuccessorTo(ghosts[index - 1]);
				} else {
					ghosts[index - 1].MakePredecessorTo(null);
				}
			} else if(index < ghosts.Count - 1) {
				ghost.MakePredecessorTo(null);
				ghosts[index + 1].MakeSuccessorTo(null);
			}

			ghosts.RemoveAt(index);
			recording.RemoveKeyframe(index);

			UnityEngine.Object.Destroy(objectToDelete);
		}

		public void DeleteAllSnapshots() {
			foreach(UserTransformableGhost ghost in ghosts) UnityEngine.Object.Destroy(ghost);
			ghosts.Clear();

			recording.RemoveAllKeyframes();
		}
	}
}
