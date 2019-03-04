using RCNetwork.Events;
using System.IO;
using UnityEngine;

internal sealed class GameGuidDependency : NetworkDependency
{
	public string gameGuid
	{
		get;
		private set;
	}

	public string playerName
	{
		get;
		private set;
	}

	public uint entryTimeSeconds
	{
		get;
		private set;
	}

	public string playerIpAddress
	{
		get;
		private set;
	}

	public GameGuidDependency(byte[] data)
		: base(data)
	{
	}

	public GameGuidDependency(string _gameGuid, string _playerName, uint _entryTimeSeconds)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		gameGuid = _gameGuid;
		playerName = _playerName;
		entryTimeSeconds = _entryTimeSeconds;
		NetworkPlayer player = Network.get_player();
		playerIpAddress = player.get_externalIP();
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(gameGuid);
				binaryWriter.Write(playerName);
				binaryWriter.Write(entryTimeSeconds);
				binaryWriter.Write(playerIpAddress);
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
				gameGuid = binaryReader.ReadString();
				playerName = binaryReader.ReadString();
				entryTimeSeconds = binaryReader.ReadUInt32();
				playerIpAddress = binaryReader.ReadString();
			}
		}
	}
}
