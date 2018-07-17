using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets {
	public class UserTransformLiveEnvironment {
		public UserTransformableRecordable target { get; private set; }

		public UserTransformLiveEnvironment(UserTransformableRecordable target) {
			this.target = target;
		}

		public void UpdateRobot() {
			RobotInterface.instance.Move(new RobotInterface.MoveCommand(target.transform.localPosition, target.transform.localRotation));
		}
	}
}
