using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections;

namespace Simulation
{
	internal sealed class DestroyCubeNoEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DestroyCubeNoEffectDependency _dependency;

		[Inject]
		public DestructionManager destructionManager
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
			_dependency = (dependency as DestroyCubeNoEffectDependency);
			return this;
		}

		public void Execute()
		{
			DestroyCubes(_dependency);
		}

		private void DestroyCubes(DestroyCubeNoEffectDependency dependency)
		{
			DestroyCubesSingleHit(dependency);
		}

		private IEnumerator DestroyCubesDelayed(DestroyCubeNoEffectDependency dependency)
		{
			DestroyCubesSingleHit(dependency);
			yield return null;
		}

		private void DestroyCubesSingleHit(DestroyCubeNoEffectDependency destroyCubeDependency)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, destroyCubeDependency.shootingMachineId);
			int playerFromMachineId2 = playerMachinesContainer.GetPlayerFromMachineId(destroyCubeDependency.targetType, destroyCubeDependency.hitMachineId);
			bool targetIsMe = playerTeamsContainer.IsMe(destroyCubeDependency.targetType, playerFromMachineId2);
			destructionManager.PerformDestruction(destroyCubeDependency.hitCubeInfo, playerFromMachineId, destroyCubeDependency.hitMachineId, playerFromMachineId2, destroyCubeDependency.targetType, targetIsMe, playEffects: false, int.MaxValue);
		}
	}
}
