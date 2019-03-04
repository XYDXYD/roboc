using Svelto.ECS;
using UnityEngine;

namespace Mothership.TechTree
{
	internal interface ITechTreeViewScrollableComponent
	{
		UIScrollView ScrollView
		{
			get;
		}

		UICenterOnChild CenterOnChild
		{
			get;
		}

		SpringPanel SpringPanel
		{
			get;
		}

		DispatchOnSet<Vector2> DragDelta
		{
			get;
		}

		float ContentMargin
		{
			get;
		}
	}
}
