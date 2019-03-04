using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections;

namespace Simulation
{
	internal sealed class DestroyCubeClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private DestroyCubeDependency _dependency;

		[Inject]
		public DestructionManager destructionManager
		{
			private get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
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
			_dependency = (dependency as DestroyCubeDependency);
			return this;
		}

		public void Execute()
		{
			int hitMachineId = _dependency.hitMachineId;
			DestroyCubes();
		}

		private void DestroyCubes()
		{
			DestroyCubesSingleHit(_dependency);
		}

		private IEnumerator DestroyCubesDelayed(DestroyCubeDependency destroyCubeDependency)
		{
			DestroyCubesSingleHit(destroyCubeDependency);
			yield return null;
		}

		private void DestroyCubesSingleHit(DestroyCubeDependency destroyCubeDependency)
		{
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			if (playerMachinesContainer.IsMachineRegistered(TargetType.Player, destroyCubeDependency.shootingMachineId))
			{
				fireStateSync.RecievedFireResponse(destroyCubeDependency.shootingMachineId, destroyCubeDependency.hitMachineId, destroyCubeDependency.timeStamp, destroyCubeDependency.targetType);
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, destroyCubeDependency.shootingMachineId);
				int playerFromMachineId2 = playerMachinesContainer.GetPlayerFromMachineId(destroyCubeDependency.targetType, destroyCubeDependency.hitMachineId);
				bool targetIsMe = playerTeamsContainer.IsMe(TargetType.Player, playerFromMachineId2);
				destructionManager.PerformDestruction(destroyCubeDependency.hitCubeInfo, playerFromMachineId, destroyCubeDependency.hitMachineId, playerFromMachineId2, destroyCubeDependency.targetType, targetIsMe, playEffects: true, destroyCubeDependency.weaponDamage, isReconnecting: true);
				if (WorldSwitching.IsMultiplayer())
				{
					RemoteClientWeaponFire weaponFire = this.weaponFire;
					TargetType targetType = destroyCubeDependency.targetType;
					HitCubeInfo hitCubeInfo = destroyCubeDependency.hitCubeInfo[0];
					weaponFire.FireRemoteClientWeapons(targetType, hitCubeInfo.gridLoc, targetIsMe, destroyCubeDependency.itemDescriptor, destroyCubeDependency.shootingMachineId, destroyCubeDependency.hitMachineId, destroyCubeDependency.hitEffectOffset, destroyCubeDependency.hitEffectNormal, destroyCubeDependency.stackCount);
				}
			}
		}
	}
}
