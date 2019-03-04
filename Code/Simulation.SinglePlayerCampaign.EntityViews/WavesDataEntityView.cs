using Simulation.SinglePlayerCampaign.Components;
using Svelto.ECS;

namespace Simulation.SinglePlayerCampaign.EntityViews
{
	internal class WavesDataEntityView : EntityView
	{
		public IWavesDataComponent wavesData;

		public WavesDataEntityView()
			: this()
		{
		}
	}
}
