using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.Components;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class WaveDataImplementor : IWaveDataComponent
	{
		public WaveData waveData
		{
			get;
			private set;
		}

		public WaveDataImplementor(WaveData waveData_)
		{
			waveData = waveData_;
		}
	}
}
