namespace LobbyServiceLayer
{
	internal class EnterMatchmakingQueueDependency
	{
		public readonly Platoon Platoon;

		public readonly int GarageSlot;

		public readonly LobbyType LobbyGameMode;

		public readonly int PlatoonSize;

		public readonly bool IsPlatoonLeader;

		public readonly int BrawlVersionNumber;

		public readonly GameModePreferences GameModePreferences;

		public EnterMatchmakingQueueDependency(Platoon platoon, int garageSlot, LobbyType gameMode, int platoonSize, bool isPlatoonLeader, int brawlVersionNumber_, GameModePreferences gameModePrefs)
		{
			Platoon = platoon;
			GarageSlot = garageSlot;
			LobbyGameMode = gameMode;
			PlatoonSize = platoonSize;
			IsPlatoonLeader = isPlatoonLeader;
			BrawlVersionNumber = brawlVersionNumber_;
			GameModePreferences = gameModePrefs;
		}
	}
}
