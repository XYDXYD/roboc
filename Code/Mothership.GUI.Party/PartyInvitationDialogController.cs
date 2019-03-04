using Avatars;
using Fabric;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;

namespace Mothership.GUI.Party
{
	internal class PartyInvitationDialogController : IInitialize, IPartyInvitationDialogController, IWaitForFrameworkDestruction
	{
		private string _inviterName;

		private PartyInvitationDialogView _guiView;

		private PresetAvatarMap _avatarMap;

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController guiInputController
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

		unsafe void IInitialize.OnDependenciesInjected()
		{
			_avatarMap = PresetAvatarMapProvider.GetAvatarMap();
			AvatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
		{
			if (avatarData.avatarType == AvatarType.PlayerAvatar && _inviterName == avatarData.avatarName)
			{
				_guiView.SetInviterAvatar(avatarData.texture);
			}
		}

		public void ReceiveMessage(object message)
		{
			if (message.GetType() == typeof(ButtonType))
			{
				ButtonType buttonType = (ButtonType)message;
				if (buttonType == ButtonType.AcceptPlatoonInvite)
				{
					AcceptInvite();
				}
				if (buttonType == ButtonType.DeclinePlatoonInvite)
				{
					DeclineInvite();
				}
			}
			else if (message.GetType() == typeof(PartyInvitationReceivedMessage))
			{
				PartyInvitationReceivedMessage partyInvitationReceivedMessage = (PartyInvitationReceivedMessage)message;
				Configure(partyInvitationReceivedMessage.originatorName, partyInvitationReceivedMessage.displayName, partyInvitationReceivedMessage.avatarInfo);
			}
			else if (message.GetType() == typeof(HidePartyInvitationDialogMessage))
			{
				_guiView.Hide();
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Pending), 1);
			}
			else if (message.GetType() == typeof(ShowPartyInvitationDialogMessage))
			{
				_guiView.Show();
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_Party_Request_Pending));
			}
		}

		public void RegisterView(PartyInvitationDialogView guiView)
		{
			_guiView = guiView;
		}

		public void CheckForHotkeyInput()
		{
			if (guiInputController.GetShortCutMode() == ShortCutMode.AllShortCuts || guiInputController.GetShortCutMode() == ShortCutMode.OnlyGUINoSwitching || guiInputController.GetShortCutMode() == ShortCutMode.BuildShortCuts)
			{
				if (InputRemapper.Instance.GetButtonDown("AcceptSurrender"))
				{
					AcceptInvite();
				}
				else if (InputRemapper.Instance.GetButtonDown("DeclineSurrender"))
				{
					DeclineInvite();
				}
			}
		}

		internal void AcceptInvite()
		{
			_guiView.DispatchMessage(new PartyInvitationResponseMessage(accept: true));
		}

		internal void DeclineInvite()
		{
			_guiView.DispatchMessage(new PartyInvitationResponseMessage(accept: false));
		}

		private void Configure(string originatorName, string displayName, AvatarInfo avatarInfo)
		{
			if (avatarInfo.UseCustomAvatar)
			{
				_inviterName = originatorName;
				AvatarLoader.RequestAvatar(AvatarType.PlayerAvatar, originatorName);
			}
			else
			{
				_guiView.SetInviterAvatar(_avatarMap.GetPresetAvatar(avatarInfo.AvatarId));
			}
			_guiView.SetInviterName(displayName);
		}

		public unsafe void OnFrameworkDestroyed()
		{
			AvatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
	}
}
