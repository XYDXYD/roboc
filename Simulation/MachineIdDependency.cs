using RCNetwork.Events;
using System.IO;

namespace Simulation
{
	internal sealed class MachineIdDependency : NetworkDependency
	{
		public int machineId;

		public MachineIdDependency(byte[] data)
			: base(data)
		{
		}

		public MachineIdDependency(int pMachineId)
		{
			machineId = pMachineId;
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(machineId);
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
				}
			}
		}
	}
}
