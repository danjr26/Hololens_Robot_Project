using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTransformableGhost : UserTransformable {
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
