using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UISlider))]
	public class UIProgressBarAdapter : MonoBehaviour, IUIProgressAdapter
	{
		private UISlider _sliderBar;

		public UIProgressBarAdapter()
			: this()
		{
		}

		public void Setup()
		{
			_sliderBar = this.GetComponent<UISlider>();
		}

		public void SetSliderValue(float value_)
		{
			value_ = Mathf.Clamp(value_, 0f, 1f);
			_sliderBar.set_value(value_);
		}
	}
}
