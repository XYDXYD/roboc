using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class DragAndDropController
	{
		private object _dragData;

		private GameObject _dragDisplayIcon;

		private GameObject _hoverItem;

		private DragAndDropState _currentDragState;

		private DragAndDropView _view;

		[Inject]
		internal DragAndDropGUIEventObserver dragAndDropEventObserver
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController guiInputController
		{
			get;
			set;
		}

		public DragAndDropState CurrentDragState => _currentDragState;

		public object CurrentDragItem => _dragData;

		public GameObject CurrentHoverItem => _hoverItem;

		public unsafe void Initialise(IContextNotifer contextNotifier)
		{
			_currentDragState = DragAndDropState.NotHovering;
			_dragData = null;
			_dragDisplayIcon = null;
			_hoverItem = null;
			dragAndDropEventObserver.AddAction(new ObserverAction<DragAndDropGUIMessage>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			guiInputController.OnScreenStateChange += HandleOnScreenStateChange;
		}

		private void HandleOnScreenStateChange()
		{
			if (UICamera.get_hoveredObject() != _hoverItem && _currentDragState == DragAndDropState.HoveringDraggable)
			{
				if (_hoverItem != null)
				{
					DragAndDropGUIBehaviourView component = _hoverItem.GetComponent<DragAndDropGUIBehaviourView>();
					if (component != null)
					{
						component.HandleOnHover(component.get_gameObject(), hoverState: false);
					}
				}
				if (UICamera.get_hoveredObject() != null)
				{
					HoverIfSomethingToHoverOnNewScreen();
				}
			}
			if (_currentDragState == DragAndDropState.DraggingItem)
			{
				_currentDragState = DragAndDropState.NotHovering;
				_dragData = null;
				HandleDragDisplayIconChanged(DragDisplayIconDesiredState.Hidden);
				if (UICamera.get_hoveredObject() != null)
				{
					_currentDragState = DragAndDropState.HoveringDraggable;
					HoverIfSomethingToHoverOnNewScreen();
				}
			}
		}

		private void HoverIfSomethingToHoverOnNewScreen()
		{
			DragAndDropGUIBehaviourView component = UICamera.get_hoveredObject().GetComponent<DragAndDropGUIBehaviourView>();
			if (component != null)
			{
				component.HandleOnHover(component.get_gameObject(), hoverState: true);
				_hoverItem = component.get_gameObject();
			}
		}

		private void HandleGUIEvent(ref DragAndDropGUIMessage dragAndDropEvent)
		{
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			switch (dragAndDropEvent.MessageType)
			{
			case DragAndDropGUIMessageType.StartHovering:
				_hoverItem = (dragAndDropEvent.Data as GameObject);
				if (_currentDragState == DragAndDropState.NotHovering)
				{
					_currentDragState = DragAndDropState.HoveringDraggable;
					IProvideDragObjectBehaviour component = _hoverItem.GetComponent<IProvideDragObjectBehaviour>();
					if (component != null && !component.HasSomethingToDrag())
					{
						_currentDragState = DragAndDropState.NotHovering;
					}
				}
				break;
			case DragAndDropGUIMessageType.StoppedHovering:
				if (_currentDragState == DragAndDropState.HoveringDraggable)
				{
					_currentDragState = DragAndDropState.NotHovering;
				}
				break;
			case DragAndDropGUIMessageType.Dragging:
				if (Object.op_Implicit(_dragDisplayIcon))
				{
					Vector2 val = (Vector2)dragAndDropEvent.Data;
					UIWidget component2 = _dragDisplayIcon.GetComponent<UIWidget>();
					UIWidget component3 = _dragDisplayIcon.get_transform().get_parent().GetComponent<UIWidget>();
					Bounds val2 = component3.CalculateBounds();
					Vector3 size = val2.get_size();
					int num = (int)size.x;
					Vector3 size2 = val2.get_size();
					int num2 = (int)size2.y;
					component2.SetAnchor(_dragDisplayIcon.get_transform().get_parent());
					component2.SetAnchor(0f, (int)val.x, 1f, (int)val.y - num2, 0f, (int)val.x + num, 1f, (int)val.y);
					component2.MarkAsChanged();
				}
				break;
			case DragAndDropGUIMessageType.StartDragging:
				if (_currentDragState == DragAndDropState.HoveringDraggable || _currentDragState == DragAndDropState.NotHovering)
				{
					object[] array = dragAndDropEvent.Data as object[];
					_dragData = array[0];
					_dragDisplayIcon = (array[1] as GameObject);
					GameObject val3 = array[2] as GameObject;
					if (val3 != null)
					{
						_dragDisplayIcon.get_transform().set_parent(val3.get_transform());
					}
					HandleDragDisplayIconChanged(DragDisplayIconDesiredState.Shown);
					_currentDragState = DragAndDropState.DraggingItem;
				}
				break;
			case DragAndDropGUIMessageType.DragEnd:
				if (_currentDragState == DragAndDropState.DraggingItem)
				{
					_currentDragState = DragAndDropState.NotHovering;
					HandleDragDisplayIconChanged(DragDisplayIconDesiredState.Hidden);
				}
				break;
			case DragAndDropGUIMessageType.Drop:
			{
				ICanReceiveDropBehaviour canReceiveDropBehaviour = dragAndDropEvent.Data as ICanReceiveDropBehaviour;
				if (canReceiveDropBehaviour.CanReceiveObject(_dragData))
				{
					canReceiveDropBehaviour.ReceiveDrop(_dragData);
				}
				_dragData = null;
				break;
			}
			}
		}

		public void SetView(DragAndDropView view)
		{
			_view = view;
		}

		private void HandleDragDisplayIconChanged(DragDisplayIconDesiredState desiredState)
		{
			if (desiredState == DragDisplayIconDesiredState.Shown && _dragDisplayIcon != null)
			{
				_dragDisplayIcon.SetActive(true);
			}
			if (desiredState == DragDisplayIconDesiredState.Hidden)
			{
				_dragDisplayIcon.SetActive(false);
				_dragDisplayIcon = null;
			}
		}
	}
}
