namespace Simulation
{
	internal struct CubeHistoryEvent
	{
		public enum Type : byte
		{
			Damage,
			Destroy,
			Heal
		}

		public Byte3 gridLoc;

		public int damage;

		public Type type;

		public CubeHistoryEvent(Type pType, Byte3 pGridLoc, int pDamage)
		{
			type = pType;
			gridLoc = pGridLoc;
			damage = pDamage;
		}
	}
}
