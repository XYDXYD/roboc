using RCNetwork.Events;
using System;
using System.IO;

internal sealed class CubesDestroyedDependency : NetworkDependency
{
	public uint cpuDestroyed;

	public int owner;

	public CubesDestroyedDependency(uint cpuDestroyed, int owner)
	{
		this.cpuDestroyed = cpuDestroyed;
		this.owner = owner;
	}

	public CubesDestroyedDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(BitConverter.GetBytes(cpuDestroyed));
				binaryWriter.Write(BitConverter.GetBytes(owner));
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
				cpuDestroyed = BitConverter.ToUInt32(binaryReader.ReadBytes(4), 0);
				owner = BitConverter.ToInt32(binaryReader.ReadBytes(4), 0);
			}
		}
	}
}
