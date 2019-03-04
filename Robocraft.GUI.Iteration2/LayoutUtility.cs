using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal static class LayoutUtility
	{
		public static void CheckIsLayout(UIWidgetContainer c)
		{
		}

		public static void ScheduleReposition(UIWidgetContainer c)
		{
			Reposition(c, now: false);
		}

		public static void Reposition(UIWidgetContainer c, bool now)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			CheckIsLayout(c);
			if (c is UITable)
			{
				if (now)
				{
					c.Reposition();
				}
				else
				{
					c.set_repositionNow(true);
				}
			}
			else if (c is UIGrid)
			{
				if (now)
				{
					c.Reposition();
				}
				else
				{
					c.set_repositionNow(true);
				}
			}
			UIScrollView component = c.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.ResetPosition();
			}
		}

		public static void KeepWidgetOnScreen(UIWidget widget)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			Transform transform = widget.get_transform();
			Vector3 position = transform.get_position();
			float num = (float)(-Screen.get_width()) / (float)Screen.get_height();
			float num2 = (float)Screen.get_width() / (float)Screen.get_height();
			float num3 = Mathf.Abs(widget.get_worldCorners()[0].x - widget.get_worldCorners()[3].x);
			Vector2 pivotOffset = widget.get_pivotOffset();
			float num4 = position.x - num3 * pivotOffset.x;
			float num5 = position.x + num3 * (1f - pivotOffset.x);
			if (num4 < num)
			{
				position.x += num - num4;
				transform.set_position(position);
			}
			else if (num5 > num2)
			{
				position.x -= num5 - num2;
				transform.set_position(position);
			}
		}

		public static void KeepWidgetOnTopOfParentAndInScreen(UIWidget parentWidget, UIWidget widget)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = parentWidget.get_worldCorners()[0];
			float num = widget.get_worldCorners()[2].x - widget.get_worldCorners()[1].x;
			float num2 = widget.get_worldCorners()[1].y - widget.get_worldCorners()[0].y;
			float num3 = (float)Screen.get_width() / (float)Screen.get_height();
			float num4 = (float)(-Screen.get_height()) / (float)Screen.get_width();
			if (position.x + num > num3)
			{
				position.x = parentWidget.get_worldCorners()[2].x - num;
			}
			if (position.y - num2 < num4)
			{
				position.y = parentWidget.get_worldCorners()[2].y + num2;
			}
			widget.get_transform().set_position(position);
		}
	}
}
