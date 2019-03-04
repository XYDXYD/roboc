using UnityEngine;

namespace Mothership.TechTree
{
	internal static class TechTreeViewUtility
	{
		public static void RestrictWithinBounds(IBoundsComponent boundsComponent, ITechTreeViewScrollableComponent scrollableComponent, ITechTreeZoomableComponent zoomComponent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = zoomComponent.TreeRoot.get_localScale();
			float x = localScale.x;
			float contentMargin = scrollableComponent.ContentMargin;
			Vector2 min = boundsComponent.BoundsMin * x;
			Vector2 max = boundsComponent.BoundsMax * x;
			min.x -= contentMargin;
			min.y -= contentMargin;
			max.x += contentMargin;
			max.y += contentMargin;
			RestrictWithinBounds(scrollableComponent.ScrollView, min, max);
		}

		private static void RestrictWithinBounds(UIScrollView scrollView, Vector2 min, Vector2 max)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = scrollView.get_panel().CalculateConstrainOffset(min, max);
			scrollView.MoveRelative(val);
		}
	}
}
