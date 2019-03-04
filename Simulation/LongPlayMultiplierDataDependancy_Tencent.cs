using RCNetwork.Events;
using System.IO;

namespace Simulation
{
	internal class LongPlayMultiplierDataDependancy_Tencent : NetworkDependency
	{
		public float LongPlayClientMultiplier;

		public LongPlayMultiplierDataDependancy_Tencent()
		{
		}

		public LongPlayMultiplierDataDependancy_Tencent(byte[] data)
			: base(data)
		{
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(LongPlayClientMultiplier);
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
					LongPlayClientMultiplier = binaryReader.ReadSingle();
				}
			}
		}
	}
}
