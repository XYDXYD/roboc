using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Mothership
{
	public class InviteesListEntryView : GenericExpandeableListEntryViewBase
	{
		[SerializeField]
		private UILabel PlayerName;

		[SerializeField]
		private UILabel PlayerClan;

		[SerializeField]
		private UITexture ClanAvatarTexture;

		[SerializeField]
		private UITexture PlayerAvatarTexture;

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		[Inject]
		internal IMultiAvatarLoader AvatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver AvatarAvailableObserver
		{
			private get;
			set;
		}

		public unsafe override void Setup()
		{
			base.Setup();
			AvatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public override void Show()
		{
			base.Show();
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
		{
			if (PlayerName.get_text() == avatarData.avatarName && avatarData.avatarType == AvatarType.PlayerAvatar)
			{
				PlayerAvatarTexture.set_mainTexture(avatarData.texture);
			}
			if (avatarData.avatarType == AvatarType.ClanAvatar && avatarData.avatarName == PlayerClan.get_text())
			{
				ClanAvatarTexture.set_mainTexture(avatarData.texture);
			}
		}

		public override void Default()
		{
			base.Default();
			PlayerClan.set_text(string.Empty);
			PlayerName.set_text(string.Empty);
			ClanAvatarTexture.set_mainTexture(null);
			PlayerAvatarTexture.set_mainTexture(null);
		}

		public override void Listen(object message)
		{
			if (!(message is GenericComponentMessage))
			{
				return;
			}
			GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
			if (genericComponentMessage.Target == string.Empty && genericComponentMessage.Message == MessageType.ButtonClicked)
			{
				genericComponentMessage.Consume();
				if (genericComponentMessage.Originator == "decline")
				{
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, "InviteeList", new InviteeListItemComponentDataContainer(PlayerName.get_text(), PlayerClan.get_text(), InviteeListItemComponentDataContainer.ListItemAction.Decline, this)));
				}
				if (genericComponentMessage.Originator == "accept")
				{
					BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, "InviteeList", new InviteeListItemComponentDataContainer(PlayerName.get_text(), PlayerClan.get_text(), InviteeListItemComponentDataContainer.ListItemAction.Accept, this)));
				}
			}
		}

		public override void UpdateData(object data)
		{
			ClanInvite clanInvite = data as ClanInvite;
			PlayerName.set_text(clanInvite.InviterDisplayName);
			PlayerClan.set_text(clanInvite.ClanName);
			if (!string.IsNullOrEmpty(clanInvite.ClanName))
			{
				AvatarLoader.RequestAvatar(AvatarType.ClanAvatar, clanInvite.ClanName);
			}
			if (clanInvite.InviterAvatarInfo.UseCustomAvatar)
			{
				AvatarLoader.RequestAvatar(AvatarType.PlayerAvatar, clanInvite.InviterName);
			}
			else
			{
				PlayerAvatarTexture.set_mainTexture(PresetAvatarMapProvider.GetPresetAvatar(clanInvite.InviterAvatarInfo.AvatarId));
			}
		}
	}
}
