using Mothership.GUI;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

internal sealed class CameraDraggingView : MonoBehaviour, IInitialize
{
	private Vector3 _lastMousePosition;

	private bool _isHoveringGarage;

	private bool _isDragAndDropSystemActive;

	[Inject]
	internal GarageCameraOrientationController orientationController
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

	public CameraDraggingView()
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		orientationController.SetView(this);
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
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Input.GetMouseButton(0) || Input.GetMouseButton(1);
		bool flag2 = flag;
		Camera component = UICamera.list.get_Item(0).GetComponent<Camera>();
		if (_isHoveringGarage && flag2 && !_isDragAndDropSystemActive)
		{
			orientationController.OnUserPanCamera(Vector2.op_Implicit(Input.get_mousePosition() - _lastMousePosition));
		}
		_lastMousePosition = Input.get_mousePosition();
	}
}
