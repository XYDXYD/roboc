namespace Simulation
{
	internal class RegisterPlayerData
	{
		public int playerId = -1;

		public string playerName;

		public string displayName;

		public bool isMe;

		public bool isMyTeam;

		public RegisterPlayerData(int playerId, string playerName, string displayName, bool isMe, bool isMyTeam)
		{
			this.playerId = playerId;
			this.playerName = playerName;
			this.displayName = displayName;
			this.isMe = isMe;
			this.isMyTeam = isMyTeam;
		}
	}
}
