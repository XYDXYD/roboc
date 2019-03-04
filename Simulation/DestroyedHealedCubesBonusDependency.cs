using RCNetwork.Events;
using Simulation.NamedCollections;
using System.Collections.Generic;
using System.IO;

namespace Simulation
{
	internal class DestroyedHealedCubesBonusDependency : NetworkDependency
	{
		public Dictionary<int, PlayersCubes> shooterTargetPlayers
		{
			get;
			protected set;
		}

		public DestroyedHealedCubesBonusDependency(Dictionary<int, PlayersCubes> pShooterTargetPlayers)
		{
			shooterTargetPlayers = pShooterTargetPlayers;
		}

		public DestroyedHealedCubesBonusDependency(byte[] data)
			: base(data)
		{
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((byte)shooterTargetPlayers.Count);
					foreach (KeyValuePair<int, PlayersCubes> shooterTargetPlayer in shooterTargetPlayers)
					{
						binaryWriter.Write((byte)shooterTargetPlayer.Key);
						binaryWriter.Write((byte)shooterTargetPlayer.Value.Count);
						foreach (KeyValuePair<int, CubeAmounts> item in shooterTargetPlayer.Value)
						{
							binaryWriter.Write((byte)item.Key);
							binaryWriter.Write((short)item.Value.Count);
							foreach (KeyValuePair<uint, uint> item2 in item.Value)
							{
								binaryWriter.Write(item2.Key);
								binaryWriter.Write(item2.Value);
							}
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
					shooterTargetPlayers = new Dictionary<int, PlayersCubes>();
					int num = binaryReader.ReadByte();
					for (int i = 0; i < num; i++)
					{
						int key = binaryReader.ReadByte();
						PlayersCubes playersCubes = new PlayersCubes();
						shooterTargetPlayers.Add(key, playersCubes);
						int num2 = binaryReader.ReadByte();
						for (int j = 0; j < num2; j++)
						{
							int key2 = binaryReader.ReadByte();
							CubeAmounts cubeAmounts = new CubeAmounts();
							playersCubes.Add(key2, cubeAmounts);
							int num3 = binaryReader.ReadInt16();
							for (int k = 0; k < num3; k++)
							{
								cubeAmounts.Add(binaryReader.ReadUInt32(), binaryReader.ReadUInt32());
							}
						}
					}
				}
			}
		}
	}
}
