using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericTooltipArea : MonoBehaviour, IView
	{
		public enum Location
		{
			TopLeft,
			Top,
			TopRight,
			Left,
			Center,
			Right,
			BottomLeft,
			Bottom,
			BottomRight
		}

		[SerializeField]
		private GameObject _tooltipTemplate;

		[SerializeField]
		private Location _location = Location.Center;

		[SerializeField]
		private string _text;

		[SerializeField]
		private bool _localizeText = true;

		private GenericTooltipAreaPresenter _presenter;

		internal bool localizeText => _localizeText;

		internal Location location => _location;

		internal GameObject tooltipPrefab => _tooltipTemplate;

		internal string text
		{
			get
			{
				return _text;
			}
			set
			{
				if (value != _text)
				{
					_text = value;
					_presenter.OnTextChanged();
				}
			}
		}

		public GenericTooltipArea()
			: this()
		{
		}

		private void Awake()
		{
		}

		internal void SetPresenter(GenericTooltipAreaPresenter presenter)
		{
			_presenter = presenter;
		}

		private void OnHover(bool isOver)
		{
			if (this.get_enabled())
			{
				_presenter.OnHover(isOver);
			}
		}

		private void OnClick()
		{
			_presenter.OnClick();
		}

		private void OnDisable()
		{
			if (_presenter != null)
			{
				_presenter.OnHover(isOver: false);
			}
		}

		internal UIWidget GetReferenceWidget()
		{
			return this.GetComponent<UIWidget>();
		}
	}
}
