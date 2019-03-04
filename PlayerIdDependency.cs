using RCNetwork.Events;
using System.IO;

internal sealed class PlayerIdDependency : NetworkDependency
{
	public int owner;

	public PlayerIdDependency(byte[] data)
		: base(data)
	{
	}

	public PlayerIdDependency(int _owner)
	{
		owner = _owner;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)owner);
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
			}
		}
	}
}
