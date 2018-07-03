using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using UnityEngine.EventSystems;

public class HoloInputManager : MonoBehaviour {
	public static HoloInputManager instance { get; private set; }

	public enum GestureCaptureType {
		none,
		manipulation,
		navigation
	}

	public GestureCaptureType captureWhat { get; private set; }

	public bool startRecognizeThisFrame { get; private set; }
	public bool stopRecognizeThisFrame { get; private set; }
	public bool isRecognized { get; private set; }

	public bool clickThisFrame { get; private set; }
	public bool startHoldThisFrame { get; private set; }
	public bool stopHoldThisFrame { get; private set; }
	public bool cancelHoldThisFrame { get; private set; }
	public bool isHolding { get; private set; }

	public bool startManipulationThisFrame { get; private set; }
	public bool updateManipulationThisFrame { get; private set; }
	public bool stopManipulationThisFrame { get; private set; }
	public bool cancelManipulationThisFrame { get; private set; }
	public bool isManipulating { get; private set; }

	public Vector3 manipulationFrameDelta { get; private set; }
	public Vector3 manipulationFullDelta { get; private set; }

	public bool startNavigationThisFrame { get; private set; }
	public bool updateNavigationThisFrame { get; private set; }
	public bool stopNavigationThisFrame { get; private set; }
	public bool cancelNavigationThisFrame { get; private set; }
	public bool isNavigating { get; private set; }

	public Vector3 navigationFrameDelta { get; private set; }
	public Vector3 navigationFullDelta { get; private set; }

	GestureRecognizer holdRecognizer;
	GestureRecognizer manipulationRecognizer;
	GestureRecognizer navigationRecognizer;
	InteractionManager interactionManager;

	private int nPastHandPositions = 10;
	private List<Vector3> pastHandPositions;

	public Vector3 handPosition { get; private set; }
	public Vector3 handPositionFrameDelta { get; private set; }

	void Start() {
		instance = this;

		pastHandPositions = new List<Vector3>(nPastHandPositions);
		for (int i = 0; i < nPastHandPositions; i++) {
			pastHandPositions.Add(new Vector3());
		}

		startRecognizeThisFrame = false;
		stopRecognizeThisFrame = false;
		isRecognized = false;

		clickThisFrame = false;
		startHoldThisFrame = false;
		stopHoldThisFrame = false;
		cancelHoldThisFrame = false;
		isHolding = false;

		startManipulationThisFrame = false;
		updateManipulationThisFrame = false;
		stopManipulationThisFrame = false;
		cancelManipulationThisFrame = false;
		isManipulating = false;

		manipulationFrameDelta = new Vector3(0, 0, 0);
		manipulationFullDelta = new Vector3(0, 0, 0);

		startNavigationThisFrame = false;
		updateNavigationThisFrame = false;
		stopNavigationThisFrame = false;
		cancelNavigationThisFrame = false;
		isNavigating = false;

		navigationFrameDelta = new Vector3(0, 0, 0);
		navigationFullDelta = new Vector3(0, 0, 0);

		holdRecognizer = new GestureRecognizer();
		holdRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold);
		holdRecognizer.Tapped += OnClick;
		holdRecognizer.HoldStarted += OnHoldStart;
		holdRecognizer.HoldCompleted += OnHoldStop;
		holdRecognizer.HoldCanceled += OnHoldCancel;

