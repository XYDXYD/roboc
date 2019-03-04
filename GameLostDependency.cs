using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

internal class GameLostDependency : NetworkDependency
{
	public List<int> loserIds
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

	public GameLostDependency(byte[] data)
		: base(data)
	{
	}

	public GameLostDependency(ICollection<int> _loserIds, int _winningTeam, GameEndReason reason)
	{
		loserIds = new List<int>(_loserIds);
		winningTeam = _winningTeam;
		gameEndReason = reason;
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
