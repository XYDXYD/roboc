using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class RadarModuleEntityView : EntityView
	{
		public IModuleActivationComponent activationComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IRadarComponent radarComponent;

		public IRadarStatsComponent radarStatsComponent;

		public IModuleConfirmActivationComponent confirmActivationComponent;

		public RadarModuleEntityView()
			: this()
		{
		}
	}
}
