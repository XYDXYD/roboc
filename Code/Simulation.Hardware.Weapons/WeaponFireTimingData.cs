using Svelto.DataStructures;
using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponFireTimingData
	{
		public Dictionary<ItemDescriptor, float> currentCooldownBetweenShots = new Dictionary<ItemDescriptor, float>(48);

		public Dictionary<ItemDescriptor, float> refireTimeRemaining = new Dictionary<ItemDescriptor, float>(48);

		public Dictionary<ItemDescriptor, int> enabledWeaponCount = new Dictionary<ItemDescriptor, int>(48);

		public Dictionary<ItemDescriptor, FasterList<WeaponFireTimingNode>> weaponsBySubCategory = new Dictionary<ItemDescriptor, FasterList<WeaponFireTimingNode>>();

		public Dictionary<ItemDescriptor, MisfireTimingData> misfireData = new Dictionary<ItemDescriptor, MisfireTimingData>();

		public bool isLocalHumanPlayer
		{
			get;
			private set;
		}

		public WeaponFireTimingData(bool isLocalHumanPlayer_)
		{
			isLocalHumanPlayer = isLocalHumanPlayer_;
		}

		public void AddSubCategory(ItemDescriptor itemDescriptor)
		{
			enabledWeaponCount.Add(itemDescriptor, 0);
			weaponsBySubCategory.Add(itemDescriptor, new FasterList<WeaponFireTimingNode>());
			refireTimeRemaining[itemDescriptor] = 0f;
			currentCooldownBetweenShots[itemDescriptor] = 0f;
			misfireData.Add(itemDescriptor, new MisfireTimingData());
		}
	}
}
