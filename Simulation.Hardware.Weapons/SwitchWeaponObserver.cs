using System;

namespace Simulation.Hardware.Weapons
{
	internal sealed class SwitchWeaponObserver
	{
		public event Action<int, ItemDescriptor> SwitchWeaponsEvent = delegate
		{
		};

		public event Action<int, ItemDescriptor> RemoteSwitchWeaponsEvent = delegate
		{
		};

		public event Action<CrosshairType> SwitchCrosshairEvent = delegate
		{
		};

		public void SwitchWeapons(int machineId, ItemDescriptor itemDescriptor)
		{
			this.SwitchWeaponsEvent(machineId, itemDescriptor);
		}

		public void RemoteSwitchWeapon(int machineId, ItemDescriptor itemDescriptor)
		{
			this.RemoteSwitchWeaponsEvent(machineId, itemDescriptor);
		}

		public void SwitchCrosshair(CrosshairType crosshairType)
		{
			this.SwitchCrosshairEvent(crosshairType);
		}
	}
}
