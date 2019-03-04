using RCNetwork.Events;
using System;
using System.Collections.Generic;
using System.IO;

internal class ProtectTeamMateBonusRequestDependency : NetworkDependency
{
	public Dictionary<int, Dictionary<uint, uint>> dependencyData
	{
		get;
		private set;
	}

	public ProtectTeamMateBonusRequestDependency(Dictionary<int, Dictionary<uint, uint>> playerBonusesData)
	{
		dependencyData = playerBonusesData;
	}

	public ProtectTeamMateBonusRequestDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(dependencyData.Count));
				foreach (KeyValuePair<int, Dictionary<uint, uint>> dependencyDatum in dependencyData)
				{
					binaryWriter.Write(Convert.ToByte(dependencyDatum.Key));
					binaryWriter.Write(dependencyDatum.Value.Keys.Count);
					foreach (KeyValuePair<uint, uint> item in dependencyDatum.Value)
					{
						binaryWriter.Write(item.Key);
						binaryWriter.Write(item.Value);
					}
				}
				return memoryStream.ToArray();
			}
		}
	}

	public override void Deserialise(byte[] data)
	{
		dependencyData = new Dictionary<int, Dictionary<uint, uint>>();
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = Convert.ToInt32(binaryReader.ReadByte());
				for (int i = 0; i < num; i++)
				{
					int key = Convert.ToInt32(binaryReader.ReadByte());
					Dictionary<uint, uint> dictionary = new Dictionary<uint, uint>();
					dependencyData.Add(key, dictionary);
					int num2 = binaryReader.ReadInt32();
					for (int j = 0; j < num2; j++)
					{
						uint key2 = binaryReader.ReadUInt32();
						uint value = binaryReader.ReadUInt32();
						dictionary.Add(key2, value);
					}
				}
			}
		}
	}
}
