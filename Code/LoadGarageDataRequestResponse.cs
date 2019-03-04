using Svelto.DataStructures;

internal class LoadGarageDataRequestResponse
{
	public FasterList<GarageSlotDependency> garageSlots;

	public uint currentGarageSlot;

	public FasterList<int> garageSlotOrder;
}
