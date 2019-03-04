using RCNetwork.Events;
using System.IO;

internal sealed class SurrenderVoteDependency : NetworkDependency
{
	public bool surrender;

	public SurrenderVoteDependency(bool _surrender)
	{
		surrender = _surrender;
	}

	public SurrenderVoteDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(surrender);
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
				surrender = binaryReader.ReadBoolean();
			}
		}
	}
}
