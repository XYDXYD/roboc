using Services.Mothership;
using Svelto.DataStructures;

internal sealed class GarageSlotDependency
{
	public uint garageSlot;

	public string name;

	public uint numberCubes;

	public bool canBeRated;

	public uint crfId;

	public FasterList<ItemCategory> movementCategories;

	public uint totalRobotCPU;

	public uint totalCosmeticCPU;

	public uint totalRobotRanking;

	public UniqueSlotIdentifier uniqueSlotId;

	public uint remoteThumbnailVersionNumber;

	public ControlSettings controlSetting = default(ControlSettings);

	public bool tutorialRobot;

	public int starterRobotIndex;

	public int masteryLevel;

	public string baySkinID;

	internal WeaponOrderMothership weaponOrder;

	public GarageSlotDependency(uint _garageSlot)
	{
		garageSlot = _garageSlot;
		uniqueSlotId = new UniqueSlotIdentifier();
		starterRobotIndex = -1;
	}
}
