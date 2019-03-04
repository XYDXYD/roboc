using RCNetwork.Events;
using System.IO;

internal sealed class CloakModuleEventDependency : NetworkDependency
{
	public int playerId;

	public CloakModuleEventDependency()
	{
	}

	public CloakModuleEventDependency(byte[] data)
		: base(data)
	{
	}

	public void SetVariables(int playerId)
	{
		this.playerId = playerId;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)playerId);
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
				playerId = binaryReader.ReadByte();
			}
		}
	}
}
