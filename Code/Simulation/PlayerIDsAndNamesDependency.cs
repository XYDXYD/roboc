using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

namespace Simulation
{
	internal class PlayerIDsAndNamesDependency : NetworkDependency
	{
		public Dictionary<int, PlayerNamesContainer.Player> idToPlayer;

		public PlayerIDsAndNamesDependency()
		{
		}

		public PlayerIDsAndNamesDependency(byte[] data)
			: base(data)
		{
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((byte)idToPlayer.Count);
					foreach (KeyValuePair<int, PlayerNamesContainer.Player> item in idToPlayer)
					{
						binaryWriter.Write(item.Key);
						binaryWriter.Write(item.Value.name);
						binaryWriter.Write(item.Value.displayName);
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
					int num = binaryReader.ReadByte();
					idToPlayer = new Dictionary<int, PlayerNamesContainer.Player>(num);
					for (int i = 0; i < num; i++)
					{
						int key = binaryReader.ReadInt32();
						string name = binaryReader.ReadString();
						string displayName = binaryReader.ReadString();
						idToPlayer.Add(key, new PlayerNamesContainer.Player(name, displayName));
					}
				}
			}
		}
	}
}
