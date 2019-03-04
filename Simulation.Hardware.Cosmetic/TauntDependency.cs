using RCNetwork.Events;
using System.IO;
using UnityEngine;

namespace Simulation.Hardware.Cosmetic
{
	internal class TauntDependency : NetworkDependency
	{
		public int machineId;

		public string tauntId;

		public Vector3 relativePosition;

		public Quaternion relativeOrientation;

		public TauntDependency()
		{
		}

		public TauntDependency(byte[] data)
			: base(data)
		{
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(machineId);
					binaryWriter.Write(tauntId);
					binaryWriter.Write(relativePosition.x);
					binaryWriter.Write(relativePosition.y);
					binaryWriter.Write(relativePosition.z);
					binaryWriter.Write(relativeOrientation.x);
					binaryWriter.Write(relativeOrientation.y);
					binaryWriter.Write(relativeOrientation.z);
					binaryWriter.Write(relativeOrientation.w);
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
					tauntId = binaryReader.ReadString();
					relativePosition.x = binaryReader.ReadSingle();
					relativePosition.y = binaryReader.ReadSingle();
					relativePosition.z = binaryReader.ReadSingle();
					relativeOrientation.x = binaryReader.ReadSingle();
					relativeOrientation.y = binaryReader.ReadSingle();
					relativeOrientation.z = binaryReader.ReadSingle();
					relativeOrientation.w = binaryReader.ReadSingle();
				}
			}
		}
	}
}
