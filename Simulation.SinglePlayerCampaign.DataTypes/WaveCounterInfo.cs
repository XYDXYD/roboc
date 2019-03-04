namespace Simulation.SinglePlayerCampaign.DataTypes
{
	internal struct WaveCounterInfo
	{
		public readonly int WaveJustStartedIndex;

		public readonly int NumberOfWaves;

		public WaveCounterInfo(int waveJustStartedIndex, int numberOfWaves)
		{
			WaveJustStartedIndex = waveJustStartedIndex;
			NumberOfWaves = numberOfWaves;
		}
	}
}
