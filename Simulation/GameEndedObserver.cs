using System;

namespace Simulation
{
	internal sealed class GameEndedObserver
	{
		private bool _gameEnded;

		public event Action<bool> OnGameEnded = delegate
		{
		};

		public void GameEnded(bool won)
		{
			if (!_gameEnded)
			{
				this.OnGameEnded(won);
				_gameEnded = true;
			}
		}
	}
}
