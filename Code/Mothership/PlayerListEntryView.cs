using Authentication;
using Mothership.GUI.Social;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	public class PlayerListEntryView : GenericExpandeableListEntryViewBase, IContextMenuRoot
	{
		[SerializeField]
		private UILabel PlayerName;

		[SerializeField]
		private UILabel PlayerInfo;

		[SerializeField]
		private UITexture AvatarTexture;

		[SerializeField]
		private UITexture ClanAvatarTexture;

		[SerializeField]
		private GameObject InviteToPartyButton;

		[SerializeField]
		private Color MyNameColor = Color.get_cyan();

		[SerializeField]
		private Color DefaultNameColor = Color.get_yellow();

		[SerializeField]
		private UILabel seasonMemberExperience;

		private string _PlayerName;

		private const int MAX_CLAN_MEMBERS = 50;

		private UIButtonColor _playerNameButtonColor;

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
			PlayerInfo.set_text(string.Empty);
			PlayerName.set_text(string.Empty);
			_PlayerName = string.Empty;
		}

		public override void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == string.Empty && genericComponentMessage.Message == MessageType.ButtonClicked)
				{
					genericComponentMessage.Consume();
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, genericComponentMessage.Originator, new PlayerListItemComponentDataContainer(this, _PlayerName, PlayerName.get_text())));
				}
			}
		}

		private void SetPlayerNameNormalColor(Color color)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			PlayerName.set_color(color);
			if (_playerNameButtonColor == null)
			{
				UIButtonColor[] components = this.GetComponents<UIButtonColor>();
				foreach (UIButtonColor val in components)
				{
					if (val.tweenTarget == PlayerName.get_gameObject())
					{
						_playerNameButtonColor = val;
						break;
					}
				}
			}
			if (_playerNameButtonColor != null)
			{
				_playerNameButtonColor.set_defaultColor(color);
			}
		}

		public override void UpdateData(object data)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			ClanMemberWithContextInfo clanMemberWithContextInfo = data as ClanMemberWithContextInfo;
			seasonMemberExperience.set_text(clanMemberWithContextInfo.Member.SeasonXP.ToString());
			PlayerName.set_text(clanMemberWithContextInfo.Member.DisplayName);
			_PlayerName = clanMemberWithContextInfo.Member.Name;
			if (clanMemberWithContextInfo.Member.Name.Equals(User.Username))
			{
				SetPlayerNameNormalColor(MyNameColor);
			}
			else
			{
				SetPlayerNameNormalColor(DefaultNameColor);
			}
			if (clanMemberWithContextInfo.Member.ClanMemberState == ClanMemberState.Accepted)
			{
				switch (clanMemberWithContextInfo.Member.ClanMemberRank)
				{
				case ClanMemberRank.Leader:
					PlayerInfo.set_text(StringTableBase<StringTable>.Instance.GetString("strClanLeader"));
					break;
				case ClanMemberRank.Member:
					PlayerInfo.set_text(StringTableBase<StringTable>.Instance.GetString("strClanMember"));
					break;
				case ClanMemberRank.Officer:
					PlayerInfo.set_text(StringTableBase<StringTable>.Instance.GetString("strClanOfficer"));
					break;
				}
			}
			else
			{
				PlayerInfo.set_text(StringTableBase<StringTable>.Instance.GetString("strClanInvitee"));
			}
			InviteToPartyButton.SetActive(clanMemberWithContextInfo.CanBeInvitedToParty);
			if (clanMemberWithContextInfo.Member.AvatarInfo.UseCustomAvatar)
			{
				AvatarTexture.set_mainTexture(clanMemberWithContextInfo.PlayerAvatarTexture);
			}
			else
			{
				AvatarTexture.set_mainTexture(PresetAvatarMapProvider.GetPresetAvatar(clanMemberWithContextInfo.Member.AvatarInfo.AvatarId));
			}
		}
	}
}
