using Mothership.GUI;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

internal sealed class CameraPreviewView : MonoBehaviour, IInitialize
{
	public UISliderMouseWheelScroller itemScrollBar;

	private CameraPreviewUtility.CameraConfiguration cameraConfig;

	[SerializeField]
	private float distanceFromTargetMinimum;

	[SerializeField]
	private float distanceFromTargetMaximum;

	[SerializeField]
	private float pitchMin;

	[SerializeField]
	private float pitchMax;

	[SerializeField]
	private float zoomSpeed;

	[SerializeField]
	private float draggingRotationSpeed;

	[SerializeField]
	private float fovNormalZoom;

	[SerializeField]
	private float fovMaximumZoom;

	[SerializeField]
	private float distanceToNearestWall;

	[SerializeField]
	private float zoomLevelToBeginHuggingWall;

	private Vector3 _lastMousePosition;

	private bool _isHoveringGarage;

	private bool _isDragAndDropSystemActive;

	[Inject]
	internal CameraPreview cameraPreview
	{
		private get;
		set;
	}

	[Inject]
	internal DragAndDropGUIEventObserver guiEventObserver
	{
		private get;
		set;
	}

	public CameraPreviewView()
		: this()
	{
	}

	unsafe void IInitialize.OnDependenciesInjected()
	{
		guiEventObserver.AddAction(new ObserverAction<DragAndDropGUIMessage>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private static bool IsGarageCollider(DragAndDropGUIMessage eventData)
	{
		GameObject val = eventData.Data as GameObject;
		return val != null && val.get_gameObject().get_tag() == "GarageCollider";
	}

	private void OnDragAndDropGUIEvent(ref DragAndDropGUIMessage eventData)
	{
		if (eventData.MessageType == DragAndDropGUIMessageType.StartHovering)
		{
			_isHoveringGarage = IsGarageCollider(eventData);
		}
		if (eventData.MessageType == DragAndDropGUIMessageType.StoppedHovering)
		{
			_isHoveringGarage = false;
		}
		if (eventData.MessageType == DragAndDropGUIMessageType.StartDragging)
		{
			_isDragAndDropSystemActive = true;
		}
		if (eventData.MessageType == DragAndDropGUIMessageType.DragEnd)
		{
			_isDragAndDropSystemActive = false;
		}
	}

	private void Start()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		cameraConfig = new CameraPreviewUtility.CameraConfiguration(distanceFromTargetMinimum, distanceFromTargetMaximum, pitchMin, pitchMax, zoomSpeed, draggingRotationSpeed, fovMaximumZoom, fovNormalZoom, distanceToNearestWall, zoomLevelToBeginHuggingWall);
		cameraPreview.SetView(this);
		cameraPreview.SetConfig(cameraConfig);
		_lastMousePosition = Input.get_mousePosition();
		_isHoveringGarage = false;
		_isDragAndDropSystemActive = false;
	}

	private void Update()
	{
		UpdateInputs();
	}

	public void UpdateInputs()
	{
		bool flag = Input.GetMouseButton(0) || Input.GetMouseButton(1);
		bool flag2 = flag;
		Camera component = UICamera.list.get_Item(0).GetComponent<Camera>();
		if (Object.op_Implicit(itemScrollBar))
		{
			itemScrollBar.topBorder = (_isHoveringGarage ? component.get_pixelHeight() : 0);
		}
		if (_isHoveringGarage && Input.GetAxis("Mouse ScrollWheel") != 0f)
		{
			cameraPreview.ZoomView(Input.GetAxis("Mouse ScrollWheel"));
		}
	}
}
