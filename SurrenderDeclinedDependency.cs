using RCNetwork.Events;
using System.IO;

internal sealed class SurrenderDeclinedDependency : NetworkDependency
{
	public int _surrenderingPlayerId;

	public float _gameTimeElapsed;

	public SurrenderDeclinedDependency(int surrenderingPlayerId, float gameTimeElapsed)
	{
		_surrenderingPlayerId = surrenderingPlayerId;
		_gameTimeElapsed = gameTimeElapsed;
	}

	public SurrenderDeclinedDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(_surrenderingPlayerId);
				binaryWriter.Write(_gameTimeElapsed);
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
				_surrenderingPlayerId = binaryReader.ReadInt32();
				_gameTimeElapsed = binaryReader.ReadSingle();
			}
		}
	}
}
