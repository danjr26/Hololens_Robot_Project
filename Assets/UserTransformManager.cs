using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;
using System.IO;

public class UserTransformManager : MonoBehaviour {
	public static UserTransformManager instance;

	public UserTransformRecorder recorder { get; private set; }
	//public RobotInterface robotInterface;

	public Arrow arrowPrefab;

	private UserTransformable _focusedObject = null;
	public UserTransformable focusedObject {
		get {
			return _focusedObject;
		}
		set {
			if(_focusedObject != null) {
				_focusedObject.OnLoseFocus();
			}
			_focusedObject = value;
			transformMode = TransformMode.none;
			_focusedObject.OnGainFocus();
		}
	}

	public bool isRecording {
		get {
			return recorder != null;
		}
	}

	public enum TransformMode {
		none,
		translate,
		rotate,
		num_options
	}

	private TransformMode _transformMode;
	public TransformMode transformMode {
		get {
			return _transformMode;
		}
		set {
			switch(value) {
				case TransformMode.none:
					HoloInputManager.instance.ChangeCaptureType(HoloInputManager.GestureCaptureType.none);
					break;
				case TransformMode.translate:
				case TransformMode.rotate:
					HoloInputManager.instance.ChangeCaptureType(HoloInputManager.GestureCaptureType.manipulation);
					break;
			}
			_transformMode = value;
		}
	}

	void Start () {
		instance = this;
		focusedObject = null;
		recorder = null;
	}

	public void StartRecording(UserTransformableRecordable target) {
		recorder = new UserTransformRecorder(target);
	}

	public void StopRecording() {
		recorder.DeleteAllSnapshots();
		recorder = null;
	}

	public void SaveRecording() {
		try {
			FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "test.hrpsav"), 16 + 28 * recorder.keyframes.Count);

			BinaryWriter writer = new BinaryWriter(file);
			writer.Write((int)recorder.keyframes.Count);

			for(int i = 0; i < recorder.keyframes.Count; i++) {
				writer.Write((float)recorder.keyframes[i].transform.position.x);
				writer.Write((float)recorder.keyframes[i].transform.position.y);
				writer.Write((float)recorder.keyframes[i].transform.position.z);

				writer.Write((float)recorder.keyframes[i].transform.rotation.x);
				writer.Write((float)recorder.keyframes[i].transform.rotation.y);
				writer.Write((float)recorder.keyframes[i].transform.rotation.z);
				writer.Write((float)recorder.keyframes[i].transform.rotation.w);
			}

			writer.Write((uint)0xffffffff);

			file.Flush();
			
		} catch (Exception e) {
			OutputText.instance.text = e.Message + "\n" + e.StackTrace;
		}
	}

	public void LoadRecording(GameObject objectToApply) {
		try {
			FileStream file = File.OpenRead(Path.Combine(Application.persistentDataPath, "test.hrpsav"));

			file.Position = 0;

			BinaryReader reader = new BinaryReader(file);
			OutputText.instance.text = "Got reader";
			int n = reader.ReadInt32();
			OutputText.instance.text = "n = " + n.ToString();

			Vector3 position = new Vector3();
			Quaternion rotation = new Quaternion();

			for (int i = 0; i < n; i++) {
				position.x = reader.ReadSingle();
				position.y = reader.ReadSingle();
				position.z = reader.ReadSingle();

				rotation.x = reader.ReadSingle();
				rotation.y = reader.ReadSingle();
				rotation.z = reader.ReadSingle();
				rotation.w = reader.ReadSingle();

				objectToApply.transform.SetPositionAndRotation(position, rotation);

				recorder.CreateSnapshot(objectToApply.transform);
			}

			OutputText.instance.text = "done";

			if (reader.ReadUInt32() != 0xffffffff)
				throw new Exception("Invalid save file");
		}
		catch (Exception e) {
			OutputText.instance.text = OutputText.instance.text + "\n" + e.Message + "\n" + e.StackTrace;
		}
	}
}