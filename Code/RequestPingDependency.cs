using RCNetwork.Events;
using System.IO;

internal sealed class RequestPingDependency : NetworkDependency
{
	public float timeStamp;

	public int playerId;

	public int requester;

	public RequestPingDependency(byte[] data)
		: base(data)
	{
	}

	public RequestPingDependency(float _time, int _playerId)
	{
		timeStamp = _time;
		playerId = _playerId;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)playerId);
				binaryWriter.Write((byte)requester);
				binaryWriter.Write(timeStamp);
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
				requester = binaryReader.ReadByte();
				timeStamp = binaryReader.ReadSingle();
			}
		}
	}
}
