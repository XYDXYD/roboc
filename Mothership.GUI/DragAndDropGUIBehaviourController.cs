using UnityEngine;

namespace Mothership.GUI
{
	internal class DragAndDropGUIBehaviourController : IDragAndDropGUIBehaviourController
	{
		private DragAndDropGUIEventObservable _observeable;

		public DragAndDropGUIBehaviourController(DragAndDropGUIEventObservable observeable)
		{
			_observeable = observeable;
		}

		public void HandleDragging(Vector2 offset)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			DragAndDropGUIMessage dragAndDropGUIMessage = new DragAndDropGUIMessage(DragAndDropGUIMessageType.Dragging, offset);
			_observeable.Dispatch(ref dragAndDropGUIMessage);
		}

		public void HandleDragStart(IProvideDragObjectBehaviour dragStartObjectProvider)
		{
			DragAndDropGUIMessage dragAndDropGUIMessage = new DragAndDropGUIMessage(DragAndDropGUIMessageType.StartDragging, new object[3]
			{
				dragStartObjectProvider.ProvideDragObjectData(),
				dragStartObjectProvider.ProvideDragObjectToShow(),
				dragStartObjectProvider.ProvideTargetToParentDragIconUnder()
			});
			_observeable.Dispatch(ref dragAndDropGUIMessage);
		}

		public void HandleDragEnd()
		{
			DragAndDropGUIMessage dragAndDropGUIMessage = new DragAndDropGUIMessage(DragAndDropGUIMessageType.DragEnd);
			_observeable.Dispatch(ref dragAndDropGUIMessage);
		}

		public void HandleDrop(ICanReceiveDropBehaviour receiverObject)
		{
			DragAndDropGUIMessage dragAndDropGUIMessage = new DragAndDropGUIMessage(DragAndDropGUIMessageType.Drop, receiverObject);
			_observeable.Dispatch(ref dragAndDropGUIMessage);
		}

		public void HandleHoverEnter(GameObject hoverTarget)
		{
			DragAndDropGUIMessage dragAndDropGUIMessage = new DragAndDropGUIMessage(DragAndDropGUIMessageType.StartHovering, hoverTarget);
			_observeable.Dispatch(ref dragAndDropGUIMessage);
		}

		public void HandleHoverExit()
		{
			DragAndDropGUIMessage dragAndDropGUIMessage = new DragAndDropGUIMessage(DragAndDropGUIMessageType.StoppedHovering);
			_observeable.Dispatch(ref dragAndDropGUIMessage);
		}
	}
}
