namespace LobbyServiceLayer
{
	public struct QuitterInfo
	{
		public readonly bool QuitLastGame;

		public readonly int QuitterBlockTime;

		public QuitterInfo(bool quitLastGame, int quitterBlockTime)
		{
			this = default(QuitterInfo);
			QuitLastGame = quitLastGame;
			QuitterBlockTime = quitterBlockTime;
		}
	}
}
