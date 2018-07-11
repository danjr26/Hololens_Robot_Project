using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

#if NETFX_CORE
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using System.Threading;
using System.Threading.Tasks;
#endif


public class RobotInterface : MonoBehaviour {
	public static RobotInterface instance;
	//public ToolData toolData = null;
	//public LoadData loadData = null;
	public bool isConnected { get; private set; }

	public Action onConnectSuccess = null;
	public Action<string> onConnectFailure = null;
	public Action onDisconnect = null;
	public Action onCommandSendSuccess = null;
	public Action<string> onCommandSendFailure = null;

#if NETFX_CORE
	StreamSocketListener socketListener;
	StreamSocket outSocket;
	DataWriter writer;
#endif

	void Awake() {
		instance = this;
		isConnected = false;
	}

	void Start() {
		instance = this;
	}

	private void Update() {
		
	}

	public async Task StartConnection(string ip, string socketName) {
#if NETFX_CORE
		try {
			if(isConnected) await EndConnection();
			outSocket = new StreamSocket();
			await outSocket.ConnectAsync(new HostName(ip), socketName);
			writer = new DataWriter(outSocket.OutputStream);
			isConnected = true;
			onConnectSuccess?.Invoke();
		} catch (Exception e) {
			onConnectFailure?.Invoke(e.Message);
		}
#endif
	}

	public async Task EndConnection() {
#if NETFX_CORE
		try {
			isConnected = false;
			await outSocket.CancelIOAsync();
			outSocket.Dispose();
			outSocket = null;
		} catch (Exception e) {

		}
#endif
	}

	public async Task SendCommand(string command) {
#if NETFX_CORE
		try {
			writer.WriteString(command);
			await writer.StoreAsync();
			await writer.FlushAsync();
			onCommandSendSuccess?.Invoke();
		} catch (Exception e) {
			onCommandSendFailure?.Invoke(e.Message);
		}
#endif
	}

	public async Task Move(MoveCommand command) {
		await SendCommand(command.ToURCommand());
	}

	public class MoveCommand {
		public bool ensureLinearity = false;
		public Vector3 position;
		public Quaternion rotation;
		public float velocity = 0.5f;
		public float acceleration = 0.5f;

		public MoveCommand(Vector3 endpoint, Quaternion orientation) {
			position = endpoint * 1000.0f; // meters to mm;
			rotation = orientation;
		}

		public string ToURCommand() {
			Vector3 eulerAngles = rotation.eulerAngles * Mathf.Deg2Rad;
			return
				(ensureLinearity) ? "movel" : "movej" 
				+ "("
					+ "p[" 
						+ position.x.ToString("F3") + ", "
						+ position.y.ToString("F3") + ", "
						+ position.z.ToString("F3") + ", "
						+ eulerAngles.x.ToString("F3") + ", "
						+ eulerAngles.y.ToString("F3") + ", "
						+ eulerAngles.z.ToString("F3")
					+ "]"
					+ "v=" + velocity.ToString("F3")
					+ "a=" + acceleration.ToString("F3")
				+ ")";
		}
	}

