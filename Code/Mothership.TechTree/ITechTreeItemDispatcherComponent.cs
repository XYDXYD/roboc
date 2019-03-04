using Svelto.ECS;
using UnityEngine;

namespace Mothership.TechTree
{
	internal interface ITechTreeItemDispatcherComponent
	{
		DispatchOnChange<bool> IsHover
		{
			get;
		}

		DispatchOnSet<bool> IsClicked
		{
			get;
		}

		DispatchOnSet<Vector2> DragDelta
		{
			get;
		}
	}
}