		manipulationRecognizer = new GestureRecognizer();
		manipulationRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold | GestureSettings.ManipulationTranslate);
		manipulationRecognizer.Tapped += OnClick;
		manipulationRecognizer.HoldStarted += OnHoldStart;
		manipulationRecognizer.HoldCompleted += OnHoldStop;
		manipulationRecognizer.HoldCanceled += OnHoldCancel;
		manipulationRecognizer.ManipulationStarted += OnManipulationStart;
		manipulationRecognizer.ManipulationUpdated += OnManipulationUpdate;
		manipulationRecognizer.ManipulationCompleted += OnManipulationComplete;
		manipulationRecognizer.ManipulationCanceled += OnManipulationCancel;

		navigationRecognizer = new GestureRecognizer();
		navigationRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.Hold | GestureSettings.NavigationX | GestureSettings.NavigationY);
		navigationRecognizer.Tapped += OnClick;
		navigationRecognizer.HoldStarted += OnHoldStart;
		navigationRecognizer.HoldCompleted += OnHoldStop;
		navigationRecognizer.HoldCanceled += OnHoldCancel;
		navigationRecognizer.NavigationStarted += OnNavigationStart;
		navigationRecognizer.NavigationUpdated += OnNavigationUpdate;
		navigationRecognizer.NavigationCompleted += OnNavigationComplete;
		navigationRecognizer.NavigationCanceled += OnNavigationCancel;

		InteractionManager.InteractionSourceDetected += OnStartRecognize;
		InteractionManager.InteractionSourceLost += OnStopRecognize;
	}

	public void ChangeCaptureType(GestureCaptureType type) {
		switch (captureWhat) {
			case GestureCaptureType.none:
				if(holdRecognizer.IsCapturingGestures()) holdRecognizer.StopCapturingGestures();
				break;
			case GestureCaptureType.manipulation:
				if(manipulationRecognizer.IsCapturingGestures()) manipulationRecognizer.StopCapturingGestures();
				break;
			case GestureCaptureType.navigation:
				if(navigationRecognizer.IsCapturingGestures()) navigationRecognizer.StopCapturingGestures();
				break;
		}

		switch (type) {
			case GestureCaptureType.none:
				holdRecognizer.StartCapturingGestures();
				break;
			case GestureCaptureType.manipulation:
				manipulationRecognizer.StartCapturingGestures();
				break;
			case GestureCaptureType.navigation:
				navigationRecognizer.StartCapturingGestures();
				break;
		}

		captureWhat = type;
	}

	private void OnClick(TappedEventArgs obj) {
		clickThisFrame = true;
	}

	private void OnStartRecognize(InteractionSourceDetectedEventArgs obj) {
		startRecognizeThisFrame = true;
		isRecognized = true;
	}

	private void OnStopRecognize(InteractionSourceLostEventArgs obj) {
		stopRecognizeThisFrame = true;
		isRecognized = false;
	}

	private void OnHoldStart(HoldStartedEventArgs obj) {
		startHoldThisFrame = true;
		isHolding = true;
	}

	private void OnHoldStop(HoldCompletedEventArgs obj) {
		stopHoldThisFrame = true;
		isHolding = false;
	}

	private void OnHoldCancel(HoldCanceledEventArgs obj) {
		cancelHoldThisFrame = true;
		isHolding = false;
	}

	private void OnManipulationStart(ManipulationStartedEventArgs obj) {
		startManipulationThisFrame = true;
		isManipulating = true;
	}

	private void OnManipulationUpdate(ManipulationUpdatedEventArgs obj) {
		updateManipulationThisFrame = true;

		manipulationFrameDelta = obj.cumulativeDelta - manipulationFullDelta;
		manipulationFullDelta = obj.cumulativeDelta;
	}

	private void OnManipulationComplete(ManipulationCompletedEventArgs obj) {
		stopManipulationThisFrame = true;
		isManipulating = false;

		manipulationFrameDelta = new Vector3(0, 0, 0);
		manipulationFullDelta = new Vector3(0, 0, 0);
	}

	private void OnManipulationCancel(ManipulationCanceledEventArgs obj) {
		cancelManipulationThisFrame = true;
		isManipulating = false;

		manipulationFrameDelta = new Vector3(0, 0, 0);
		manipulationFullDelta = new Vector3(0, 0, 0);
	}

	private void OnNavigationStart(NavigationStartedEventArgs obj) {
		startNavigationThisFrame = true;
		isNavigating = true;
	}

	private void OnNavigationUpdate(NavigationUpdatedEventArgs obj) {
		updateNavigationThisFrame = true;

		navigationFrameDelta = obj.normalizedOffset - navigationFullDelta;
		navigationFullDelta = obj.normalizedOffset;
	}

	private void OnNavigationComplete(NavigationCompletedEventArgs obj) {
		stopNavigationThisFrame = true;
		isNavigating = false;

		navigationFrameDelta = new Vector3(0, 0, 0);
		navigationFullDelta = new Vector3(0, 0, 0);
	}

	private void OnNavigationCancel(NavigationCanceledEventArgs obj) {
		cancelNavigationThisFrame = true;
		isNavigating = false;

		navigationFrameDelta = new Vector3(0, 0, 0);
		navigationFullDelta = new Vector3(0, 0, 0);
	}

	private void LateUpdate() {
		InteractionSourceState[] sources = InteractionManager.GetCurrentReading();

		if (sources.Length == 1) {
			Vector3 newHandPosition;
			Vector3 averageHandPosition = new Vector3();
			sources[0].sourcePose.TryGetPosition(out newHandPosition);

			if (startRecognizeThisFrame) {
				for (int i = 1; i < nPastHandPositions; i++) pastHandPositions[i] = newHandPosition;
			}
			else {
				for (int i = 1; i < nPastHandPositions; i++) pastHandPositions[i] = pastHandPositions[i - 1];
			}
			pastHandPositions[0] = newHandPosition;

			for (int i = 0; i < nPastHandPositions; i++) averageHandPosition += pastHandPositions[i];
			averageHandPosition /= nPastHandPositions;

			if (!startRecognizeThisFrame) {
				handPositionFrameDelta = averageHandPosition - handPosition;
			} else {
				handPositionFrameDelta = new Vector3(0, 0, 0);
			}

			handPosition = averageHandPosition;
		} else {
			handPositionFrameDelta = new Vector3(0, 0, 0);
		}

		startRecognizeThisFrame = false;
		stopRecognizeThisFrame = false;

		clickThisFrame = false;
		startHoldThisFrame = false;
		stopHoldThisFrame = false;
		cancelHoldThisFrame = false;

		startManipulationThisFrame = false;
		updateManipulationThisFrame = false;
		stopManipulationThisFrame = false;
		cancelManipulationThisFrame = false;
	}
}
