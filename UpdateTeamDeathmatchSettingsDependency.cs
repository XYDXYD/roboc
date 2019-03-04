using RCNetwork.Events;
using System.IO;

internal sealed class UpdateTeamDeathmatchSettingsDependency : NetworkDependency
{
	public GameModeSettings settings;

	public UpdateTeamDeathmatchSettingsDependency(GameModeSettings settings)
	{
		this.settings = settings;
	}

	public UpdateTeamDeathmatchSettingsDependency(byte[] data)
		: base(data)
	{
	}

	public override byte[] Serialise()
	{
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(settings.gameTimeMinutes);
				binaryWriter.Write(settings.killLimit.Value);
				binaryWriter.Write(settings.respawnHealDuration);
				binaryWriter.Write(settings.respawnFullHealDuration);
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
				int gameTimeMinutes_ = binaryReader.ReadInt32();
				int value = binaryReader.ReadInt32();
				float respawnHealDuration_ = binaryReader.ReadSingle();
				float respawnFullHealDuration_ = binaryReader.ReadSingle();
				settings = new GameModeSettings(respawnHealDuration_, respawnFullHealDuration_, gameTimeMinutes_)
				{
					killLimit = value
				};
			}
		}
	}
}
