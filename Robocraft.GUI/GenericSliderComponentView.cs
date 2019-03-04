using System;
using UnityEngine;

namespace Robocraft.GUI
{
	public class GenericSliderComponentView : GenericComponentViewBase
	{
		private const float SLIDER_CONFIRM_INTERVAL = 0.5f;

		private UISliderAdapter _slider;

		private bool _valueToDispatch;

		private float _sliderValue;

		private float _lastSliderUpdate;

		public override void Setup()
		{
			base.Setup();
			_slider = this.GetComponentInChildren<UISliderAdapter>();
			_slider.Setup();
			UISliderAdapter slider = _slider;
			slider.OnSliderChanged = (Action<float>)Delegate.Combine(slider.OnSliderChanged, new Action<float>(OnSliderValueChanged));
			_lastSliderUpdate = 0f;
			_valueToDispatch = false;
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
			_slider.Hide();
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
			_slider.Show();
		}

		public void EnableSelf()
		{
			_slider.Enable();
		}

		public void DisableSelf()
		{
			_slider.Disable();
		}

		internal void OnSliderValueChanged(float newValue)
		{
			if (_sliderValue != newValue)
			{
				_sliderValue = newValue;
				_lastSliderUpdate = 0f;
				_valueToDispatch = true;
				(_controller as GenericSliderComponent).HandleValueChanging(_sliderValue);
			}
		}

		public void Update()
		{
			_lastSliderUpdate += Time.get_deltaTime();
			if (_lastSliderUpdate > 0.5f && _valueToDispatch && !Input.GetMouseButton(0))
			{
				_lastSliderUpdate = 0f;
				(_controller as GenericSliderComponent).HandleValueConfirmed(_sliderValue);
				_valueToDispatch = false;
			}
		}

		public void SetSliderValue(float value)
		{
			_sliderValue = value;
			_slider.SetSliderValue(value);
		}
	}
}
