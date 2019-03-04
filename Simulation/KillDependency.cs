using RCNetwork.Events;
using System;
using System.IO;

namespace Simulation
{
	internal class KillDependency : NetworkDependency
	{
		public int playerId
		{
			get;
			private set;
		}

		public int killerPlayerId
		{
			get;
			private set;
		}

		public KillDependency(int _playerId, int _killerPlayerId)
		{
			killerPlayerId = _killerPlayerId;
			playerId = _playerId;
		}

		public KillDependency(byte[] data)
			: base(data)
		{
		}

		public override byte[] Serialise()
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(Convert.ToByte(playerId));
					binaryWriter.Write(Convert.ToByte(killerPlayerId));
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
					playerId = Convert.ToInt32(binaryReader.ReadByte());
					killerPlayerId = Convert.ToInt32(binaryReader.ReadByte());
				}
			}
		}
	}
}
