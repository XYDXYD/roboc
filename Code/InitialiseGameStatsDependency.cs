using RCNetwork.Events;
using System;
using System.Collections.Generic;
using System.IO;

internal sealed class InitialiseGameStatsDependency : NetworkDependency
{
	public List<InGamePlayerStats> inGamePlayerStatsList
	{
		get;
		private set;
	}

	public InitialiseGameStatsDependency(List<InGamePlayerStats> data)
	{
		inGamePlayerStatsList = data;
	}

	public InitialiseGameStatsDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(inGamePlayerStatsList.Count));
				for (int i = 0; i < inGamePlayerStatsList.Count; i++)
				{
					InGamePlayerStats inGamePlayerStats = inGamePlayerStatsList[i];
					binaryWriter.Write(Convert.ToByte(inGamePlayerStats.playerId));
					binaryWriter.Write(Convert.ToByte(inGamePlayerStats.inGameStats.Count));
					for (int j = 0; j < inGamePlayerStats.inGameStats.Count; j++)
					{
						InGameStat inGameStat = inGamePlayerStats.inGameStats[j];
						binaryWriter.Write(Convert.ToByte((uint)inGameStat.ID));
						binaryWriter.Write(inGameStat.Amount);
						binaryWriter.Write(inGameStat.Score);
					}
				}
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
				inGamePlayerStatsList = new List<InGamePlayerStats>();
				int num = Convert.ToInt32(binaryReader.ReadByte());
				for (int i = 0; i < num; i++)
				{
					int playerName = Convert.ToInt32(binaryReader.ReadByte());
					int num2 = Convert.ToInt32(binaryReader.ReadByte());
					List<InGameStat> list = new List<InGameStat>();
					for (int j = 0; j < num2; j++)
					{
						InGameStatId id = (InGameStatId)Convert.ToUInt32(binaryReader.ReadByte());
						uint amount = binaryReader.ReadUInt32();
						uint score = binaryReader.ReadUInt32();
						list.Add(new InGameStat(id, amount, score));
					}
					inGamePlayerStatsList.Add(new InGamePlayerStats(playerName, list));
				}
			}
		}
	}
}
