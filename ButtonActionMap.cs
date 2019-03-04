using System.Collections.Generic;

internal class ButtonActionMap
{
	public int catecoryId;

	public string categoryName;

	public Dictionary<int, ButtonReassignData> map = new Dictionary<int, ButtonReassignData>();
}
