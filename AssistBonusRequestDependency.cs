using RCNetwork.Events;
using System;
using System.Collections.Generic;
using System.IO;

internal sealed class AssistBonusRequestDependency : NetworkDependency
{
	public int requesterPlayerId
	{
		get;
		private set;
	}

	public List<int> awardedPlayerIds
	{
		get;
		private set;
	}

	public AssistBonusRequestDependency(int requesterPlayerId, List<int> data)
	{
		this.requesterPlayerId = requesterPlayerId;
		awardedPlayerIds = data;
	}

	public AssistBonusRequestDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(requesterPlayerId);
				binaryWriter.Write(awardedPlayerIds.Count);
				for (int i = 0; i < awardedPlayerIds.Count; i++)
				{
					binaryWriter.Write(Convert.ToByte(awardedPlayerIds[i]));
				}
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		awardedPlayerIds = new List<int>();
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				requesterPlayerId = binaryReader.ReadInt32();
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					awardedPlayerIds.Add(Convert.ToInt32(binaryReader.ReadByte()));
				}
			}
		}
	}
}
