using ExtensionMethods;
using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	public class FriendListEntryView : GenericExpandeableListEntryViewBase, IContextMenuRoot
	{
		[SerializeField]
		private UILabel PlayerName;

		[SerializeField]
		private UILabel ClanName;

		[SerializeField]
		private UILabel PlayerInfo;

		[SerializeField]
		private UITexture AvatarTexture;

		[SerializeField]
		private UITexture ClanAvatarTexture;

		[SerializeField]
		private UIButton LeftTickButton;

		[SerializeField]
		private UIButton LeftCrossButton;

		[SerializeField]
		private UIButton LeftPlusButton;

		[SerializeField]
		private UIButton RightTickButton;

		[SerializeField]
		private UIButton RightCrossButton;

		[SerializeField]
		private UIButton RightPlusButton;

		[SerializeField]
		private UIButton ClanAvatarGO;

		[SerializeField]
		private UIButton InfoTextButtonComponent;

		[SerializeField]
		private Color32[] Colors;

		private Friend _friend;

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		public override void Setup()
		{
			base.Setup();
		}

		public override void Default()
		{
			base.Default();
			PlayerName.set_text(string.Empty);
			ClanName.set_text(string.Empty);
			PlayerInfo.set_text(string.Empty);
			LeftTickButton.get_gameObject().SetActive(false);
			LeftCrossButton.get_gameObject().SetActive(false);
			LeftPlusButton.get_gameObject().SetActive(false);
			RightTickButton.get_gameObject().SetActive(false);
			RightCrossButton.get_gameObject().SetActive(false);
			RightPlusButton.get_gameObject().SetActive(false);
		}

		public override void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == string.Empty && genericComponentMessage.Message == MessageType.ButtonClicked)
				{
					genericComponentMessage.Consume();
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, "FriendListButton", new FriendListItemComponentDataContainer(this, _friend, genericComponentMessage.Originator)));
				}
			}
		}

		public override void UpdateData(object data)
		{
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			Default();
			FriendListEntryData friendListEntryData = data as FriendListEntryData;
			_friend = friendListEntryData.Friend;
			PlayerName.set_text(friendListEntryData.DisplayName);
			string text = friendListEntryData.ClanName;
			if (text.IsNullOrWhiteSpace())
			{
				text = string.Empty;
			}
			ClanName.set_text(text);
			ClanName.UpdateNGUIText();
			int num = NGUIText.CalculateOffsetToFit(text);
			if (num > 0)
			{
				int num2 = text.Length - num - 3;
				if (num2 < 0)
				{
					num2 = 0;
				}
				ClanName.set_text($"{text.Substring(0, num2)}...");
			}
			PlayerInfo.set_text(friendListEntryData.StatusText);
			if (Colors.Length >= friendListEntryData.StatusColourIndex)
			{
				InfoTextButtonComponent.set_defaultColor(Color32.op_Implicit(Colors[friendListEntryData.StatusColourIndex]));
			}
			if (friendListEntryData.Avatar.UseCustomAvatar)
			{
				AvatarTexture.set_mainTexture(friendListEntryData.FriendsPlayerAvatar);
			}
			else
			{
				AvatarTexture.set_mainTexture(PresetAvatarMapProvider.GetPresetAvatar(friendListEntryData.Avatar.AvatarId));
			}
			ClanAvatarGO.get_gameObject().SetActive(false);
			switch (friendListEntryData.LeftButton)
			{
			case FriendListEntryData.FriendListButtonTypes.TICK:
				LeftTickButton.get_gameObject().SetActive(true);
				break;
			case FriendListEntryData.FriendListButtonTypes.CROSS:
				LeftCrossButton.get_gameObject().SetActive(true);
				break;
			case FriendListEntryData.FriendListButtonTypes.PLUS:
				LeftPlusButton.get_gameObject().SetActive(true);
				break;
			}
			switch (friendListEntryData.RightButton)
			{
			case FriendListEntryData.FriendListButtonTypes.TICK:
				RightTickButton.get_gameObject().SetActive(true);
				break;
			case FriendListEntryData.FriendListButtonTypes.CROSS:
				RightCrossButton.get_gameObject().SetActive(true);
				break;
			case FriendListEntryData.FriendListButtonTypes.PLUS:
				RightPlusButton.get_gameObject().SetActive(true);
				break;
			}
		}
	}
}
