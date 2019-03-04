using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	[RequireComponent(typeof(UIEventListener))]
	internal class GarageTooltip : MonoBehaviour
	{
		public float showDelayTime = 0.5f;

		public float showSpeed = 5f;

		public float hideSpeed = 8f;

		public GameObject tooltip;

		private UIWidget[] _tooltipWidgets;

		private bool _displayPopup;

		private bool _isFading;

		private bool _fadeIn;

		private float _displayStartTime;

		private float _hideStartAlpha = 1f;

		private float _hideStartTime;

		private float _currentAlpha;

		public GarageTooltip()
			: this()
		{
		}

		private void OnEnable()
		{
			_tooltipWidgets = tooltip.GetComponentsInChildren<UIWidget>();
			_currentAlpha = 0f;
			UpdateWidgetAlpha(_currentAlpha);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)UpdateTask);
		}

		private void OnDisable()
		{
			tooltip.SetActive(false);
		}

		public void OnHover(bool over)
		{
			_isFading = true;
			if (over)
			{
				_fadeIn = true;
			}
			else
			{
				_fadeIn = false;
			}
		}

		public void OnClick()
		{
			_isFading = true;
			_fadeIn = false;
		}

		private IEnumerator UpdateTask()
		{
			while (true)
			{
				if (_isFading)
				{
					Fade();
				}
				yield return null;
			}
		}

		private void Fade()
		{
			if (!_displayPopup && _fadeIn)
			{
				_displayPopup = true;
				tooltip.SetActive(true);
				_currentAlpha = 0f;
				_displayStartTime = Time.get_time();
			}
			if (_displayPopup && !_fadeIn)
			{
				_displayPopup = false;
				_hideStartAlpha = _currentAlpha;
				_hideStartTime = Time.get_time();
			}
			if (_fadeIn)
			{
				if (Time.get_time() - _displayStartTime > showDelayTime)
				{
					float num = (Time.get_time() - showDelayTime - _displayStartTime) * showSpeed;
					_currentAlpha = Mathf.Lerp(0f, 1f, num);
					UpdateWidgetAlpha(_currentAlpha);
				}
				if (_currentAlpha >= 1f)
				{
					_isFading = false;
				}
			}
			else
			{
				float num2 = (Time.get_time() - _hideStartTime) * hideSpeed;
				_currentAlpha = Mathf.Lerp(_hideStartAlpha, 0f, num2);
				UpdateWidgetAlpha(_currentAlpha);
				if (_currentAlpha <= 0f)
				{
					_isFading = false;
					tooltip.SetActive(false);
				}
			}
		}

		private void UpdateWidgetAlpha(float alpha)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < _tooltipWidgets.Length; i++)
			{
				Color color = _tooltipWidgets[i].get_color();
				color.a = alpha;
				_tooltipWidgets[i].set_color(color);
			}
		}
	}
}
