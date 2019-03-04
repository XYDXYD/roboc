internal class SpecialItemListData
{
	public string keyName;

	public string spriteName;

	public uint motherhsipSize;

	public SpecialItemListData(string name, string spriteName, uint motherhsipSize)
	{
		keyName = name;
		this.spriteName = spriteName;
		this.motherhsipSize = motherhsipSize;
	}
}
