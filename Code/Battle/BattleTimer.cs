using System;

namespace Battle
{
	internal class BattleTimer
	{
		private static DateTime _gameInitialiseTime = DateTime.MinValue;

		public float SecondsSinceGameInitialised => (float)(DateTime.UtcNow - _gameInitialiseTime).TotalSeconds;

		public void GameInitialised()
		{
			_gameInitialiseTime = DateTime.UtcNow;
		}
	}
}
