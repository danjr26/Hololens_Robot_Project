using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace Assets {
	public class UserTransformableRecording {
		public List<UserTransformKeyframe> keyframes { get; private set; }

		public UserTransformableRecording() {
			keyframes = new List<UserTransformKeyframe>();
		}

		public UserTransformKeyframe AddKeyframe(Vector3 position, Quaternion rotation) {
			UserTransformKeyframe newKeyframe = new UserTransformKeyframe(position, rotation);
			keyframes.Add(newKeyframe);
			return newKeyframe;
		}

		public UserTransformKeyframe AddKeyframe(Transform transform) {
			UserTransformKeyframe newKeyframe = new UserTransformKeyframe(transform);
			keyframes.Add(newKeyframe);
			return newKeyframe;
		}

		public UserTransformKeyframe AddKeyframe(UserTransformKeyframe keyframe) {
			keyframes.Add(keyframe);
			return keyframe;
		}

		public void RemoveKeyframe(int index) {
			keyframes.RemoveAt(index);
		}

		public void RemoveAllKeyframes() {
			keyframes.Clear();
		}

		public async Task Execute(Transform frameOfReference) {
			foreach(UserTransformKeyframe keyframe in keyframes) {
				await keyframe.Execute(frameOfReference);
			}
		}

		public void Save(string filename) {
			try {
				FileStream file = File.Create(Path.Combine(Application.persistentDataPath, filename));

				BinaryWriter writer = new BinaryWriter(file);
				writer.Write((UInt32)keyframes.Count);

				for (int i = 0; i < keyframes.Count; i++) {
					keyframes[i].PutToBinary(ref writer);
				}

				writer.Write((UInt32)0xffffffff);

				file.Flush();

			}
			catch (Exception e) {
				OutputText.instance.text = e.Message + "\n" + e.StackTrace;
			}
		}

		public static UserTransformableRecording Load(string filename) {
			UserTransformableRecording newRecording = new UserTransformableRecording();

			try {
				FileStream file = File.OpenRead(Path.Combine(Application.persistentDataPath, filename));

				file.Position = 0;

				BinaryReader reader = new BinaryReader(file);
				int nKeyframes = reader.ReadInt32();

				for (int i = 0; i < nKeyframes; i++) {
					newRecording.AddKeyframe(new UserTransformKeyframe(ref reader));
				}

				if (reader.ReadUInt32() != 0xffffffff)
					throw new Exception("Invalid save file");
			}
			catch (Exception e) {
				OutputText.instance.text = OutputText.instance.text + "\n" + e.Message + "\n" + e.StackTrace;
			}

			return newRecording;
		}
	}
}
