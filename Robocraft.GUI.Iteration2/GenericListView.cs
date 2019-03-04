using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericListView : MonoBehaviour, IView
	{
		[SerializeField]
		private GameObject _itemTemplate;

		[SerializeField]
		private UIWidgetContainer _layout;

		private GenericListPresenter _presenter;

		internal UIWidgetContainer layout
		{
			set
			{
				_layout = value;
			}
		}

		internal GameObject itemTemplate
		{
			get
			{
				return _itemTemplate;
			}
			set
			{
				_itemTemplate = value;
			}
		}

		public GenericListView()
			: this()
		{
		}

		private void Awake()
		{
			LayoutUtility.CheckIsLayout(_layout);
			Transform itemsParent = GetItemsParent();
		}

		public void SetPresenter(GenericListPresenter presenter)
		{
			_presenter = presenter;
		}

		private void OnEnable()
		{
			if (_presenter != null)
			{
				_presenter.OnViewActive(active: true);
				Layout();
			}
		}

		private void OnDisable()
		{
			if (_presenter != null)
			{
				_presenter.OnViewActive(active: false);
			}
		}

		public void Layout()
		{
			LayoutUtility.ScheduleReposition(_layout);
		}

		internal Transform GetItemsParent()
		{
			return _layout.get_transform();
		}
	}
}
