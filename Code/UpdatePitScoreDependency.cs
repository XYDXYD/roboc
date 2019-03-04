using RCNetwork.Events;
using System.IO;

internal sealed class UpdatePitScoreDependency : NetworkDependency
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

	public uint streak
	{
		get;
		private set;
	}

	public int destroyedId
	{
		get;
		private set;
	}

	public int leaderId
	{
		get;
		private set;
	}

	public UpdatePitScoreDependency(int playerId, uint score, uint streak, int destroyedId, int leaderId)
	{
		this.playerId = playerId;
		this.score = score;
		this.streak = streak;
		this.destroyedId = destroyedId;
		this.leaderId = leaderId;
	}

	public UpdatePitScoreDependency(byte[] data)
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
				binaryWriter.Write(score);
				binaryWriter.Write(streak);
				binaryWriter.Write((byte)destroyedId);
				binaryWriter.Write((byte)leaderId);
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
				score = binaryReader.ReadUInt32();
				streak = binaryReader.ReadUInt32();
				destroyedId = binaryReader.ReadByte();
				leaderId = binaryReader.ReadByte();
			}
		}
	}
}
