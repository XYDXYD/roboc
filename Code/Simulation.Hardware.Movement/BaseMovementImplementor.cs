using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal class BaseMovementImplementor : MonoBehaviour, IVisibilityTracker, IVisibilityComponent, IItemDescriptorComponent, ICPUComponent, IMaxSpeedStatsComponent
	{
		private bool _isInRange = true;

		private ItemDescriptor _itemDescriptor;

		public ItemDescriptor itemDescriptor => _itemDescriptor;

		public bool offScreen
		{
			get;
			set;
		}

		public bool inRange => _isInRange;

		public bool isOffScreen
		{
			set
			{
				offScreen = value;
			}
		}

		public bool isInRange
		{
			get
			{
				return _isInRange;
			}
			set
			{
				_isInRange = value;
			}
		}

		public uint cpuRating
		{
			get;
			set;
		}

		public float horizontalMaxSpeed
		{
			get;
			set;
		}

		public float verticalMaxSpeed
		{
			get;
			set;
		}

		public float speedBoost
		{
			get;
			set;
		}

		public int minRequiredItems
		{
			get;
			set;
		}

		public float minRequiredItemsModifier
		{
			get;
			set;
		}

		public BaseMovementImplementor()
			: this()
		{
			offScreen = false;
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
		}

		internal void SetDescriptor(ItemDescriptor itemDescriptor)
		{
			_itemDescriptor = itemDescriptor;
		}
	}
}
