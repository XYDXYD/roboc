using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class BasicTooltipView : MonoBehaviour, ITooltipView, IView
	{
		[SerializeField]
		private UILabel label;

		[SerializeField]
		private UIWidget background;

		[SerializeField]
		private UIWidget arrow;

		public BasicTooltipView()
			: this()
		{
		}

		private void Awake()
		{
		}

		public void ShowTooltip(UIWidget referenceWidget, object content, GenericTooltipArea.Location preferredLocation)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			this.get_gameObject().SetActive(true);
			label.set_text((content == null) ? string.Empty : content.ToString());
			Vector3 position = 0.5f * (referenceWidget.get_worldCorners()[0] + referenceWidget.get_worldCorners()[3]);
			position.y -= 0.5f * Mathf.Abs(background.get_worldCorners()[0].y - background.get_worldCorners()[1].y);
			position.y -= Vector3.Distance(arrow.get_worldCorners()[1], arrow.get_worldCorners()[2]);
			this.get_transform().set_position(position);
			AdjustToFitScreen();
		}

		private void AdjustToFitScreen()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = this.get_transform().get_position();
			float num = (float)(-Screen.get_width()) / (float)Screen.get_height();
			float num2 = (float)Screen.get_width() / (float)Screen.get_height();
			float num3 = Mathf.Abs(background.get_worldCorners()[0].x - background.get_worldCorners()[3].x);
			float num4 = position.x - num3 / 2f;
			float num5 = position.x + num3 / 2f;
			label.get_transform().set_localPosition(new Vector3(0f, 0f, 0f));
			Vector3 position2 = label.get_transform().get_position();
			if (num4 < num)
			{
				position2.x += num - num4;
				label.get_transform().set_position(position2);
			}
			else if (num5 > num2)
			{
				position2.x -= num5 - num2;
				label.get_transform().set_position(position2);
			}
		}
	}
}
