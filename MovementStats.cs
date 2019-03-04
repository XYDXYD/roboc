using System.Collections.Generic;

internal class MovementStats
{
	public float lerpValue
	{
		get;
		private set;
	}

	public IDictionary<int, MovementStatsData> data
	{
		get;
		private set;
	}

	public IDictionary<ItemCategory, IMovementCategoryData> categoryData
	{
		get;
		private set;
	}

	public MovementStats(float lerpValue, IDictionary<int, MovementStatsData> data, IDictionary<ItemCategory, IMovementCategoryData> categoryData)
	{
		this.lerpValue = lerpValue;
		this.data = data;
		this.categoryData = categoryData;
	}
}
