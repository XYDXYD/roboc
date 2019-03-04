using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

internal class GameWonDependency : NetworkDependency
{
	public List<int> winnerIds
	{
		get;
		private set;
	}

	public int winningTeam
	{
		get;
		private set;
	}

	public GameEndReason gameEndReason
	{
		get;
		private set;
	}

	public GameWonDependency(byte[] data)
		: base(data)
	{
	}

	public GameWonDependency(ICollection<int> _winnerIds, int _winningTeam, GameEndReason gameEndReason_)
	{
		winnerIds = new List<int>(_winnerIds);
		winningTeam = _winningTeam;
		gameEndReason = gameEndReason_;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)winningTeam);
				binaryWriter.Write((byte)gameEndReason);
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
				winningTeam = binaryReader.ReadByte();
				gameEndReason = (GameEndReason)binaryReader.ReadByte();
			}
		}
	}
}
