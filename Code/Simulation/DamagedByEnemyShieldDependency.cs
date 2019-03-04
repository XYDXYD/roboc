using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

namespace Simulation
{
	internal sealed class DamagedByEnemyShieldDependency : NetworkDependency
	{
		public int machineId;

		public float timestamp;

		public List<HitCubeInfo> damagedCubes;

		public DamagedByEnemyShieldDependency()
		{
		}

		public DamagedByEnemyShieldDependency(byte[] data)
			: base(data)
		{
		}

		public void SetValues(int machineId_, float timestamp_, List<HitCubeInfo> damagedCubes_)
		{
			machineId = machineId_;
			timestamp = timestamp_;
			damagedCubes = damagedCubes_;
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((short)machineId);
					binaryWriter.Write(timestamp);
					ushort num = (ushort)damagedCubes.Count;
					binaryWriter.Write(num);
					for (int i = 0; i < num; i++)
					{
						HitCubeInfo hitCubeInfo = damagedCubes[i];
						binaryWriter.Write(hitCubeInfo.gridLoc.x);
						binaryWriter.Write(hitCubeInfo.gridLoc.y);
						binaryWriter.Write(hitCubeInfo.gridLoc.z);
						binaryWriter.Write(hitCubeInfo.destroyed);
						if (!hitCubeInfo.destroyed)
						{
							binaryWriter.Write(hitCubeInfo.damage);
						}
					}
					return memoryStream.ToArray();
				}
			}
		}

		public override void Deserialise(byte[] data)
		{
			damagedCubes = new List<HitCubeInfo>();
			using (MemoryStream input = new MemoryStream(data))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					machineId = binaryReader.ReadInt16();
					timestamp = binaryReader.ReadSingle();
					int num = binaryReader.ReadUInt16();
					for (int i = 0; i < num; i++)
					{
						HitCubeInfo item = default(HitCubeInfo);
						item.gridLoc.x = binaryReader.ReadByte();
						item.gridLoc.y = binaryReader.ReadByte();
						item.gridLoc.z = binaryReader.ReadByte();
						item.destroyed = binaryReader.ReadBoolean();
						if (!item.destroyed)
						{
							item.damage = binaryReader.ReadInt32();
						}
						damagedCubes.Add(item);
					}
				}
			}
		}
	}
}
