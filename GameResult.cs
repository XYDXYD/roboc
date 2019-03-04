using GameServer;
using System;
using System.Collections.Generic;
using System.IO;

internal class GameResult
{
	public List<PlayerAwardsContainer> winnerTeamAwards
	{
		get;
		private set;
	}

	public List<PlayerAwardsContainer> losingTeamAwards
	{
		get;
		private set;
	}

	public GameModeKey gameMode
	{
		get;
		private set;
	}

	public GameResult(List<PlayerAwardsContainer> _winnerTeamAwards, List<PlayerAwardsContainer> _losingTeamAwards, GameModeKey _gameModeType)
	{
		winnerTeamAwards = _winnerTeamAwards;
		losingTeamAwards = _losingTeamAwards;
		gameMode = _gameModeType;
	}

	public virtual byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				BinaryWriter binaryWriter2 = binaryWriter;
				GameModeKey gameMode = this.gameMode;
				binaryWriter2.Write((byte)gameMode.type);
				BinaryWriter binaryWriter3 = binaryWriter;
				GameModeKey gameMode2 = this.gameMode;
				binaryWriter3.Write(gameMode2.IsRanked);
				BinaryWriter binaryWriter4 = binaryWriter;
				GameModeKey gameMode3 = this.gameMode;
				binaryWriter4.Write(gameMode3.IsBrawl);
				BinaryWriter binaryWriter5 = binaryWriter;
				GameModeKey gameMode4 = this.gameMode;
				binaryWriter5.Write(gameMode4.IsCustomGame);
				SerialiseWinners(binaryWriter);
				SerialiseLosers(binaryWriter);
				return memoryStream.ToArray();
			}
		}
	}

	protected void SerialiseWinners(BinaryWriter bw)
	{
		SerialiseAwards(bw, won: true, died: false, winnerTeamAwards);
	}

	protected void SerialiseLosers(BinaryWriter bw)
	{
		SerialiseAwards(bw, won: false, died: false, losingTeamAwards);
	}

	protected void SerialiseAwards(BinaryWriter bw, bool won, bool died, List<PlayerAwardsContainer> playerAwards)
	{
		bw.Write(playerAwards.Count);
		foreach (PlayerAwardsContainer playerAward in playerAwards)
		{
			bw.Write(playerAward.playerName);
			bw.Write((byte)playerAward.masteryLevel);
			bw.Write((byte)playerAward.teamID);
			bw.Write(playerAward.isPlayerAlive);
			bw.Write((byte)playerAward.numPartyMembers);
			bw.Write(Convert.ToInt32(playerAward.score));
			bw.Write((byte)playerAward.scorePosition);
			bw.Write((byte)playerAward.scorePositionWithinTeam);
			bw.Write((byte)playerAward.awards.Count);
			foreach (KeyValuePair<PlayerAwardId, int> award in playerAward.awards)
			{
				bw.Write((byte)award.Key);
				bw.Write(Convert.ToInt32(award.Value));
			}
		}
	}
}
