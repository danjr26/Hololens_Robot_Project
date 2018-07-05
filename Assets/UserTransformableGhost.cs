using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
public class UserTransformableGhost : UserTransformable {
	Arrow arrowPrefab;
	Arrow arrow = null;
	UserTransformableGhost predecessor = null;
	UserTransformableGhost successor = null;
	UserTransformKeyframe keyframe = null;

	private void Awake() {
		arrowPrefab = (Resources.Load("DefaultArrow") as GameObject).GetComponent<Arrow>();
	}

	private void OnDestroy() {
		if (arrow != null) Destroy(arrow.gameObject);
	}

	public void GhostifyRenderer() {
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer != null) {
			Color color = renderer.material.color;
			color.a = 0.4f;
			renderer.material.color = color;
		}
	}

	public void DeghostifyRenderer() {
		MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
		if (renderer != null) {
			Color color = renderer.material.color;
			color.a = 1.0f;
			renderer.material.color = color;
		}
	}

	public void BindKeyframe(UserTransformKeyframe newKeyframe) {
		keyframe = newKeyframe;
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
			if (arrow == null) arrow = Instantiate(arrowPrefab);
			arrow.FromTo(newPredecessor.gameObject, gameObject);
		}
	}

	public void UpdateArrow() {
		if (arrow != null) arrow.FromTo(predecessor.gameObject, gameObject);
	}

	public void UpdateBothArrows() {
		UpdateArrow();
		if (successor != null) successor.UpdateArrow();
	}

	protected override void BuildMenu() {
		if (!isMenuOpen) return;
		menu.GetComponent<ObjectMenu>().AddButton(
			"Delete Snapshot",
			delegate () {
				UserTransformManager.instance.recordEnvironment.DeleteSnapshot(gameObject);
				isMenuOpen = false;
			}
		);
	}

	new protected void Update() {
		base.Update();
		if (keyframe != null && 
			UserTransformManager.instance.focusedObject == this &&  
			HoloInputManager.instance.updateManipulationThisFrame) {

			keyframe.GetFromTransform(gameObject.transform);

			UpdateBothArrows();
		}
		
	}
}
