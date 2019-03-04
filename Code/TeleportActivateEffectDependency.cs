using RCNetwork.Events;
using System.IO;

internal sealed class TeleportActivateEffectDependency : NetworkDependency
{
	public bool activate;

	public int playerId;

	public int moduleIndex;

	public TeleportActivateEffectDependency()
	{
	}

	public TeleportActivateEffectDependency(byte[] data)
		: base(data)
	{
	}

	public void Inject(bool activate_, int playerId_, int moduleIndex_)
	{
		activate = activate_;
		playerId = playerId_;
		moduleIndex = moduleIndex_;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(activate);
				binaryWriter.Write((byte)playerId);
				binaryWriter.Write((byte)moduleIndex);
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
				activate = binaryReader.ReadBoolean();
				playerId = binaryReader.ReadByte();
				moduleIndex = binaryReader.ReadByte();
			}
		}
	}
}
