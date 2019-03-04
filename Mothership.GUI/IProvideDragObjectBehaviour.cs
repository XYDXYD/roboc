using UnityEngine;

namespace Mothership.GUI
{
	internal interface IProvideDragObjectBehaviour
	{
		object ProvideDragObjectData();

		bool HasSomethingToDrag();

		GameObject ProvideDragObjectToShow();

		GameObject ProvideTargetToParentDragIconUnder();
	}
}
