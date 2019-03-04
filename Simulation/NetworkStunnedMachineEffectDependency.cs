using RCNetwork.Events;
using System.IO;

namespace Simulation
{
	internal sealed class NetworkStunnedMachineEffectDependency : NetworkDependency
	{
		public int machineId;

		public bool isStunned;

		public int ownerId;

		public NetworkStunnedMachineEffectDependency()
		{
		}

		public NetworkStunnedMachineEffectDependency(byte[] data)
			: base(data)
		{
		}

		public void SetValues(int machineId_, bool isStunned_, int ownerId_)
		{
			machineId = machineId_;
			isStunned = isStunned_;
			ownerId = ownerId_;
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(machineId);
					binaryWriter.Write(isStunned);
					binaryWriter.Write(ownerId);
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
					machineId = binaryReader.ReadInt32();
					isStunned = binaryReader.ReadBoolean();
					ownerId = binaryReader.ReadInt32();
				}
			}
		}
	}
}
