using UnityEngine;

internal class BetterUITextList : UITextList
{
	public BetterUITextList()
		: this()
	{
	}

	public void AddWithoutRebuild(string text)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		Paragraph val = null;
		if (this.get_paragraphs().size < base.paragraphHistory)
		{
			val = new Paragraph();
		}
		else
		{
			val = this.get_paragraphs().get_Item(0);
			this.get_paragraphs().RemoveAt(0);
		}
		val.text = text;
		this.get_paragraphs().Add(val);
	}

	public void ClearWithoutRebuild()
	{
		this.get_paragraphs().Clear();
	}

	public void Rebuild()
	{
		this.Rebuild();
	}

	public void OnDrag(Vector2 delta)
	{
	}

	public int GetScrollHeight()
	{
		return this.get_scrollHeight();
	}

	public float GetLineHeight()
	{
		return this.get_lineHeight();
	}
}
