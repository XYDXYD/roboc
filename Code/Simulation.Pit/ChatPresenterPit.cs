namespace Simulation.Pit
{
	internal class ChatPresenterPit : ChatPresenterMultiplayer
	{
		protected override void Initialize()
		{
			_battleId = base.BattleParameters.BattleId;
			JoinBattleChannel();
		}

		protected override void TearDown()
		{
			LeaveBattleChannel();
		}

		private void JoinBattleChannel()
		{
			JoinChannel(GetSharedChannelName(), ChatChannelType.Battle);
		}

		private void LeaveBattleChannel()
		{
			if (base.chatChannelContainer.TryGetChannelByType(ChatChannelType.Battle) != null)
			{
				LeaveChannel(GetSharedChannelName(), ChatChannelType.Battle);
			}
		}
	}
}
