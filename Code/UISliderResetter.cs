using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System.Collections;
using UnityEngine;

internal class UISliderResetter : MonoBehaviour, IChainListener
{
	public float resetValue;

	public bool resetOnEnable = true;

	private UISlider _slider;

	public UISliderResetter()
		: this()
	{
	}

	private void Awake()
	{
		_slider = this.GetComponent<UISlider>();
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.ResetSlider)
			{
				ResetSlider();
			}
		}
	}

	private void OnEnable()
	{
		if (resetOnEnable)
		{
			ResetSlider();
		}
	}

	private void ResetSlider()
	{
		if (_slider != null)
		{
			TaskRunner.get_Instance().Run(ResetSliderWithDelay());
		}
	}

	private IEnumerator ResetSliderWithDelay()
	{
		yield return null;
		_slider.set_value(resetValue);
		_slider.ForceUpdate();
	}
}
