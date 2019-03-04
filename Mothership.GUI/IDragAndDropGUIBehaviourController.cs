using UnityEngine;

namespace Mothership.GUI
{
	internal interface IDragAndDropGUIBehaviourController
	{
		void HandleDragStart(IProvideDragObjectBehaviour dragStartObjectProvider);

		void HandleDragEnd();

		void HandleDrop(ICanReceiveDropBehaviour receiverObject);

		void HandleHoverEnter(GameObject target);

		void HandleHoverExit();

		void HandleDragging(Vector2 offset);
	}
}
