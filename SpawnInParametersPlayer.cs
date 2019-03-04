internal sealed class SpawnInParametersPlayer
{
	public int machineId;

	public int playerId;

	public string playerName;

	public int teamId;

	public bool isOnMyTeam;

	public PreloadedMachine preloadedMachine;

	public bool isAIMachine;

	public bool isMe;

	public bool isLocal;

	public SpawnInParametersPlayer(int _playerId, int _machineId, string _playerName, int _teamId, bool _isMe, bool _isOnMyTeam, PreloadedMachine _preloadedMachine, bool _isAImachine, bool _isLocal)
	{
		machineId = _machineId;
		playerId = _playerId;
		playerName = _playerName;
		teamId = _teamId;
		preloadedMachine = _preloadedMachine;
		isAIMachine = _isAImachine;
		isMe = _isMe;
		isOnMyTeam = _isOnMyTeam;
		isLocal = _isLocal;
	}
}
