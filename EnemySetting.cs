public struct EnemySetting
{
	public readonly float InitialHealthBoost;

	public readonly float IncreasePerWaveHealthBoost;

	public readonly float InitialDamageBoost;

	public readonly float IncreasePerWaveDamageBoost;

	public EnemySetting(float initialHealthBoost, float increasePerWaveHealthBoost, float initialDamageBoost, float increasePerWaveDamageBoost)
	{
		InitialHealthBoost = initialHealthBoost;
		IncreasePerWaveHealthBoost = increasePerWaveHealthBoost;
		InitialDamageBoost = initialDamageBoost;
		IncreasePerWaveDamageBoost = increasePerWaveDamageBoost;
	}
}
