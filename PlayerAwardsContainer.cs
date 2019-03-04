using GameServer;
using System.Collections.Generic;

internal class PlayerAwardsContainer
{
	public string playerName
	{
		get;
		private set;
	}

	public bool didTeamWin
	{
		get;
		private set;
	}

	public uint score
	{
		get;
		private set;
	}

	public int scorePosition
	{
		get;
		private set;
	}

	public int scorePositionWithinTeam
	{
		get;
		private set;
	}

	public Dictionary<PlayerAwardId, int> awards
	{
		get;
		private set;
	}

	public bool isPlayerAlive
	{
		get;
		private set;
	}

	public int numPartyMembers
	{
		get;
		private set;
	}

	public int masteryLevel
	{
		get;
		private set;
	}

	public int teamID
	{
		get;
		private set;
	}

	public PlayerAwardsContainer(string _playerName, bool _isWinner, uint _score, Dictionary<PlayerAwardId, int> _awards, int _scorePosition, int _scorePositionWithinTeam, int numPartyMembers_, bool _isPlayerAlive, int masteryLevel_, int teamID_)
	{
		playerName = _playerName;
		didTeamWin = _isWinner;
		score = _score;
		awards = _awards;
		scorePosition = _scorePosition;
		scorePositionWithinTeam = _scorePositionWithinTeam;
		isPlayerAlive = _isPlayerAlive;
		numPartyMembers = numPartyMembers_;
		masteryLevel = masteryLevel_;
		teamID = teamID_;
	}
}
