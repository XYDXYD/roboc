using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UISlider))]
	[RequireComponent(typeof(BoxCollider))]
	public class UISliderAdapter : MonoBehaviour
	{
		private UISlider _sliderBar;

		private BoxCollider[] _colliders;

		private UIButton[] _buttons;

		private Vector2 _lastResolution;

		private bool _enabled;

		private bool _shown;

		public Action<float> OnSliderChanged;

		public UISliderAdapter()
			: this()
		{
		}

		public void Setup()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			_colliders = this.GetComponentsInChildren<BoxCollider>();
			_sliderBar = this.GetComponent<UISlider>();
			_sliderBar.onChange.Add(new EventDelegate(this, "OnSliderChangedDelegate"));
			_buttons = this.GetComponentsInChildren<UIButton>();
			_enabled = true;
			_shown = true;
		}

		public void Show()
		{
			_shown = true;
			this.get_gameObject().SetActive(true);
			for (int i = 0; i < _buttons.Length; i++)
			{
				UIButton val = _buttons[i];
				if (_enabled)
				{
					val.SetState(0, true);
				}
			}
		}

		public void Hide()
		{
			_shown = false;
			this.get_gameObject().SetActive(false);
		}

		public void Enable()
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Invalid comparison between Unknown and I4
			_enabled = true;
			for (int i = 0; i < _colliders.Length; i++)
			{
				_colliders[i].set_enabled(true);
			}
			for (int j = 0; j < _buttons.Length; j++)
			{
				UIButton val = _buttons[j];
				if ((int)val.get_state() == 3)
				{
					val.set_state(0);
				}
			}
		}

		public void Disable()
		{
			_enabled = false;
			for (int i = 0; i < _colliders.Length; i++)
			{
				_colliders[i].set_enabled(false);
			}
			for (int j = 0; j < _buttons.Length; j++)
			{
				UIButton val = _buttons[j];
				val.set_state(3);
			}
		}

		public void OnSliderChangedDelegate()
		{
			OnSliderChanged(_sliderBar.get_value());
		}

		public void SetSliderValue(float value_)
		{
			value_ = Mathf.Clamp(value_, 0f, 1f);
			_sliderBar.Set(value_, false);
		}

		private void Update()
		{
			if (_lastResolution.x != (float)Screen.get_width() || _lastResolution.y != (float)Screen.get_height())
			{
				_lastResolution.x = Screen.get_width();
				_lastResolution.y = Screen.get_height();
				_sliderBar.ForceUpdate();
			}
		}
	}
}
