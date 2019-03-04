using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;

internal sealed class FireMissClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private FireMissDependency _dependency;

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
		_dependency = (dependency as FireMissDependency);
		return this;
	}

	public void Execute()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (playerMachinesContainer.IsMachineRegistered(TargetType.Player, _dependency.shootingMachineId))
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, _dependency.shootingMachineId);
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerFromMachineId))
			{
				bool isEnemy_ = !playerTeamContainer.IsOnMyTeam(TargetType.Player, playerFromMachineId);
				Quaternion rotation_ = Quaternion.LookRotation(_dependency.hitNormal);
				HitInfo hitInfo = new HitInfo(_dependency.targetType, _dependency.itemDescriptor, isEnemy_, _dependency.hit, _dependency.hitSelf, _dependency.hitPoint, rotation_, _dependency.hitNormal, targetIsMe_: false, shooterIsMe_: false, targetOnMyTeam_: false, null, 0, playSound_: true, isMiss_: true);
				networkHitEffectObservable.Dispatch(ref hitInfo);
			}
		}
	}
}
