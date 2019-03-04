using Services.Mothership;

internal class SaveMachineDependency
{
	public MachineModel model;

	public WeaponOrderMothership weaponOrder;

	public SaveMachineDependency(MachineModel m, WeaponOrderMothership w)
	{
		model = new MachineModel(m);
		weaponOrder = w;
	}
}
