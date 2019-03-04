namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunGroupImplementor : ISharedChaingunSpinComponent, IWeaponSpinStatsComponent, IItemDescriptorComponent, IImplementor
	{
		private SharedSpinData _sharedSpinData;

		private ItemDescriptor _itemDescriptor;

		public SharedSpinData sharedSpinData => _sharedSpinData;

		public float spinUpTime
		{
			get;
			set;
		}

		public float spinDownTime
		{
			get;
			set;
		}

		public float spinInitialCooldown
		{
			get;
			set;
		}

		public ItemDescriptor itemDescriptor => _itemDescriptor;

		internal ChaingunGroupImplementor(ItemDescriptor itemDescriptor)
		{
			_sharedSpinData = new SharedSpinData();
			_itemDescriptor = itemDescriptor;
		}
	}
}
