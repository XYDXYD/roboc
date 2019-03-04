internal class TutorialEnemyMachineData
{
	public MachineModel model;

	public byte[] colourData;

	public TutorialEnemyMachineData(MachineModel model, byte[] colourData)
	{
		this.model = model;
		this.colourData = colourData;
	}

	public TutorialEnemyMachineData(TutorialEnemyMachineData original)
	{
		model = original.model;
		colourData = original.colourData;
	}
}
