namespace Simulation.SinglePlayer
{
	internal class AIBotIdData : IAIBotIdDataComponent
	{
		private int _playerId;

		private int _teamId;

		private int _machineId;

		int IAIBotIdDataComponent.playerId
		{
			get
			{
				return _playerId;
			}
		}

		int IAIBotIdDataComponent.teamId
		{
			get
			{
				return _teamId;
			}
		}

		public int machineId => _machineId;

		public AIBotIdData(int playerId, int teamId, int machineId)
		{
			_playerId = playerId;
			_teamId = teamId;
			_machineId = machineId;
		}
	}
}
