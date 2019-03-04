using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackAudioManagerNode : EntityView
	{
		public IRigidBodyComponent rigidBodyComponent;

		public ITankTrackAudioEventComponent audioEventComponent;

		public IAudioGameObjectComponent audioGoComponent;

		public ITankTrackAudioPlayingComponent tankTrackAudioPlayingComponent;

		public IMaxSpeedRatioComponent maxSpeedRatioComponent;

		public TankTrackAudioManagerNode()
			: this()
		{
		}
	}
}
