using RCNetwork.Events;
using System;
using System.IO;

internal sealed class SetFinalGameScoreDependency : NetworkDependency
{
	public int playerId
	{
		get;
		private set;
	}

	public uint score
	{
		get;
		private set;
	}

	public SetFinalGameScoreDependency(int _playerId, uint _score)
	{
		playerId = _playerId;
		score = _score;
	}

	public SetFinalGameScoreDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Convert.ToByte(playerId));
				binaryWriter.Write(score);
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
				playerId = Convert.ToInt32(binaryReader.ReadByte());
				score = binaryReader.ReadUInt32();
			}
		}
	}
}
