using RCNetwork.Events;
using System.Collections.Generic;
using System.IO;

namespace Simulation
{
	internal sealed class SyncMachineCubesDependency : NetworkDependency
	{
		public int machineId;

		public List<CubeHistoryEvent> history;

		public SyncMachineCubesDependency()
		{
		}

		public SyncMachineCubesDependency(byte[] data)
			: base(data)
		{
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write((short)machineId);
					binaryWriter.Write(history.Count);
					for (int i = 0; i < history.Count; i++)
					{
						CubeHistoryEvent cubeHistoryEvent = history[i];
						binaryWriter.Write(cubeHistoryEvent.gridLoc.x);
						binaryWriter.Write(cubeHistoryEvent.gridLoc.y);
						binaryWriter.Write(cubeHistoryEvent.gridLoc.z);
						binaryWriter.Write((byte)cubeHistoryEvent.type);
						if (cubeHistoryEvent.type != CubeHistoryEvent.Type.Destroy)
						{
							binaryWriter.Write(cubeHistoryEvent.damage);
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
					machineId = binaryReader.ReadInt16();
					int num = binaryReader.ReadInt32();
					history = new List<CubeHistoryEvent>(num);
					for (int i = 0; i < num; i++)
					{
						CubeHistoryEvent item = default(CubeHistoryEvent);
						item.gridLoc.x = binaryReader.ReadByte();
						item.gridLoc.y = binaryReader.ReadByte();
						item.gridLoc.z = binaryReader.ReadByte();
						item.type = (CubeHistoryEvent.Type)binaryReader.ReadByte();
						if (item.type != CubeHistoryEvent.Type.Destroy)
						{
							item.damage = binaryReader.ReadInt32();
						}
						history.Add(item);
					}
				}
			}
		}
	}
}