	/*
	public class MoveCommand {
		public bool ensureLinearity = false;
		public TargetPoseData targetPoseData = new TargetPoseData();
		public SpeedData speedData = new SpeedData();
		public ZoneData zoneData = new ZoneData();
		public StopPointData stopPointData = new StopPointData();

		public MoveCommand(Vector3 endpoint, Quaternion orientation) {
			targetPoseData.position = endpoint * 1000.0f;
			targetPoseData.orientation = orientation;
		}

		private string LinearityToRAPID() {
			return (ensureLinearity) ? "MoveL " : "MoveJ ";
		}

		public string ToRAPID() {
			ToolData toolData = RobotInterface.instance.toolData;
			return
				LinearityToRAPID() + "( \n" + 
				targetPoseData.ToRAPID() + ", \n" +
				speedData.ToRAPID() + ", \n" +
				zoneData.ToRAPID() + "\n" +
				((zoneData.isStopPoint) ? "\\Inpos:=" + stopPointData.ToRAPID() : "") + 
				((toolData == null) ? "" : (", \n" + toolData.ToRAPID())) + "\n);";
		}
	}

	public class QuadrantData {
		//used to break gimbal lock
		public int axis1 = 0;
		public int axis4 = 0;
		public int axis6 = 0;
		public int extraAxis = 0;

		public string ToRAPID() {
			return "[" + 
				axis1.ToString() + ", " + 
				axis4.ToString() + ", " +
				axis6.ToString() + ", " +
				extraAxis.ToString() + 
				"]";
		}
	}

	public class TargetPoseData {
		public Vector3 position = new Vector3(0.0f, 0.0f, 500.0f);
		public Quaternion orientation = Quaternion.identity;
		public QuadrantData quadrants = new QuadrantData();

		public string ToRAPID() {
			return "[" +
				Vector3ToRAPID(position) + ", " +
				QuaternionToRAPID(orientation) + ", " +
				quadrants.ToRAPID() +
				"]";
		}
	}

	public class SpeedData {
		public float translation = 1000.0f;
		public float rotation = 180.0f;
		public float externalTranslation = 1000.0f;
		public float externalRotation = 180.0f;

		public string ToRAPID() {
			return "[" +
				translation.ToString("F6") + ", " +
				rotation.ToString("F6") + ", " +
				externalTranslation.ToString("F6") + ", " +
				externalRotation.ToString("F6") + 
				"]";
		}
	}

	public class ZoneData {
		public bool isStopPoint = true;
		public float maxRadiusTranslation = 0.0f;
		public float maxRadiusRotation = 0.0f;
		public float maxRadiusExternal = 0.0f;
		public float maxRotationAngle = 0.0f;
		public float maxRadiusLinearExternal = 0.0f;
		public float maxRotationAngleExternal = 0.0f;

		public string ToRAPID() {
			return "[" +
				BoolToRAPID(isStopPoint) + ", " +
				maxRadiusTranslation.ToString("F6") + ", " +
				maxRadiusRotation.ToString("F6") + ", " +
				maxRadiusExternal.ToString("F6") + ", " +
				maxRotationAngle.ToString("F6") + ", " +
				maxRadiusLinearExternal.ToString("F6") + ", " +
				maxRotationAngleExternal.ToString("F6") +
				"]";
		}
	}
	
	public class StopPointData {
		public bool isSynced = true;

		// 100 is default, lower is more precise
		public float positionEpsilon = 100.0f;
		public float speedEpsilon = 100.0f;

		public float minWait = 0.0f;
		public float maxWait = 2.0f;

		public string ToRAPID() {
			return "[" +
				"inpos, " +
				BoolToRAPID(isSynced) + ", " + "[" +
					positionEpsilon.ToString("F6") + ", " +
					speedEpsilon.ToString("F6") + ", " +
					minWait.ToString("F6") + ", " +
					maxWait.ToString("F6") + 
					"]" + ", " +
				"0, 0, \"\", 0, 0" +
				"]";
		}
	}

	public class ToolData {
		public bool isHolding = true;
		public Quaternion orientation = Quaternion.identity;
		public LoadData load = new LoadData();

		public string ToRAPID() {
			return "[" +
				BoolToRAPID(isHolding) + ", " +
				QuaternionToRAPID(orientation) + ", " +
				load.ToRAPID() +
				"]";
		}
	}

	public class LoadData {
		public float mass = 0.0f;
		public Vector3 centerOfGravity = new Vector3();
		public Quaternion orientation = Quaternion.identity;
		public Vector3 axialMoments = new Vector3();

		public string ToRAPID() {
			return "[" +
				mass.ToString("F6") + ", " +
				Vector3ToRAPID(centerOfGravity) + ", " +
				QuaternionToRAPID(orientation) + ", " +
				axialMoments.x.ToString("F6") + ", " +
				axialMoments.y.ToString("F6") + ", " +
				axialMoments.z.ToString("F6") + 
				"]";
		}
	}

	public static string BoolToRAPID(bool b) {
		return (b) ? "TRUE" : "FALSE";
	}

	public static string Vector3ToRAPID(Vector3 v) {
		return "[" +
			v.x.ToString("F6") + ", " +
			v.y.ToString("F6") + ", " +
			v.z.ToString("F6") + 
			"]";
	} 

	public static string QuaternionToRAPID(Quaternion q) {
		return "[" +
			q.w.ToString("F6") + ", " +
			q.x.ToString("F6") + ", " +
			q.y.ToString("F6") + ", " +
			q.z.ToString("F6") + 
			"]";
	}*/
}
