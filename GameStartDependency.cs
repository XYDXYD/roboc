using RCNetwork.Events;
using System.IO;

internal sealed class GameStartDependency : NetworkDependency
{
	public bool isReconnecting;

	public GameStartDependency(byte[] data)
		: base(data)
	{
	}

	public GameStartDependency(bool isReconnecting_)
	{
		isReconnecting = isReconnecting_;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(isReconnecting);
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
				isReconnecting = binaryReader.ReadBoolean();
			}
		}
	}
}
