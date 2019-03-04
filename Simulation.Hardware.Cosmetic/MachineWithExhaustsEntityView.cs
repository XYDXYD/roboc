using Simulation.Hardware.Movement;
using Svelto.ECS;

namespace Simulation.Hardware.Cosmetic
{
	internal class MachineWithExhaustsEntityView : EntityView
	{
		public IMachineWithExhaustsAudioComponent audioComponent;

		public IInputMotorComponent motorForwardsFeedbackComponent;

		public IAudioGameObjectComponent audioGoComponent;

		public MachineWithExhaustsEntityView()
			: this()
		{
		}
	}
}
