using Simulation.Hardware.Weapons;
using Svelto.DataStructures;

namespace Simulation.Hardware
{
	internal sealed class SwitchWeaponData
	{
		public bool destroyed = true;

		public int order;

		public int machineId;

		public ItemDescriptor itemDescriptor;

		public int itemDescriptorKey;

		public CrosshairType crosshairType;

		public FasterList<WeaponSwitchNode> _weapons = new FasterList<WeaponSwitchNode>();

		private bool _active;

		public SwitchWeaponData(int machineId_, ItemDescriptor itemDescriptor_)
		{
			machineId = machineId_;
			itemDescriptor = itemDescriptor_;
			itemDescriptorKey = itemDescriptor.GenerateKey();
		}

		public void AddWeapon(WeaponSwitchNode weaponNode)
		{
			_weapons.Add(weaponNode);
			weaponNode.weaponActiveComponent.active = _active;
		}

		public void Activate(bool activate)
		{
			_active = activate;
			for (int num = _weapons.get_Count() - 1; num >= 0; num--)
			{
				WeaponSwitchNode weaponSwitchNode = _weapons.get_Item(num);
				weaponSwitchNode.weaponActiveComponent.active = activate;
			}
		}
	}
}
