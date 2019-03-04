using RCNetwork.Events;
using System;
using System.IO;

internal sealed class UpdateGameStatsDependency : NetworkDependency
{
	public int playerId
	{
		get;
		private set;
	}

	public InGameStatId gameStatsId
	{
		get;
		private set;
	}

	public uint amount
	{
		get;
		private set;
	}

	public uint score
	{
		get;
		private set;
	}

	public uint deltaScore
	{
		get;
		private set;
	}

	public UpdateGameStatsDependency(int _playerId, InGameStatId _gameStatsId, uint _amount, uint _score, uint _deltaScore)
	{
		SetFields(_playerId, _gameStatsId, _amount, _score, _deltaScore);
	}

	public UpdateGameStatsDependency(byte[] data)
		: base(data)
	{
	}

	public void SetFields(int _playerId, InGameStatId _gameStatsId, uint _amount, uint _score, uint _deltaScore)
	{
		playerId = _playerId;
		gameStatsId = _gameStatsId;
		amount = _amount;
		score = _score;
		deltaScore = _deltaScore;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(playerId));
				binaryWriter.Write(Convert.ToByte((uint)gameStatsId));
				binaryWriter.Write(amount);
				binaryWriter.Write(score);
				binaryWriter.Write(deltaScore);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				playerId = Convert.ToInt32(binaryReader.ReadByte());
				gameStatsId = (InGameStatId)Convert.ToUInt32(binaryReader.ReadByte());
				amount = binaryReader.ReadUInt32();
				score = binaryReader.ReadUInt32();
				deltaScore = binaryReader.ReadUInt32();
			}
		}
	}
}
