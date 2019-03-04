namespace Services.Simulation
{
	public sealed class WeaponOrderSimulation : WeaponOrder
	{
		public WeaponOrderSimulation(int[] weaponOrderList)
			: base(weaponOrderList)
		{
		}

		public int WeaponCount()
		{
			int num = 0;
			for (int i = 0; i < _order.get_Count(); i++)
			{
				if (_order.get_Item(i) != 0)
				{
					num++;
				}
			}
			return num;
		}
	}
}
