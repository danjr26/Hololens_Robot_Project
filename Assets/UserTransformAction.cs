using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

namespace Assets {
	public class UserTransformAction {
		public virtual async Task Execute(UserTransformKeyframe keyframe, Transform frameOfReference) { }

		public static UserTransformAction FromBinary(ref BinaryReader reader) {
			return new UserTransformAction();
		}

		public void PutToBinary(ref BinaryWriter writer) {

		}
	}
}
