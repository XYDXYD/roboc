using UnityEngine;

namespace Mothership.GUI
{
	internal class DragAndDropView : MonoBehaviour
	{
		public Texture2D normalMouseCursor;

		public Texture2D hoveringDraggableItemMouseCursor;

		public Texture2D draggingItemMouseCursor;

		public Vector2 normalCursorHotSpot;

		public Vector2 hoveringDraggableItemHotSpot;

		public Vector2 draggingItemMouseHotSpot;

		private DragAndDropController _controller;

		public DragAndDropView()
			: this()
		{
		}

		private void Awake()
		{
			RestoreDefaultCursor();
		}

		private void OnDisable()
		{
			RestoreDefaultCursor();
		}

		public void Update()
		{
			switch (_controller.CurrentDragState)
			{
			case DragAndDropState.NotHovering:
				ShowNormalCursor();
				break;
			case DragAndDropState.HoveringDraggable:
				ShowHoverCursor();
				break;
			case DragAndDropState.DraggingItem:
				ShowGrabCursor();
				break;
			}
		}

		public void InjectController(DragAndDropController controller)
		{
			_controller = controller;
		}

		private void RestoreDefaultCursor()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Cursor.SetCursor(normalMouseCursor, normalCursorHotSpot, 0);
		}

		private void ShowNormalCursor()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Cursor.SetCursor(normalMouseCursor, normalCursorHotSpot, 0);
		}

		private void ShowHoverCursor()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Cursor.SetCursor(hoveringDraggableItemMouseCursor, hoveringDraggableItemHotSpot, 0);
		}

		private void ShowGrabCursor()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			Cursor.SetCursor(draggingItemMouseCursor, draggingItemMouseHotSpot, 0);
		}
	}
}
