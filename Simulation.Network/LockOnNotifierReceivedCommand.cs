using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation.Network
{
	internal sealed class LockOnNotifierReceivedCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private LockOnNotifierDependency _dependency;

		[Inject]
		public LockOnNotifierController lockOnNotifierController
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as LockOnNotifierDependency);
			return this;
		}

		public void Execute()
		{
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, _dependency.targetPlayerId);
			if (playerMachinesContainer.IsMachineRegistered(TargetType.Player, activeMachine))
			{
				lockOnNotifierController.SetPlayerLockAlert(_dependency.firingPlayerId, _dependency.targetPlayerId, _dependency.lockStage, _dependency.lockedCubePosition, _dependency.itemCategory, _dependency.itemSize);
			}
		}
	}
}
