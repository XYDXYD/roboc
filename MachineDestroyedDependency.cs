using RCNetwork.Events;
using System.IO;

internal sealed class MachineDestroyedDependency : NetworkDependency
{
	public int owner;

	public MachineDestroyedDependency(byte[] data)
		: base(data)
	{
	}

	public MachineDestroyedDependency(int _owner)
	{
		owner = _owner;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(owner);
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
				owner = binaryReader.ReadInt32();
			}
		}
	}
}
