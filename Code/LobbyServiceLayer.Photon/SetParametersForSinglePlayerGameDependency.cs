namespace LobbyServiceLayer.Photon
{
	internal struct SetParametersForSinglePlayerGameDependency
	{
		public readonly string MapName;

		public SetParametersForSinglePlayerGameDependency(string mapName)
		{
			MapName = mapName;
		}
	}
}
