using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIToggleCheckboxGroup : MonoBehaviour, IChainListener
{
	public GameObject[] checkboxGOs;

	private GameObject _currentCheckbox;

	public UIToggleCheckboxGroup()
		: this()
	{
	}

	public int GetCurrentBox()
	{
		int num = 0;
		GameObject[] array = checkboxGOs;
		foreach (GameObject val in array)
		{
			if (val == _currentCheckbox)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public void Listen(object message)
	{
		if (message is GameObject)
		{
			ToggleBoxCheked(message as GameObject);
		}
	}

	private void ToggleBoxCheked(GameObject checkboxGO)
	{
		GameObject[] array = checkboxGOs;
		foreach (GameObject val in array)
		{
			if (val == checkboxGO)
			{
				ToggleCheckbox(val, state: true);
			}
			else
			{
				ToggleCheckbox(val, state: false);
			}
		}
		_currentCheckbox = checkboxGO;
	}

	private void ToggleCheckbox(GameObject go, bool state)
	{
		UIToggle[] components = go.GetComponents<UIToggle>();
		foreach (UIToggle val in components)
		{
			val.set_value(state);
		}
	}
}
