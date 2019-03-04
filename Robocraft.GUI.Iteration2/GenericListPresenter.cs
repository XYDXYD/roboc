using System.Collections.Generic;
using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericListPresenter : IDataPresenter, IPresenter
	{
		private GenericListView _view;

		private IDataSource _dataSource;

		private IItemFactory _itemFactory;

		private List<IItemPresenter> _items = new List<IItemPresenter>();

		public void SetView(GenericListView view)
		{
			_view = view;
		}

		public void SetDataSource(IDataSource ds)
		{
			if (_dataSource != ds)
			{
				if (_dataSource != null && _dataSource is INotifyDataChanged)
				{
					((INotifyDataChanged)_dataSource).onAllDataChanged -= UpdateFromSource;
				}
				_dataSource = ds;
				if (_dataSource is INotifyDataChanged)
				{
					((INotifyDataChanged)_dataSource).onAllDataChanged += UpdateFromSource;
				}
			}
		}

		public void SetItemFactory(IItemFactory itemFactory)
		{
			_itemFactory = itemFactory;
		}

		internal void OnViewActive(bool active)
		{
			int num = Mathf.Min(_items.Count, _dataSource.NumberOfDataItemsAvailable(0));
			for (int i = 0; i < num; i++)
			{
				IItemPresenter itemPresenter = _items[i];
				itemPresenter.SetActive(active);
			}
		}

		public void SetActive(bool active)
		{
			_view.get_gameObject().SetActive(active);
		}

		public void UpdateFromSource()
		{
			for (int i = _dataSource.NumberOfDataItemsAvailable(0); i < _items.Count; i++)
			{
				IItemPresenter itemPresenter = _items[i];
				itemPresenter.SetActive(active: false);
			}
			Transform itemsParent = _view.GetItemsParent();
			for (int j = _items.Count; j < _dataSource.NumberOfDataItemsAvailable(0); j++)
			{
				IItemPresenter item = _itemFactory.Build(_view.itemTemplate, itemsParent);
				_items.Add(item);
			}
			int num = 0;
			if (_view.get_transform() != itemsParent)
			{
				num = _view.get_gameObject().get_transform().GetSiblingIndex() + 1;
			}
			for (int k = 0; k < _dataSource.NumberOfDataItemsAvailable(0); k++)
			{
				IItemPresenter itemPresenter2 = _items[k];
				itemPresenter2.SetActive(_view.get_gameObject().get_activeSelf());
				itemPresenter2.SetDataSource(_dataSource);
				itemPresenter2.SetDataSourceIndex(k);
				itemPresenter2.SetSiblingIndex(num + k);
				itemPresenter2.UpdateFromSource();
			}
			_view.Layout();
		}
	}
}
