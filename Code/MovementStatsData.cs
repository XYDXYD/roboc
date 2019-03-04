internal class MovementStatsData
{
	public float horizontalTopSpeed
	{
		get;
		private set;
	}

	public float verticalTopSpeed
	{
		get;
		private set;
	}

	public float speedBoost
	{
		get;
		private set;
	}

	public float maxCarryMass
	{
		get;
		private set;
	}

	public int minRequiredItems
	{
		get;
		private set;
	}

	public float minRequiredItemsModifier
	{
		get;
		private set;
	}

	public MovementStatsData(float horizontalTopSpeed, float verticalTopSpeed, float speedBoost, float maxCarryMass, int minRequiredItems, float minRequiredItemsModifier)
	{
		this.horizontalTopSpeed = horizontalTopSpeed;
		this.verticalTopSpeed = verticalTopSpeed;
		this.speedBoost = speedBoost;
		this.maxCarryMass = maxCarryMass;
		this.minRequiredItems = minRequiredItems;
		this.minRequiredItemsModifier = minRequiredItemsModifier;
	}
}
