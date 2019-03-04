using RCNetwork.Events;
using System.IO;

internal class GameEndDependency : NetworkDependency
{
	public GameEndReason gameEndReason
	{
		get;
		private set;
	}

	public GameEndDependency(byte[] data)
		: base(data)
	{
	}

	public GameEndDependency(GameEndReason gameEndReason_)
	{
		gameEndReason = gameEndReason_;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)gameEndReason);
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
				gameEndReason = (GameEndReason)binaryReader.ReadByte();
			}
		}
	}
}
