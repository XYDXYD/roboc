using Avatars;
using Robocraft.GUI;
using Robocraft.GUI.Iteration2;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Mothership.GUI.Party
{
	internal class InvitablePlayerItemPresenter : IItemPresenter, IDataPresenter, IPresenter
	{
		public const string INVITE_BUTTON = "InvitablePlayerItemButton";

		private IDataSource _dataSource;

		private InvitablePlayerItemView _view;

		private int _itemIndex;

		private AvatarInfo _friendAvatarInfo;

		private string _accountName;

		[Inject]
		internal IMultiAvatarLoader avatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver availableAvatarObserver
		{
			private get;
			set;
		}

		[Inject]
		internal PresetAvatarMapProvider presetAvatarMapProvider
		{
			private get;
			set;
		}

		public unsafe void SetView(InvitablePlayerItemView view)
		{
			_view = view;
			availableAvatarObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
		{
			if (_accountName != null && avatarData.avatarName == _accountName && avatarData.avatarType == AvatarType.PlayerAvatar)
			{
				_view.playerAvatar.set_mainTexture(avatarData.texture);
			}
		}

		public void Listen(object obj)
		{
			if (obj is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = (GenericComponentMessage)obj;
				if (genericComponentMessage.Message == MessageType.ButtonClicked && genericComponentMessage.Originator == "InvitablePlayerItemButton")
				{
					TriggerInvite();
				}
			}
		}

		private void TriggerInvite()
		{
			string playerName = null;
			bool invitable = false;
			string displayName = null;
			RetrieveData(out playerName, out displayName, out invitable);
			_view.Bubble(new InvitePlayerMessage(displayName));
			_view.inviteButton.SetActive(false);
		}

		public void SetActive(bool active)
		{
			_view.get_gameObject().SetActive(active);
		}

		public void SetDataSource(IDataSource ds)
		{
			_dataSource = ds;
		}

		public void SetDataSourceIndex(int index)
		{
			_itemIndex = index;
		}

		public void UpdateFromSource()
		{
			string playerName = null;
			bool invitable = false;
			string displayName = null;
			RetrieveData(out playerName, out displayName, out invitable);
			_view.playerName.set_text(displayName);
			_view.inviteButton.SetActive(invitable);
			_accountName = playerName;
			if (_friendAvatarInfo.UseCustomAvatar)
			{
				avatarLoader.RequestAvatar(AvatarType.PlayerAvatar, playerName);
			}
			else
			{
				_view.playerAvatar.set_mainTexture(presetAvatarMapProvider.GetPresetAvatar(_friendAvatarInfo.AvatarId));
			}
		}

		private void RetrieveData(out string playerName, out string displayName, out bool invitable)
		{
			ClanMemberWithContextInfo clanMemberWithContextInfo = _dataSource.QueryData<ClanMemberWithContextInfo>(_itemIndex, 0);
			if (clanMemberWithContextInfo != null)
			{
				playerName = clanMemberWithContextInfo.Member.Name;
				displayName = clanMemberWithContextInfo.Member.DisplayName;
				invitable = clanMemberWithContextInfo.CanBeInvitedToParty;
			}
			else
			{
				InvitablePlayerData invitablePlayerData = _dataSource.QueryData<InvitablePlayerData>(_itemIndex, 0);
				playerName = invitablePlayerData.playerName;
				displayName = invitablePlayerData.displayName;
				invitable = invitablePlayerData.invitable;
				_friendAvatarInfo = invitablePlayerData.avatarInfo;
			}
		}

		public void SetSiblingIndex(int index)
		{
			_view.get_gameObject().get_transform().SetSiblingIndex(index);
		}
	}
}
