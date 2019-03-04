public class RobotPartData
{
	public string RobotId;

	public string NameStrKey;

	public string RobotClass;

	public string CategoryStrKey;

	public byte[] Data;

	public byte[] ColourData;

	public RobotPartData(string robotId, string nameStrKey, string robotClass, string categoryStrKey, byte[] data, byte[] colourData)
	{
		RobotId = robotId;
		NameStrKey = nameStrKey;
		RobotClass = robotClass;
		CategoryStrKey = categoryStrKey;
		Data = data;
		ColourData = colourData;
	}
}
