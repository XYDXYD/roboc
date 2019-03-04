using Svelto.DataStructures;

namespace Simulation.BattleTracker
{
	public class PlayerKillData
	{
		public ItemCategory activeWeapon;

		public FasterList<ItemCategory> victimMovements;
	}
}
