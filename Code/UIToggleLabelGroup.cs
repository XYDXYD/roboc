using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIToggleLabelGroup : MonoBehaviour, IChainListener
{
	public UIButton[] buttonGOs;

	public Color colour = Color.get_white();

	private UILabel _lastLabelDown;

	private UIButton _lastButtonDown;

	public UIToggleLabelGroup()
		: this()
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public void Listen(object message)
	{
		if (message is GameObject)
		{
			ToggleButtonDown(message as GameObject);
		}
	}

	public void SelectedButton(GameObject go)
	{
		ToggleButtonDown(go);
	}

	private void ToggleButtonDown(GameObject buttonGO)
	{
		_lastButtonDown = null;
		UIButton[] array = buttonGOs;
		foreach (UIButton val in array)
		{
			if (val.tweenTarget.get_gameObject() == buttonGO)
			{
				ToggleButton(val, state: true);
				_lastButtonDown = val;
			}
			else
			{
				ToggleButton(val, state: false);
			}
		}
	}

	private void ToggleButton(UIButton button, bool state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Color defaultColor = button.get_defaultColor();
		if (state)
		{
			defaultColor = colour;
		}
		UILabel component = button.tweenTarget.GetComponent<UILabel>();
		if (component != null)
		{
			if (state)
			{
				if (component.get_color() != button.hover)
				{
					component.set_color(defaultColor);
				}
				_lastLabelDown = component;
			}
			else
			{
				component.set_color(button.get_defaultColor());
			}
		}
		UISprite component2 = button.tweenTarget.GetComponent<UISprite>();
		if (component2 != null)
		{
			if (state)
			{
				component2.set_enabled(true);
			}
			else
			{
				component2.set_enabled(false);
			}
		}
	}

	private void LateUpdate()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (_lastLabelDown != null && Object.op_Implicit(_lastButtonDown) && _lastLabelDown.get_color() != _lastButtonDown.hover)
		{
			_lastLabelDown.set_color(colour);
		}
	}
}
