using UnityEngine;

namespace Mothership
{
	internal static class HalfScreenGUIHelper
	{
		public static void Show(GUIPosition position, UIWidget guiWidget)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (position == GUIPosition.Right)
			{
				Rect rect = Camera.get_main().get_rect();
				num = rect.get_x() - (float)guiWidget.get_width() / (float)Screen.get_width();
			}
			else
			{
				Rect rect2 = Camera.get_main().get_rect();
				num = rect2.get_x() + (float)guiWidget.get_width() / (float)Screen.get_width();
			}
			Rect rect3 = Camera.get_main().get_rect();
			rect3.set_x(num);
			rect3.set_width(1f);
			Camera.get_main().set_rect(rect3);
		}

		public static void ExactlyHalfScreenCameraOnRHS()
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			Rect rect = Camera.get_main().get_rect();
			rect.set_x(0.5f);
			rect.set_width(1f);
			Camera.get_main().set_rect(rect);
		}

		public static void Hide()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Camera.get_main().set_rect(new Rect(0f, 0f, (float)Screen.get_width(), (float)Screen.get_height()));
		}
	}
}
