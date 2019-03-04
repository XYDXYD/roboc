using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackAudioNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IPartLevelComponent partLevelComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IMaxSpeedComponent maxSpeedComponent;

		public IMaxTurnRateStoppedComponent maxTurnRateComponent;

		public IVisibilityComponent visibilityComponent;

		public ITrackSpeedComponent trackSpeedComponent;

		public ITrackTurnSpeedComponent trackTurnSpeedComponent;

		public TankTrackAudioNode()
			: this()
		{
		}
	}
}
