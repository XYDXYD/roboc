using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UISprite))]
	public class UIProgressCircleAdapter : MonoBehaviour, IUIProgressAdapter
	{
		private UISprite _filledCircle;

		public UIProgressCircleAdapter()
			: this()
		{
		}

		public void Setup()
		{
			_filledCircle = this.GetComponent<UISprite>();
		}

		public void SetSliderValue(float value_)
		{
			value_ = Mathf.Clamp(value_, 0f, 1f);
			_filledCircle.set_fillAmount(value_);
		}
	}
}
