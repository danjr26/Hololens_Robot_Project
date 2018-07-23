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
	public bool isConnectedToCommand { get; private set; }
	public bool isConnectedToRegister { get; private set; }

	public bool robotIsMoving { get; private set; }

	const uint fetchBufferMaxLength = 1024;
	uint fetchBufferLength = 0;
	private byte[] fetchBuffer = new byte[fetchBufferMaxLength];
	public bool isFetching { get; private set; }

	public Action onConnectSuccess = null;
	public Action<string> onConnectFailure = null;
	public Action onDisconnect = null;
	public Action onCommandSendSuccess = null;
	public Action<string> onCommandSendFailure = null;

	private Queue<IMoveCommand> moveQueue = new Queue<IMoveCommand>();

#if NETFX_CORE
	StreamSocket commandSocket;
	StreamSocket registerSocket;
	DataReader commandReader;
	DataWriter commandWriter;
	DataReader registerReader;
	DataWriter registerWriter;
#endif

	void Awake() {
		instance = this;
		isConnectedToCommand = false;
		isConnectedToRegister = false;
		isFetching = false;
		robotIsMoving = false;
	}

	void Start() {
		instance = this;
		InvokeRepeating("ProcessMoveQueue", 0.0f, 0.015f);
	}

	private void ProcessMoveQueue() {
		float[] jointSpeeds;
		bool robotWasMoving = robotIsMoving;
		if (GetFetchedRealJointSpeeds(out jointSpeeds)) {
			robotIsMoving = false;
			float max = 0.0f;
			for (uint i = 0; i < 6; i++) {
				if (Mathf.Abs(jointSpeeds[i]) > 1.0f) {
					robotIsMoving = true;
				}
				if (Mathf.Abs(jointSpeeds[i]) > max) max = Mathf.Abs(jointSpeeds[i]);
 			}
			OutputText.instance.text = max.ToString("F4");
		}

		if (moveQueue.Count > 0 && !robotIsMoving) {
			MoveNow(moveQueue.Dequeue());
			robotIsMoving = true;
			CancelInvoke("ProcessMoveQueue");
			InvokeRepeating("ProcessMoveQueue", 0.5f, 0.015f);
		}
		else if (!isFetching) {
			FetchRealJointSpeeds();
		}
	}

	public async Task StartConnection(string ip, string socketName) {
#if NETFX_CORE
		try {
			if(isConnectedToCommand || isConnectedToRegister) await EndConnection();
			
			commandSocket = new StreamSocket();
			await commandSocket.ConnectAsync(new HostName(ip), socketName);
			commandWriter = new DataWriter(commandSocket.OutputStream);
			commandReader = new DataReader(commandSocket.InputStream);

			isConnectedToCommand = true;

		} catch (Exception e) {
			onConnectFailure?.Invoke(
				"Fatal connection failure. Ensure the robot is turned on and connected to the same network as the HoloLens."
			);
		}
		
		if(isConnectedToCommand) {
			try {
				registerSocket = new StreamSocket();
				await registerSocket.ConnectAsync(new HostName(ip), "502");
				registerReader = new DataReader(registerSocket.InputStream);
				registerWriter = new DataWriter(registerSocket.OutputStream);

				isConnectedToRegister = true;
			} catch (Exception e) {
				onConnectFailure?.Invoke(
					"Partial connection failure. Commands can be sent, but feedback from the robot cannot be read. Operation under these conditions may result in undefined behavior."
				);
			}
		}
		

		//isConnectedToRegister = true;

		if(isConnectedToCommand && isConnectedToRegister) onConnectSuccess?.Invoke();
#endif
	}

	public async Task EndConnection() {
#if NETFX_CORE
		try {
			CancelQueuedMoves();

			isConnectedToCommand = false;
			await commandSocket.CancelIOAsync();
			commandSocket.Dispose();
			commandSocket = null;

			isConnectedToRegister = false;
			await registerSocket.CancelIOAsync();
			registerSocket.Dispose();
			registerSocket = null;

		} catch (Exception e) {

		}
#endif
	}

	public async Task SendCommand(string command) {
#if NETFX_CORE
		if(isConnectedToCommand) {
			try {
				commandWriter.WriteString(command);

				await commandWriter.StoreAsync();
				await commandWriter.FlushAsync();

				onCommandSendSuccess?.Invoke();

			} catch (Exception e) {
				onCommandSendFailure?.Invoke(e.Message);
			}
		}
#endif
	}

	public async Task SendRequest(byte[] request) {
#if NETFX_CORE
		if(isConnectedToRegister) {
			try {
				registerWriter.WriteBytes(request);

				await registerWriter.StoreAsync();
				await registerWriter.FlushAsync();
				
			} catch (Exception e) {
				OutputText.instance.text = OutputText.instance.text + e.Message + "\n" + e.StackTrace;
			}
		}
#endif
	} 

	public async Task RecieveRequestResponse() {
#if NETFX_CORE
		if(isConnectedToRegister) {
			try {
				await registerReader.LoadAsync(6);
				if(registerReader.ReadUInt16() != 4 || registerReader.ReadUInt16() != 0) {
					throw new InvalidDataException("Invalid request response.");
				}		

				ushort nBytes = (ushort)(registerReader.ReadUInt16());
				await registerReader.LoadAsync(nBytes);

				registerReader.ReadByte();
				nBytes--;

				for(ushort i = 0; i < nBytes; i++) {
					fetchBuffer[i] = registerReader.ReadByte();
				}

				fetchBufferLength = nBytes;
				
			} catch (Exception e) {
				OutputText.instance.text = OutputText.instance.text + e.Message + "\n" + e.StackTrace;
			}
		}
#endif
	}

	public void QueueMove(IMoveCommand command) {
		moveQueue.Enqueue(command);
	}

	public async Task MoveNow(IMoveCommand command) {
		await SendCommand(command.ToURCommand());
	}

	public void CancelQueuedMoves() {
		moveQueue.Clear();
	}

	public async Task FetchRealPose() {
		byte[] fetchRequest = {
			0x00, 0x04,
			0x00, 0x00,
			0x00, 0x06,
			0x00,
			0x03,
			0x01, 0x90,
			0x00, 0x06
		};

		while (isFetching) ;
		isFetching = true;
		await SendRequest(fetchRequest);
		await RecieveRequestResponse();
		isFetching = false;
	}

	public async Task FetchRealJointSpeeds() {
		byte[] fetchRequest = {
			0x00, 0x04,
			0x00, 0x00,
			0x00, 0x06,
			0x00,
			0x03,
			0x01, 0x18,
			0x00, 0x06
		};

		while (isFetching) ;
		isFetching = true;
		await SendRequest(fetchRequest);
		await RecieveRequestResponse();
		isFetching = false;
	}

	public bool GetFetchedRealPose(out Vector3 position, out Quaternion rotation) {
		if(fetchBufferLength == 0) {
			position = new Vector3();
			rotation = Quaternion.identity;
			return false;
		} else {
			if (fetchBufferLength != 2 + 12 || fetchBuffer[0] != 0x03 || fetchBuffer[1] != 12) {
				throw new InvalidDataException("Invalid MODBUS response. Dump: " +  fetchBufferLength.ToString() + " " + fetchBuffer[0].ToString() + " " + fetchBuffer[1].ToString());
			}

			short[] registerData = new short[6];
			for(uint i = 0; i < 6; i++) {
				registerData[i] = (short)((fetchBuffer[2 + i * 2] << 8) | fetchBuffer[2 + i * 2 + 1]);
			}

			// tenth of millimeter to meter
			position = new Vector3(
				registerData[0],
				registerData[1],
				registerData[2]
			) / 10000.0f;

			// milliradian to degree
			Vector3 eulerAngles = new Vector3(
				registerData[3],
				registerData[4],
				registerData[5]
			) / 1000.0f * Mathf.Rad2Deg;

			rotation = Quaternion.Euler(eulerAngles);

			fetchBufferLength = 0;

			return true;
		}
	}

	public bool GetFetchedRealJointSpeeds(out float[] speeds) {
		if (fetchBufferLength == 0) {
			speeds = new float[6];
			return false;
		}
		else {
			if (fetchBufferLength != 2 + 12 || fetchBuffer[0] != 0x03 || fetchBuffer[1] != 12) {
				throw new InvalidDataException("Invalid MODBUS response. Dump: " + fetchBufferLength.ToString() + " " + fetchBuffer[0].ToString() + " " + fetchBuffer[1].ToString());
			}

			short[] registerData = new short[6];
			for (uint i = 0; i < 6; i++) {
				registerData[i] = (short)((fetchBuffer[2 + i * 2] << 8) | fetchBuffer[2 + i * 2 + 1]);
			}

			speeds = new float[6];
			for(uint i = 0; i < 6; i++) {
				speeds[i] = ((float)registerData[i]) / 1000.0f * Mathf.Rad2Deg;
			}

			fetchBufferLength = 0;

			return true;
		}
	}

	public interface IMoveCommand {
		string ToURCommand();
	}

	public class MoveToPoseCommand : IMoveCommand {
		public bool ensureLinearity = false;
		public Vector3 position;
		public Quaternion rotation;
		public float velocity = 1.0f;
		public float acceleration = 0.3f;

		public MoveToPoseCommand(Vector3 endpoint, Quaternion orientation) {
			position = endpoint;
			rotation = orientation;
		}

		public string ToURCommand() {
			Vector3 usedPosition = Quaternion.Euler(90, 0, 0) * position;
			usedPosition.x = -usedPosition.x;

			Vector3 axis;
			float angle;
			rotation.ToAngleAxis(out angle, out axis);

			axis = Quaternion.Euler(90, 0, 0) * axis;
			axis.y = -axis.y;
			axis.z = -axis.z;

			Vector3 usedRotation = axis * angle * Mathf.Deg2Rad;

			return
				(ensureLinearity) ? "movel" : "movej" 
				+ "("
					+ "p[" 
						+ usedPosition.x.ToString("F4") + ", "
						+ usedPosition.y.ToString("F4") + ", "
						+ usedPosition.z.ToString("F4") + ", "
						+ usedRotation.x.ToString("F4") + ", "
						+ usedRotation.y.ToString("F4") + ", "
						+ usedRotation.z.ToString("F4")
					+ "]"
					+ ", v=" + velocity.ToString("F4")
					//+ ", r=0.01"
				+ ")\n";
		}
	}

	public class MoveJointsCommand : IMoveCommand {
		public bool ensureLinearity = false;
		public float[] jointAngles = new float[6];
		public float velocity = 1.0f;
		public float acceleration = 0.3f;

		public MoveJointsCommand(float[] jointAngles) {
			this.jointAngles = jointAngles;
		}

		public string ToURCommand() {
			return
				(ensureLinearity) ? "movel" : "movej"
				+ "("
					+ "["
						+ (jointAngles[0]).ToString("F4") + ", "
						+ (jointAngles[1]).ToString("F4") + ", "
						+ (jointAngles[2]).ToString("F4") + ", "
						+ (jointAngles[3]).ToString("F4") + ", "
						+ (jointAngles[4]).ToString("F4") + ", "
						+ (jointAngles[5]).ToString("F4")
					+ "]"
					+ ", v=" + velocity.ToString("F4")
				+ ")\n";
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
