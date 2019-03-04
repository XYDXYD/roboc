using System;

namespace Simulation
{
	internal sealed class GameStartDispatcher
	{
		private Action _runWhenGameStarted = delegate
		{
		};

		private bool _gameStarted;

		private bool _isReconnecting;

		public bool isReconnecting => _isReconnecting;

		public void Register(Action callback)
		{
			if (_gameStarted)
			{
				callback();
			}
			else
			{
				_runWhenGameStarted = (Action)Delegate.Combine(_runWhenGameStarted, callback);
			}
		}

		public void Unregister(Action callback)
		{
			_runWhenGameStarted = (Action)Delegate.Remove(_runWhenGameStarted, callback);
		}

		public void OnGameStart(bool isReconnecting = false)
		{
			_isReconnecting = isReconnecting;
			_gameStarted = true;
			_runWhenGameStarted();
			_runWhenGameStarted = delegate
			{
			};
		}
	}
}
