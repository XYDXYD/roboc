using System.IO;

public struct CampaignDifficultySetting
{
	public readonly int Level;

	public readonly PlayerSetting PlayerDifficultySetting;

	public readonly EnemySetting EnemyDifficultySettings;

	public CampaignDifficultySetting(byte[] data)
	{
		using (MemoryStream input = new MemoryStream(data))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				Level = binaryReader.ReadInt32();
				int totalLives = binaryReader.ReadInt32();
				bool autoHeal = binaryReader.ReadBoolean();
				int singleWaveCompletionBonus = binaryReader.ReadInt32();
				float initialHealthBoost = binaryReader.ReadSingle();
				float increasePerWaveHealthBoost = binaryReader.ReadSingle();
				float initialDamageBoost = binaryReader.ReadSingle();
				float increasePerWaveDamageBoost = binaryReader.ReadSingle();
				PlayerDifficultySetting = new PlayerSetting(totalLives, autoHeal, singleWaveCompletionBonus);
				EnemyDifficultySettings = new EnemySetting(initialHealthBoost, increasePerWaveHealthBoost, initialDamageBoost, increasePerWaveDamageBoost);
			}
		}
	}
}
