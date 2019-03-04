using RCNetwork.Events;
using System.IO;

internal sealed class FusionShieldStateDependency : NetworkDependency
{
	public int teamId;

	public bool fullPower;

	public FusionShieldStateDependency(byte[] data)
		: base(data)
	{
	}

	public FusionShieldStateDependency(int teamId, bool fullPower)
	{
		this.teamId = teamId;
		this.fullPower = fullPower;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((sbyte)teamId);
				binaryWriter.Write(fullPower);
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
				teamId = binaryReader.ReadSByte();
				fullPower = binaryReader.ReadBoolean();
			}
		}
	}
}
