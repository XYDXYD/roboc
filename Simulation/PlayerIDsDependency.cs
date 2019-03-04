using RCNetwork.Events;
using System.IO;

namespace Simulation
{
	internal class PlayerIDsDependency : NetworkDependency
	{
		public int[] playerIds;

		public PlayerIDsDependency(byte[] data)
			: base(data)
		{
		}

		public PlayerIDsDependency()
		{
		}

		public PlayerIDsDependency(int[] playerIds)
		{
			this.playerIds = playerIds;
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(playerIds.Length);
					for (int i = 0; i < playerIds.Length; i++)
					{
						binaryWriter.Write(playerIds[i]);
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
					int num = binaryReader.ReadInt32();
					playerIds = new int[num];
					for (int i = 0; i < playerIds.Length; i++)
					{
						playerIds[i] = binaryReader.ReadInt32();
					}
				}
			}
		}
	}
}
