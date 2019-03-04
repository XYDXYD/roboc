using Svelto.DataStructures;

namespace Services.Mothership
{
	internal sealed class GarageSlotData
	{
		public string name;

		public MachineModel machineModel;

		public FasterList<ItemCategory> movementCategories;

		public uint cubesNumber;

		public byte[] colorMap;

		public UniqueSlotIdentifier uniqueId;

		public WeaponOrderMothership weaponOrder;

		public ControlSettings controlSetting;

		public uint totalRobotCPU;

		public uint totalCosmeticCPU;

		public uint crfId;

		public bool isReadOnlyRobot;

		public int masteryLevel;

		public bool tutorialRobot;

		public int starterRobotIndex;

		public string baySkinID;

		public GarageSlotData()
		{
			isReadOnlyRobot = false;
			tutorialRobot = false;
			starterRobotIndex = -1;
		}
	}
}
