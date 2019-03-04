using System;

[Serializable]
internal sealed class PopupMessageInfo
{
	public InvalidPlacementType type;

	public string Text = string.Empty;

	public float VisibleTime = 2f;

	public PopupMessageExtraDataPair[] stringOverridesByCubeType;
}
