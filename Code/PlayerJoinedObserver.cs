using System;

internal class PlayerJoinedObserver
{
	internal struct PlayerJoinedDependency
	{
		public string playerName;

		public int teamId;

		public int playerId;
	}

	private event Action<PlayerJoinedDependency> _playerJoined = delegate
	{
	};

	public void OnPlayerJoined(PlayerJoinedDependency dependency)
	{
		this._playerJoined(dependency);
	}

	public void Register(Action<PlayerJoinedDependency> onJoined)
	{
		_playerJoined += onJoined;
	}

	public void Unregister(Action<PlayerJoinedDependency> onJoined)
	{
		_playerJoined -= onJoined;
	}
}
