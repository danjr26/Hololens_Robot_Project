using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets {
	public class UserTransformLiveEnvironment {
		public UserTransformableRecordable target { get; private set; }
		Vector3 lastCommandedPosition = new Vector3();
		Quaternion lastCommandedRotation = Quaternion.identity;

		public UserTransformLiveEnvironment(UserTransformableRecordable target) {
			this.target = target;
		}

		public void UpdateRobot() {
			if (target.transform.localPosition != lastCommandedPosition || target.transform.localRotation != lastCommandedRotation) {
				RobotInterface.MoveToPoseCommand command = new RobotInterface.MoveToPoseCommand(target.transform.localPosition, target.transform.localRotation);
				command.velocity = 1.5f;
				lastCommandedPosition = target.transform.localPosition;
				lastCommandedRotation = target.transform.localRotation;
				RobotInterface.instance.CancelQueuedMoves();
				RobotInterface.instance.QueueMove(command);
			} 
		}
	}
}
