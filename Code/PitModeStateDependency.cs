using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

internal class PitModeStateDependency : NetworkDependency
{
	public Dictionary<int, uint> playerScores
	{
		get;
		private set;
	}

	public Dictionary<int, uint> currentStreaks
	{
		get;
		private set;
	}

	public int leaderId
	{
		get;
		private set;
	}

	public PitModeStateDependency(Dictionary<int, uint> streaks, Dictionary<int, uint> scores, int leader)
	{
		currentStreaks = streaks;
		playerScores = scores;
		leaderId = leader;
	}

	public PitModeStateDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)playerScores.Count);
				foreach (KeyValuePair<int, uint> playerScore in playerScores)
				{
					binaryWriter.Write((byte)playerScore.Key);
					binaryWriter.Write(playerScore.Value);
					binaryWriter.Write(currentStreaks[playerScore.Key]);
				}
				binaryWriter.Write(leaderId);
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		playerScores = new Dictionary<int, uint>();
		currentStreaks = new Dictionary<int, uint>();
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = binaryReader.ReadByte();
				for (int i = 0; i < num; i++)
				{
					int key = binaryReader.ReadByte();
					uint value = binaryReader.ReadUInt32();
					uint value2 = binaryReader.ReadUInt32();
					playerScores.Add(key, value);
					currentStreaks.Add(key, value2);
				}
				leaderId = binaryReader.ReadInt32();
			}
		}
	}
}
