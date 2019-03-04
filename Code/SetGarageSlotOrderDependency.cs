using Svelto.DataStructures;

internal class SetGarageSlotOrderDependency
{
	public FasterList<int> currentGarageSlotOrder
	{
		get;
		set;
	}

	public SetGarageSlotOrderDependency(FasterList<int> garageSlotOrder)
	{
		currentGarageSlotOrder = garageSlotOrder;
	}
}
