using Battle;
using Svelto.IoC;
using System;

namespace Simulation
{
	internal class ChatPresenterMultiplayer : ChatPresenter
	{
		private int _myTeam;

		protected string _battleId;

		[Inject]
		internal PlayerTeamsContainer PlayerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		internal BattleParameters BattleParameters
		{
			get;
			set;
		}

		public ChatPresenterMultiplayer()
			: base(shouldDisplayMemberEvents: false)
		{
			_chatStyle = ChatStyle.Battle;
		}

		protected override void Initialize()
		{
			_battleId = BattleParameters.BattleId;
			PlayerTeamsContainer.IfOrWhenOwnIdRegistered(JoinBattleChannels);
			foreach (IChatChannel item in base.chatChannelContainer)
			{
				SuspendIfNecessary(item);
			}
		}

		protected override void TearDown()
		{
			LeaveBattleChannels();
			foreach (IChatChannel item in base.chatChannelContainer)
			{
				if (item.ChatChannelType == ChatChannelType.Custom || item.ChatChannelType == ChatChannelType.Public)
				{
					item.Suspended = false;
				}
			}
		}

		protected override bool GetInitialExpanding()
		{
			return false;
		}

		protected override string GetChatLocation()
		{
			return "Battle";
		}

		protected string GetSharedChannelName()
		{
			return "Battle" + _battleId;
		}

		protected string GetTeamChannelName()
		{
			return GetSharedChannelName() + "Team" + _myTeam;
		}

		protected IChatChannel GetBattleChannel()
		{
			foreach (IChatChannel item in base.chatChannelContainer)
			{
				if (item.ChatChannelType == ChatChannelType.Battle)
				{
					return item;
				}
			}
			throw new Exception("Battle channel not found");
		}

		protected IChatChannel GetTeamChannel()
		{
			foreach (IChatChannel item in base.chatChannelContainer)
			{
				if (item.ChatChannelType == ChatChannelType.BattleTeam)
				{
					return item;
				}
			}
			throw new Exception("Team channel not found");
		}

		internal override void ChannelAddedToCollection(IChatChannel chatChannel)
		{
			SuspendIfNecessary(chatChannel);
			base.ChannelAddedToCollection(chatChannel);
		}

		private static void SuspendIfNecessary(IChatChannel channel)
		{
			if (channel.ChatChannelType == ChatChannelType.Custom || channel.ChatChannelType == ChatChannelType.Public || channel.ChatChannelType == ChatChannelType.Clan)
			{
				channel.Suspended = true;
			}
		}

		private void JoinBattleChannels()
		{
			_myTeam = PlayerTeamsContainer.GetMyTeam();
			JoinChannel(GetSharedChannelName(), ChatChannelType.Battle);
			JoinChannel(GetTeamChannelName(), ChatChannelType.BattleTeam);
		}

		private void LeaveBattleChannels()
		{
			if (base.chatChannelContainer.TryGetChannelByType(ChatChannelType.Battle) != null)
			{
				LeaveChannel(GetSharedChannelName(), ChatChannelType.Battle);
			}
			if (base.chatChannelContainer.TryGetChannelByType(ChatChannelType.BattleTeam) != null)
			{
				LeaveChannel(GetTeamChannelName(), ChatChannelType.BattleTeam);
			}
		}
	}
}
