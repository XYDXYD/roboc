using System;

internal sealed class DesiredGameMode
{
	private LobbyType _gameMode;

	public LobbyType DesiredMode
	{
		get
		{
			return _gameMode;
		}
		set
		{
			if (_gameMode != value)
			{
				_gameMode = value;
				this.onChanged(_gameMode);
			}
		}
	}

	public event Action<LobbyType> onChanged = delegate
	{
	};
}
