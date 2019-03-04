using System;

[Serializable]
internal class PopupMessageExtraDataPair
{
	public string cubeTypeInHexFormat;

	public string stringToShow;

	internal CubeTypeID stringKeyAsCubeTypeID
	{
		get;
		set;
	}
}
