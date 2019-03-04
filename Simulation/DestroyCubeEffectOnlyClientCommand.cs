using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class DestroyCubeEffectOnlyClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DestroyCubeEffectOnlyDependency _dependency;

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		public RemoteClientWeaponFire weaponFire
		{
			private get;
			set;
		}

		[Inject]
		public WeaponFireStateSync fireStateSync
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

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as DestroyCubeEffectOnlyDependency);
			return this;
		}

		public void Execute()
		{
			DestroyCubeEffect(_dependency);
		}

		private void DestroyCubeEffect(DestroyCubeEffectOnlyDependency destroyCubeDependency)
		{
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			if (machineRootContainer.IsMachineRegistered(TargetType.Player, destroyCubeDependency.hitMachineId))
			{
				bool targetIsMe = destroyCubeDependency.hitMachineId == playerMachinesContainer.GetActiveMachine(destroyCubeDependency.targetType, playerTeamsContainer.localPlayerId);
				if (WorldSwitching.IsMultiplayer())
				{
					weaponFire.FireRemoteClientWeapons(destroyCubeDependency.targetType, destroyCubeDependency.hitCube, targetIsMe, destroyCubeDependency.itemDescriptor, destroyCubeDependency.shootingMachineId, destroyCubeDependency.hitMachineId, destroyCubeDependency.hitEffectOffset, destroyCubeDependency.hitEffectNormal, destroyCubeDependency.stackCount);
				}
			}
		}
	}
}
