using RCNetwork.Events;
using System.IO;

internal sealed class InitiateSurrenderDependency : NetworkDependency
{
	public int playerId;

	public float timeStamp;

	public InitiateSurrenderDependency(int _playerId, float _timeStamp)
	{
		playerId = _playerId;
		timeStamp = _timeStamp;
	}

	public InitiateSurrenderDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write((byte)playerId);
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
				timeStamp = binaryReader.ReadSingle();
			}
		}
	}
}
