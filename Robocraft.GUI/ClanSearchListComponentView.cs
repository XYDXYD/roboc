using UnityEngine;

namespace Robocraft.GUI
{
	public class ClanSearchListComponentView : GenericListComponentView
	{
		private GameObject _loadMoreButton;

		public void SetLoadMoreItemsButton(GameObject button)
		{
			_loadMoreButton = button;
		}

		public void DeParentLoadMoreButton()
		{
			_loadMoreButton.get_transform().set_parent(_gridAdapter.get_transform().get_parent());
		}

		public void MoveButtonToBottomOfList()
		{
			_loadMoreButton.get_transform().set_parent(_gridAdapter.get_transform());
			_loadMoreButton.get_transform().SetAsLastSibling();
			_gridAdapter.Reposition();
		}

		public bool DoesContentFitInScrollView()
		{
			return !_gridAdapter.AutomaticallyFixThisScrollPanel.get_shouldMoveVertically();
		}

		public override void Clear()
		{
			Transform parent = _loadMoreButton.get_transform().get_parent();
			_loadMoreButton.get_transform().set_parent(_loadMoreButton.get_transform().get_parent().get_parent());
			base.Clear();
			_loadMoreButton.get_transform().set_parent(parent);
		}

		public override void Show()
		{
			_loadMoreButton.SetActive(true);
			base.Show();
		}

		public override void Hide()
		{
			_loadMoreButton.SetActive(false);
			base.Hide();
		}
	}
}
