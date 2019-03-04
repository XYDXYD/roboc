using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class LocalMachineRotorNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IAverageMovementValuesComponent averageMovementValuesComponent;

		public ITimerComponent timerComponent;

		public ILocalCOMComponent localCOMComponent;

		public IRotorsGroundedComponent groundedComponent;

		public IRotorInputComponent inputComponent;

		public IMachineInputComponent inputWrapperComponent;

		public ICacheUpdateComponent cacheUpdateComponent;

		public IMachineFunctionalComponent functionalsEnabledComponent;

		public IMachineTiltComponent machineTiltComponent;

		public IMachineDriftComponent machineDriftComponent;

		public IRobotHeightComponent robotHeightComponent;

		public IMassComponent massComponent;

		public IForceOffsetComponent forceOffsetComponent;

		public IRotorPowerValueComponent powerValueComponent;

		public IAudioLiftingLoweringComponent audioLiftingLoweringComponent;

		public IMovementStoppingParamsComponent stoppingParamsComponent;

		public IMachineStunComponent machineStunComponent;

		public LocalMachineRotorNode()
			: this()
		{
		}
	}
}
