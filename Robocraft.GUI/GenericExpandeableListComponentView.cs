using UnityEngine;

namespace Robocraft.GUI
{
	public class GenericExpandeableListComponentView : GenericListComponentView
	{
		private string _poolName;

		private GameObject _listItemHeaderTemplate;

		public override void Setup()
		{
			base.Setup();
		}

		public void AddNewHeaderItemAtBottom()
		{
			GameObject newGO = base.guicomponentFactory.BuildListHeaderEntryView(_poolName, _listItemHeaderTemplate);
			IGenericListEntryView genericListEntryView = _gridAdapter.AddToBottomList(newGO);
			(genericListEntryView as IGenericListExpandeableEntryView).IsHeaderEntry = true;
		}

		public override void SetListItemTemplate(string poolName, GameObject template)
		{
			base.SetListItemTemplate(poolName, template);
		}

		public void SetListItemHeader(GameObject template, string headerPool)
		{
			_listItemHeaderTemplate = template;
			GenericComponentViewBase component = _listItemHeaderTemplate.GetComponent<GenericComponentViewBase>();
			((IGenericComponentView)component).SetController((IGenericComponent)null);
			_poolName = headerPool;
		}
	}
}
