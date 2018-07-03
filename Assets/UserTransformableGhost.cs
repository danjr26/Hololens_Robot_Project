using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTransformableGhost : UserTransformable {
	Arrow arrowPrefab;
	Arrow arrow = null;
	UserTransformableGhost predecessor = null;
	UserTransformableGhost successor = null;

	private void Awake() {
		arrowPrefab = Resources.Load<Arrow>("DefaultArrow");
	}

	public void MakePredecessorTo(UserTransformableGhost newSuccessor) {
		successor = newSuccessor;
	}

	public void MakeSuccessorTo(UserTransformableGhost newPredecessor) {
		predecessor = newPredecessor;
		if (predecessor == null) {
			if (arrow != null) Destroy(arrow);
		}
		else {
			if (arrow != null) arrow = Instantiate(arrowPrefab);
			arrow.FromTo(newPredecessor.gameObject, gameObject);
		}
	}

	protected override void BuildMenu() {
		if (!isMenuOpen) return;
		menu.GetComponent<ObjectMenu>().AddButton(
			"Delete Snapshot",
			delegate () {
				UserTransformManager.instance.recorder.DeleteSnapshot(gameObject);
				isMenuOpen = false;
			}
		);
	}

	new protected void Update() {
		base.Update();
		if (HoloInputManager.instance.updateManipulationThisFrame) UserTransformManager.instance.recorder.UpdateArrowsConnectedTo(gameObject);
	}
}
