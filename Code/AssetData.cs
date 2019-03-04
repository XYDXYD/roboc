using System;

[Serializable]
internal sealed class AssetData
{
	public string assetName;

	public long assetSize;

	public override string ToString()
	{
		return "Asset Name: " + assetName + " size: " + assetSize / 1024;
	}
}
