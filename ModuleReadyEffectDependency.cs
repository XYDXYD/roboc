using RCNetwork.Events;
using System.IO;

internal sealed class ModuleReadyEffectDependency : NetworkDependency
{
	public bool activate;

	public int playerId;

	public int moduleIndex;

	public ItemCategory moduleType;

	public ModuleReadyEffectDependency()
	{
	}

	public ModuleReadyEffectDependency(byte[] data)
		: base(data)
	{
	}

	public ModuleReadyEffectDependency Inject(bool activate, int playerId, int moduleIndex, ItemCategory moduleType)
	{
		this.activate = activate;
		this.playerId = playerId;
		this.moduleIndex = moduleIndex;
		this.moduleType = moduleType;
		return this;
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
				binaryWriter.Write((int)moduleType);
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
				moduleType = (ItemCategory)binaryReader.ReadInt32();
			}
		}
	}
}
