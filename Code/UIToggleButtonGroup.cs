using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIToggleButtonGroup : MonoBehaviour, IChainListener
{
	public GameObject[] buttonGOs;

	public bool usePressedColor;

	public bool startWithFirstActive = true;

	public bool resetOnEnable;

	public bool useDefaultOnDisabled;

	private int _lastButtonDown = -1;

	private bool _disabled;

	public UIToggleButtonGroup()
		: this()
	{
	}

	public void DisableAllExcept(GameObject target)
	{
		for (int i = 0; i < buttonGOs.Length; i++)
		{
			if (buttonGOs[i] != target)
			{
				UIButton[] componentsInChildren = buttonGOs[i].GetComponentsInChildren<UIButton>();
				UIButton[] array = componentsInChildren;
				foreach (UIButton val in array)
				{
					val.SetState(3, true);
					val.set_isEnabled(false);
				}
			}
		}
		_disabled = true;
	}

	public void Listen(object message)
	{
		if (!_disabled && message is GameObject)
		{
			ToggleButtonDown(message as GameObject);
		}
	}

	public void OnEnable()
	{
		if (resetOnEnable)
		{
			_lastButtonDown = -1;
			GameObject[] array = buttonGOs;
			foreach (GameObject go in array)
			{
				ToggleButton(go, state: false);
			}
		}
	}

	private void ToggleButtonDown(GameObject buttonGO)
	{
		if (_disabled)
		{
			return;
		}
		bool flag = false;
		_lastButtonDown = 0;
		GameObject[] array = buttonGOs;
		foreach (GameObject val in array)
		{
			if (val != null && val == buttonGO)
			{
				ToggleButton(val, state: true);
				flag = true;
			}
			else
			{
				ToggleButton(val, state: false);
			}
			if (!flag)
			{
				_lastButtonDown++;
			}
		}
	}

	private void ToggleButton(GameObject go, bool state)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		UIButton[] components = go.GetComponents<UIButton>();
		foreach (UIButton val in components)
		{
			Color color = state ? ((!usePressedColor) ? val.hover : val.pressed) : ((!val.get_isEnabled() && !useDefaultOnDisabled) ? val.disabledColor : val.get_defaultColor());
			if (val.tweenTarget != null)
			{
				if (val.tweenTarget.GetComponent<UISprite>() != null)
				{
					val.tweenTarget.GetComponent<UISprite>().set_color(color);
				}
				if (val.tweenTarget.GetComponent<UILabel>() != null)
				{
					val.tweenTarget.GetComponent<UILabel>().set_color(color);
				}
			}
		}
	}

	private void LateUpdate()
	{
		if (!_disabled)
		{
			if (_lastButtonDown == -1 && startWithFirstActive)
			{
				_lastButtonDown = 0;
			}
			if (_lastButtonDown >= 0 && _lastButtonDown < buttonGOs.Length)
			{
				ToggleButton(buttonGOs[_lastButtonDown], state: true);
			}
		}
	}
}
