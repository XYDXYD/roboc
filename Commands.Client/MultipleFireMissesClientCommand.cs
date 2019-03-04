using Events.Dependencies;
using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

namespace Commands.Client
{
	internal sealed class MultipleFireMissesClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private MultipleFireMissesDependency _dependency;

		[Inject]
		public PlayerTeamsContainer playerTeamContainer
		{
			private get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayersContainer
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
		public NetworkHitEffectObservable networkHitEffectObservable
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as MultipleFireMissesDependency);
			return this;
		}

		public void Execute()
		{
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			if (!playerMachinesContainer.IsMachineRegistered(TargetType.Player, _dependency.shootingMachineId))
			{
				return;
			}
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, _dependency.shootingMachineId);
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerFromMachineId))
			{
				bool isEnemy_ = !playerTeamContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId);
				for (int num = _dependency.numHits - 1; num >= 0; num--)
				{
					HitInfo hitInfo = new HitInfo(_dependency.targetTypeList.get_Item(num), _dependency.itemDescriptor, isEnemy_, _dependency.hitList.get_Item(num), _dependency.hitSelfList.get_Item(num), _dependency.hitPoints.get_Item(num), Quaternion.get_identity(), _dependency.hitNormals.get_Item(num), targetIsMe_: false, shooterIsMe_: false, targetOnMyTeam_: false, null, 0, playSound_: true, isMiss_: true);
					networkHitEffectObservable.Dispatch(ref hitInfo);
				}
			}
		}
	}
}
