using Svelto.UI.Comms.SignalChain;
using UnityEngine;

internal sealed class UIResetSliderListener : MonoBehaviour, IChainListener
{
	public float resetValue = 1f;

	private UISlider _slider;

	public UIResetSliderListener()
		: this()
	{
	}

	private void Start()
	{
		_slider = this.GetComponent<UISlider>();
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.ResetSlider && _slider != null)
			{
				_slider.set_value(resetValue);
			}
		}
	}
}
