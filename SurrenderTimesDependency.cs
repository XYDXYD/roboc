using RCNetwork.Events;
using System.IO;

internal sealed class SurrenderTimesDependency : NetworkDependency
{
	public int playerCooldownSeconds;

	public int teamCooldownSeconds;

	public int surrenderTimeoutSeconds;

	public int initialSurrenderTimeoutSeconds;

	public SurrenderTimesDependency(int _playerCooldownSeconds, int _teamCooldownSeconds, int _surrenderTimeoutSeconds, int _initialSurrenderTimeoutSeconds)
	{
		playerCooldownSeconds = _playerCooldownSeconds;
		teamCooldownSeconds = _teamCooldownSeconds;
		surrenderTimeoutSeconds = _surrenderTimeoutSeconds;
		initialSurrenderTimeoutSeconds = _initialSurrenderTimeoutSeconds;
	}

	public SurrenderTimesDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(playerCooldownSeconds);
				binaryWriter.Write(teamCooldownSeconds);
				binaryWriter.Write(surrenderTimeoutSeconds);
				binaryWriter.Write(initialSurrenderTimeoutSeconds);
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
				playerCooldownSeconds = binaryReader.ReadInt32();
				teamCooldownSeconds = binaryReader.ReadInt32();
				surrenderTimeoutSeconds = binaryReader.ReadInt32();
				initialSurrenderTimeoutSeconds = binaryReader.ReadInt32();
			}
		}
	}
}
