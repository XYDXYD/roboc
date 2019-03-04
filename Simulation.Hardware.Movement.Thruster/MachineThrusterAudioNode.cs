using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class MachineThrusterAudioNode : EntityView
	{
		public IAudioUpdateTimeComponent timeToUpdateComponent;

		public IThrusterAudioNamesComponent audioNamesComponent;

		public IAudioGameObjectComponent audioGoComponent;

		public IThrusterAudioStateComponent audioStateComponent;

		public IRampAndFadeTimeComponent rampAndFadeTimeComponent;

		public MachineThrusterAudioNode()
			: this()
		{
		}
	}
}
