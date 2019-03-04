using RCNetwork.Events;
using System;
using System.IO;

[Obsolete("Use KillDependency instead")]
internal sealed class KillBonusRequestDependency : NetworkDependency
{
	internal int shooterId
	{
		get;
		private set;
	}

	public KillBonusRequestDependency(int _shooterId)
	{
		shooterId = _shooterId;
	}

	public KillBonusRequestDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(shooterId));
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
				shooterId = Convert.ToInt32(binaryReader.ReadByte());
			}
		}
	}
}
