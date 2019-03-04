using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class RadarModuleFeedbackNode : EntityView
	{
		public IRadarComponent radarComponent;

		public IHardwareOwnerComponent ownerComponent;

		public IRadarVFXComponent vfxComponent;

		public RadarModuleFeedbackNode()
			: this()
		{
		}
	}
}
