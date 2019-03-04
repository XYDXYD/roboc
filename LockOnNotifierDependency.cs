using RCNetwork.Events;
using System.IO;

internal sealed class LockOnNotifierDependency : NetworkDependency
{
	public int firingPlayerId;

	public int targetPlayerId;

	public int lockStage;

	public Byte3 lockedCubePosition;

	public ItemCategory itemCategory;

	public ItemSize itemSize;

	public LockOnNotifierDependency(byte[] data)
		: base(data)
	{
	}

	public LockOnNotifierDependency(int firingPlayerId, int targetPlayerId, int lockStage, Byte3 lockedCubePosition, ItemCategory itemCategory, ItemSize itemSize)
	{
		this.firingPlayerId = firingPlayerId;
		this.targetPlayerId = targetPlayerId;
		this.lockStage = lockStage;
		this.lockedCubePosition = lockedCubePosition;
		this.itemCategory = itemCategory;
		this.itemSize = itemSize;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)firingPlayerId);
				binaryWriter.Write((byte)targetPlayerId);
				binaryWriter.Write((byte)lockStage);
				binaryWriter.Write(lockedCubePosition.x);
				binaryWriter.Write(lockedCubePosition.y);
				binaryWriter.Write(lockedCubePosition.z);
				binaryWriter.Write((int)itemCategory);
				binaryWriter.Write((int)itemSize);
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
				firingPlayerId = binaryReader.ReadByte();
				targetPlayerId = binaryReader.ReadByte();
				lockStage = binaryReader.ReadByte();
				byte x = binaryReader.ReadByte();
				byte y = binaryReader.ReadByte();
				byte z = binaryReader.ReadByte();
				lockedCubePosition = new Byte3(x, y, z);
				itemCategory = (ItemCategory)binaryReader.ReadInt32();
				itemSize = (ItemSize)binaryReader.ReadInt32();
			}
		}
	}
}
