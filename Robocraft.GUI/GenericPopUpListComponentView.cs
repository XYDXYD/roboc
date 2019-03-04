using System;
using UnityEngine;

namespace Robocraft.GUI
{
	[RequireComponent(typeof(UIPopUpListAdapter))]
	internal class GenericPopUpListComponentView : GenericComponentViewBase
	{
		private UIPopUpListAdapter _popUpList;

		public override void Setup()
		{
			_popUpList = this.GetComponent<UIPopUpListAdapter>();
			_popUpList.Setup();
			UIPopUpListAdapter popUpList = _popUpList;
			popUpList.OnValueChanged = (Action<int>)Delegate.Combine(popUpList.OnValueChanged, new Action<int>(OnValueChanged));
			base.Setup();
		}

		public void SetCurrentSelection(string currentSelection)
		{
			_popUpList.SetCurrentSelection(currentSelection);
		}

		public override void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public override void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		private void OnValueChanged(int valueIndex)
		{
			(_controller as GenericPopUpListComponent).HandleValueChanged(valueIndex);
		}
	}
}
