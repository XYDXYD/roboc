using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(IUIProgressAdapter))]
	public class GenericProgressBarComponentView : GenericComponentViewBase
	{
		private IUIProgressAdapter _slider;

		public override void Setup()
		{
			base.Setup();
			_slider = this.GetComponent<IUIProgressAdapter>();
			_slider.Setup();
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void SetSliderValue(float value)
		{
			_slider.SetSliderValue(value);
		}
	}
}
