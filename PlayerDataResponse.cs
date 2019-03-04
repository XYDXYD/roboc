using Services.Mothership;
using Svelto.DataStructures;

internal sealed class PlayerDataResponse
{
	public uint garageSlotId;

	public string name;

	public uint crfId;

	public uint robotCPU;

	public ControlSettings controlSetting = default(ControlSettings);

	public WeaponOrderMothership weaponOrder;

	public FasterList<ItemCategory> movementCategories;
}
