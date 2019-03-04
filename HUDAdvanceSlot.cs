using UnityEngine;

public class HUDAdvanceSlot : MonoBehaviour
{
	public GameObject normal;

	public UILabel normalTitle;

	public UILabel normalValue;

	public GameObject indented;

	public UILabel indentedTitle;

	public UILabel indentedValue;

	public HUDAdvanceSlot()
		: this()
	{
	}

	public void SetIsIndented(bool isIndented)
	{
		normal.SetActive(!isIndented);
		indented.SetActive(isIndented);
	}

	public void SetTitle(string title)
	{
		UILabel obj = normalTitle;
		indentedTitle.set_text(title);
		obj.set_text(title);
	}

	public void SetValue(string value)
	{
		UILabel obj = normalValue;
		indentedValue.set_text(value);
		obj.set_text(value);
	}
}
