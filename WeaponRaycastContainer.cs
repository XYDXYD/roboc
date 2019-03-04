using System.Collections.Generic;

internal class WeaponRaycastContainer
{
	private Dictionary<int, WeaponRaycast> _weaponRaycasts = new Dictionary<int, WeaponRaycast>();

	public void RegisterWeaponRaycast(int machineId, WeaponRaycast weaponRaycast)
	{
		_weaponRaycasts.Add(machineId, weaponRaycast);
	}

	public void UnregisterWeaponRaycast(int machineId)
	{
		_weaponRaycasts.Remove(machineId);
	}

	public WeaponRaycast GetWeaponRaycast(int machineId)
	{
		return _weaponRaycasts[machineId];
	}
}
