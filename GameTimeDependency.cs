using RCNetwork.Events;
using System.IO;

internal sealed class GameTimeDependency : NetworkDependency
{
	public float time;

	public GameTimeDependency(byte[] data)
		: base(data)
	{
	}

	public GameTimeDependency(float _time)
	{
		time = _time;
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(time);
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
				time = binaryReader.ReadSingle();
			}
		}
	}
}
