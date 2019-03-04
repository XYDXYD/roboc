using Simulation.Hardware.Weapons;
using Svelto.ECS.Legacy;
using UnityEngine;

namespace Simulation.Hardware
{
	internal class BaseModuleImplementor : MonoBehaviour, IItemDescriptorComponent, IMachineDimensionDataComponent, IWeaponFireCostComponent, IWeaponCooldownComponent, IModuleActivationComponent, IModuleGuiCooldownComponent, IModuleRangeComponent, IModuleConfirmActivationComponent
	{
		private Dispatcher<IModuleGuiCooldownComponent, int> _startCooldown;

		private Dispatcher<IModuleGuiCooldownComponent, ItemCategory> _resetCooldown;

		private Dispatcher<IModuleGuiCooldownComponent, ItemCategory> _notEnoughPower;

		private Dispatcher<IModuleGuiCooldownComponent, ItemCategory> _cooldownStillActive;

		private Dispatcher<IModuleConfirmActivationComponent, int> _activationConfirmed;

		private Dispatcher<IModuleActivationComponent, int> _activate;

		private ItemDescriptor _itemDescriptor;

		private Vector3 _machineSize;

		private Vector3 _machineCenter;

		float IWeaponCooldownComponent.weaponCooldown
		{
			get;
			set;
		}

		float IWeaponFireCostComponent.weaponFireCost
		{
			get;
			set;
		}

		ItemDescriptor IItemDescriptorComponent.itemDescriptor
		{
			get
			{
				return _itemDescriptor;
			}
		}

		Vector3 IMachineDimensionDataComponent.machineSize
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _machineSize;
			}
		}

		Vector3 IMachineDimensionDataComponent.machineCenter
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _machineCenter;
			}
		}

		Dispatcher<IModuleActivationComponent, int> IModuleActivationComponent.activate
		{
			get
			{
				return _activate;
			}
		}

		Dispatcher<IModuleGuiCooldownComponent, int> IModuleGuiCooldownComponent.startCooldown
		{
			get
			{
				return _startCooldown;
			}
		}

		Dispatcher<IModuleGuiCooldownComponent, ItemCategory> IModuleGuiCooldownComponent.resetCooldown
		{
			get
			{
				return _resetCooldown;
			}
		}

		Dispatcher<IModuleGuiCooldownComponent, ItemCategory> IModuleGuiCooldownComponent.notEnoughPower
		{
			get
			{
				return _notEnoughPower;
			}
		}

		Dispatcher<IModuleGuiCooldownComponent, ItemCategory> IModuleGuiCooldownComponent.cooldownStillActive
		{
			get
			{
				return _cooldownStillActive;
			}
		}

		float IModuleRangeComponent.moduleRange
		{
			get;
			set;
		}

		public Dispatcher<IModuleConfirmActivationComponent, int> activationConfirmed => _activationConfirmed;

		public BaseModuleImplementor()
			: this()
		{
		}

		private void Awake()
		{
			if (this.get_gameObject().GetComponent<HardwareWorkingOrderImplementor>() == null)
			{
				this.get_gameObject().AddComponent<HardwareWorkingOrderImplementor>();
			}
			if (this.get_gameObject().GetComponent<ComponentTransformImplementor>() == null)
			{
				this.get_gameObject().AddComponent<ComponentTransformImplementor>();
			}
			_startCooldown = new Dispatcher<IModuleGuiCooldownComponent, int>(this);
			_resetCooldown = new Dispatcher<IModuleGuiCooldownComponent, ItemCategory>(this);
			_notEnoughPower = new Dispatcher<IModuleGuiCooldownComponent, ItemCategory>(this);
			_cooldownStillActive = new Dispatcher<IModuleGuiCooldownComponent, ItemCategory>(this);
			_activate = new Dispatcher<IModuleActivationComponent, int>(this);
			_activationConfirmed = new Dispatcher<IModuleConfirmActivationComponent, int>(this);
		}

		public void SetDescriptor(ItemDescriptor itemDescriptor)
		{
			_itemDescriptor = itemDescriptor;
		}

		public void SetMachineDimensionData(Vector3 machineSize, Vector3 machineCenter)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			_machineSize = machineSize;
			_machineCenter = machineCenter;
		}
	}
}
