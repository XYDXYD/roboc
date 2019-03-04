using Simulation;
using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System;
using UnityEngine;
using Utility;

internal sealed class PlayLocalWeaponFireEffectClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
{
	private WeaponFireEffectDependency _dependency;

	[Inject]
	public PlayerTeamsContainer playerTeamsContainer
	{
		private get;
		set;
	}

	[Inject]
	public WeaponListUtility weaponListUtility
	{
		private get;
		set;
	}

	[Inject]
	public NetworkWeaponFiredObservable networkWeaponFiredObservable
	{
		private get;
		set;
	}

	public IDispatchableCommand Inject(object dependency)
	{
		_dependency = (dependency as WeaponFireEffectDependency);
		return this;
	}

	public void Execute()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!playerTeamsContainer.IsMe(TargetType.Player, _dependency.shootingPlayerId))
		{
			try
			{
				GameObject weaponGameObject = weaponListUtility.GetWeaponGameObject(_dependency.weaponGridKey, _dependency.shootingMachineId);
				if (!WeaponIsDestroyed(weaponGameObject))
				{
					Vector3 targetPoint_ = _dependency.launchPosition + _dependency.direction;
					Vector3 direction = _dependency.direction;
					int instanceID = weaponGameObject.GetInstanceID();
					FiringInfo firingInfo = new FiringInfo(instanceID, direction, targetPoint_);
					networkWeaponFiredObservable.Dispatch(ref firingInfo);
				}
			}
			catch (Exception ex)
			{
				Console.LogException(ex);
			}
		}
	}

	private bool WeaponIsDestroyed(GameObject weapon)
	{
		return weapon == null;
	}
}
