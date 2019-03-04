using Robocraft.GUI;
using UnityEngine;

namespace Mothership
{
	public class PlayerListEntryHeaderView : GenericExpandeableListEntryViewBase
	{
		[SerializeField]
		private UILabel HeaderText;

		[SerializeField]
		private Color32[] Colors;

		[SerializeField]
		private UISprite[] ColorableSpriteElements;

		[SerializeField]
		private UIButton ExpandButton;

		[SerializeField]
		private UIButton CollapseButton;

		public override void Default()
		{
			base.Default();
			HeaderText.set_text(string.Empty);
		}

		public override void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == string.Empty && genericComponentMessage.Message == MessageType.ButtonClicked)
				{
					genericComponentMessage.Consume();
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, "HeaderListButton", new ListItemComponentDataContainer(this)));
				}
			}
		}

		public override void UpdateData(object data)
		{
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			if ((data as ListHeaderInfo).ExpandedStatus)
			{
				ExpandButton.get_gameObject().SetActive(false);
				CollapseButton.get_gameObject().SetActive(true);
			}
			else
			{
				ExpandButton.get_gameObject().SetActive(true);
				CollapseButton.get_gameObject().SetActive(false);
			}
			HeaderText.set_text((data as ListHeaderInfo).HeaderName);
			int colorIndex = (data as ListHeaderInfo).ColorIndex;
			if (colorIndex < Colors.Length)
			{
				HeaderText.set_color(Color32.op_Implicit(Colors[colorIndex]));
				for (int i = 0; i < ColorableSpriteElements.Length; i++)
				{
					ColorableSpriteElements[i].set_color(Color32.op_Implicit(Colors[colorIndex]));
				}
				ExpandButton.set_defaultColor(Color32.op_Implicit(Colors[colorIndex]));
				CollapseButton.set_defaultColor(Color32.op_Implicit(Colors[colorIndex]));
			}
		}
	}
}
