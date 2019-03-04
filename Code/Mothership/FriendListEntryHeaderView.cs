using Robocraft.GUI;
using UnityEngine;

namespace Mothership
{
	public class FriendListEntryHeaderView : GenericExpandeableListEntryViewBase
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
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, "HeaderListButton", new FriendListItemComponentDataContainer(this, null, "HeaderListButton")));
				}
			}
		}

		public override void UpdateData(object data)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			ListHeaderInfo listHeaderInfo = data as ListHeaderInfo;
			HeaderText.set_text(listHeaderInfo.HeaderName);
			if (listHeaderInfo.ColorIndex < Colors.Length)
			{
				HeaderText.set_color(Color32.op_Implicit(Colors[listHeaderInfo.ColorIndex]));
				for (int i = 0; i < ColorableSpriteElements.Length; i++)
				{
					ColorableSpriteElements[i].set_color(Color32.op_Implicit(Colors[listHeaderInfo.ColorIndex]));
				}
				ExpandButton.set_defaultColor(Color32.op_Implicit(Colors[listHeaderInfo.ColorIndex]));
				CollapseButton.set_defaultColor(Color32.op_Implicit(Colors[listHeaderInfo.ColorIndex]));
			}
			if (listHeaderInfo.ExpandedStatus)
			{
				ExpandButton.get_gameObject().SetActive(false);
				CollapseButton.get_gameObject().SetActive(true);
			}
			else
			{
				ExpandButton.get_gameObject().SetActive(true);
				CollapseButton.get_gameObject().SetActive(false);
			}
		}
	}
}
