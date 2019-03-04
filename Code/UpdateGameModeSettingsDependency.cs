using RCNetwork.Events;
using System.IO;

internal class UpdateGameModeSettingsDependency : NetworkDependency
{
	public float RespawnHealDuration;

	public float RespawnFullHealDuration;

	public UpdateGameModeSettingsDependency(float respawnHealDuration, float respawnFullHealDuration)
	{
		RespawnHealDuration = respawnHealDuration;
		RespawnFullHealDuration = respawnFullHealDuration;
	}

	public UpdateGameModeSettingsDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(RespawnHealDuration);
				binaryWriter.Write(RespawnFullHealDuration);
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
				RespawnHealDuration = binaryReader.ReadSingle();
				RespawnFullHealDuration = binaryReader.ReadSingle();
			}
		}
	}
}
