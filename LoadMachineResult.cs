using Services.Mothership;
using Svelto.DataStructures;

internal class LoadMachineResult
{
	public ControlSettings controlSettings;

	public uint cubesNumber;

	public uint garageSlot;

	public bool isReadOnlyRobot;

	public MachineModel model;

	public FasterList<ItemCategory> movementCategories;

	public WeaponOrderMothership weaponOrder;

	public int MasteryLevel;
}
