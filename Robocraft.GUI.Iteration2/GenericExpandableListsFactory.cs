using System.Collections.Generic;
using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal class GenericExpandableListsFactory
	{
		private List<IItemFactory> _itemFactories = new List<IItemFactory>();

		private List<IDataSource> _dataSources = new List<IDataSource>();

		private List<GameObject> _itemTemplates = new List<GameObject>();

		private List<string> _titles = new List<string>();

		public void Add(IDataSource ds, IItemFactory itemFactory, GameObject listItemTemplate, string strTitleId)
		{
			_itemFactories.Add(itemFactory);
			_dataSources.Add(ds);
			_titles.Add(strTitleId);
			_itemTemplates.Add(listItemTemplate);
		}

		public void Build(GameObject container, GameObject expanderTemplate)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			UIWidgetContainer val = null;
			val = container.GetComponent<UIGrid>();
			if (val == null)
			{
				UITable val2 = container.GetComponent<UITable>();
				if (val2 == null)
				{
					val2 = container.AddComponent<UITable>();
					val2.columns = 1;
					val2.direction = 0;
					val2.pivot = 1;
				}
				val = val2;
			}
			Transform transform = container.get_transform();
			for (int i = 0; i < _titles.Count; i++)
			{
				GameObject val3 = GenericWidgetFactory.InstantiateGui(null, transform, "GenericList");
				GenericListView genericListView = val3.AddComponent<GenericListView>();
				genericListView.layout = val;
				genericListView.itemTemplate = _itemTemplates[i];
				GenericWidgetFactory.BuildListExisting(val3, _dataSources[i], _itemFactories[i]);
				GameObject val4 = GenericWidgetFactory.BuildExpanderTemplate(expanderTemplate, transform, val3, _titles[i]);
				val4.set_name("Expander");
				GenericExpanderView component = val4.GetComponent<GenericExpanderView>();
				component.layout = val;
				val4.get_transform().SetSiblingIndex(val3.get_transform().GetSiblingIndex());
				val4.SetActive(true);
				val3.SetActive(true);
			}
			_itemFactories.Clear();
			_dataSources.Clear();
			_itemTemplates.Clear();
			_titles.Clear();
		}
	}
}
