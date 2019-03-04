using RCNetwork.Events;
using System.IO;

internal sealed class RespawnTimeDependency : NetworkDependency
{
	public int waitingTime;

	public int owner;

	public RespawnTimeDependency(byte[] data)
		: base(data)
	{
	}

	public RespawnTimeDependency(int _owner, int _waitingTime)
	{
		owner = _owner;
		waitingTime = _waitingTime;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)owner);
				binaryWriter.Write((short)waitingTime);
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
				owner = binaryReader.ReadByte();
				waitingTime = binaryReader.ReadInt16();
			}
		}
	}
}
