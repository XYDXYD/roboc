using System;
using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericTooltipAreaPresenter : IPresenter
	{
		private GameObjectPool _pool;

		private GenericTooltipArea _view;

		private GameObject _activeTooltip;

		public GenericTooltipAreaPresenter(GameObjectPool pool)
		{
			_pool = pool;
		}

		public void SetView(GenericTooltipArea view)
		{
			_view = view;
		}

		internal void OnHover(bool isOver)
		{
			if (isOver && !string.IsNullOrEmpty(_view.text))
			{
				Show();
			}
			else
			{
				Hide();
			}
		}

		internal void OnClick()
		{
			Hide();
		}

		internal void OnTextChanged()
		{
			if (string.IsNullOrEmpty(_view.text))
			{
				Hide();
			}
		}

		public void SetActive(bool active)
		{
			if (!active)
			{
				Hide();
			}
		}

		private void Show()
		{
			if (!(_activeTooltip != null))
			{
				string text = _view.text;
				if (_view.localizeText)
				{
					text = StringTableBase<StringTable>.Instance.GetString(text);
				}
				UIWidget referenceWidget = _view.GetReferenceWidget();
				GameObject val = _pool.Use(_view.tooltipPrefab.get_name(), (Func<GameObject>)OnTooltipAllocation);
				SetParentWithoutRescale(val.get_transform(), GetTooltipParent());
				ITooltipView component = val.GetComponent<ITooltipView>();
				component.ShowTooltip(referenceWidget, text, _view.location);
				_activeTooltip = val;
			}
		}

		private void Hide()
		{
			if (_activeTooltip != null)
			{
				_activeTooltip.SetActive(false);
				SetParentWithoutRescale(_activeTooltip.get_transform(), null);
				_pool.Recycle(_activeTooltip, _view.tooltipPrefab.get_name());
				_activeTooltip = null;
			}
		}

		private GameObject OnTooltipAllocation()
		{
			return GenericWidgetFactory.InstantiateGui(_view.tooltipPrefab, null);
		}

		private Transform GetTooltipParent()
		{
			UIRoot componentInParent = _view.GetComponentInParent<UIRoot>();
			return componentInParent.get_transform();
		}

		private static void SetParentWithoutRescale(Transform obj, Transform parent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = obj.get_localScale();
			obj.set_parent(parent);
			obj.set_localScale(localScale);
		}
	}
}
