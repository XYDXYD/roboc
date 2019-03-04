using UnityEngine;

internal class TabButton : MonoBehaviour
{
	[SerializeField]
	private UIButton _label;

	[SerializeField]
	private UIButton _underline;

	private Color _labelNormal;

	private Color _labelHover;

	private Color _underlineNormal;

	private Color _underlineHover;

	public TabButton()
		: this()
	{
	}

	public void InitColours()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		_labelNormal = _label.get_defaultColor();
		_labelHover = _label.hover;
		_underlineNormal = _underline.get_defaultColor();
		_underlineHover = _underline.hover;
	}

	public void ResetColours()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		_label.set_defaultColor(_labelNormal);
		_label.hover = _labelHover;
		_label.UpdateColor(true);
		_underline.set_defaultColor(_underlineNormal);
		_underline.hover = _underlineHover;
		_underline.UpdateColor(true);
	}

	public void HighlightButton()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		_label.set_defaultColor(_label.pressed);
		_label.hover = _label.pressed;
		_label.UpdateColor(true);
		_underline.set_defaultColor(_underline.pressed);
		_underline.hover = _underline.pressed;
		_underline.UpdateColor(true);
	}
}
