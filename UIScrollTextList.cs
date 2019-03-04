using UnityEngine;

internal class UIScrollTextList : MonoBehaviour
{
	public BetterUITextList textList;

	public UIScrollTextList()
		: this()
	{
	}

	private void Awake()
	{
	}

	private void OnScroll(float val)
	{
		int scrollHeight = textList.GetScrollHeight();
		if (scrollHeight != 0)
		{
			val *= textList.GetLineHeight();
			textList.set_scrollValue(textList.get_scrollValue() - val / (float)scrollHeight);
		}
	}
}
