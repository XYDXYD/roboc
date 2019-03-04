using UnityEngine;
using Utility;

namespace Robocraft.GUI
{
	internal class GenericListComponent<T> : GenericComponentBase
	{
		protected int _pageNumber;

		protected GenericListComponentView _view;

		public virtual int MinDataItemsPerPage => 1;

		public virtual int MaxDataItemsPerPage => 10;

		public override IGenericComponentView View => _view;

		public void NextPage()
		{
			_pageNumber++;
			try
			{
				RepopulateAllListItems();
			}
			catch
			{
				_pageNumber--;
				RepopulateAllListItems();
			}
		}

		public void PreviousPage()
		{
			if (_pageNumber > 0)
			{
				_pageNumber--;
				RepopulateAllListItems();
			}
		}

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericListComponentView);
		}

		public override void Activate()
		{
			base.Activate();
			SynchroniseView();
		}

		public override void HandleMessage(GenericComponentMessage message)
		{
			switch (message.Message)
			{
			case MessageType.ButtonWithinListClicked:
			{
				ListItemComponentDataContainer.ListItemInfo listItemInfo = message.Data.UnpackData<ListItemComponentDataContainer.ListItemInfo>();
				if (_view.IsThisGameObjectOneOfMine(listItemInfo.listItemRef))
				{
					message.Consume();
					int indexOfGameObjectInList = _view.GetIndexOfGameObjectInList(listItemInfo.listItemRef);
					(View as GenericComponentViewBase).BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, base.Name, new ListItemComponentDataContainer(listItemInfo.listItemRef, indexOfGameObjectInList)));
				}
				break;
			}
			case MessageType.NextListPage:
				NextPage();
				break;
			case MessageType.PreviousListPage:
				PreviousPage();
				break;
			case MessageType.SetScroll:
				if (message.Data != null)
				{
					float positionInterpolated = message.Data.UnpackData<float>();
					_view.ScrollToPosition(positionInterpolated);
				}
				else
				{
					_view.RepositionToTop();
				}
				break;
			case MessageType.RefreshData:
				if (message.Data != null)
				{
					int num = message.Data.UnpackData<int>();
					Console.Log("refresh unique item index " + num);
					if (num >= _view.CurrentItemCount)
					{
						SynchroniseView();
					}
					RepopulateListItem(num);
				}
				else
				{
					SynchroniseView();
					RepopulateAllListItems();
				}
				break;
			case MessageType.UpdateView:
				RepopulateAllListItems();
				break;
			}
			base.HandleMessage(message);
		}

		protected virtual void SynchroniseView()
		{
			int i;
			for (i = _view.CurrentItemCount; i < MinDataItemsPerPage; i++)
			{
				_view.AddItemAtBottom();
				RepopulateListItem(i);
			}
			int num = i + _pageNumber * MaxDataItemsPerPage;
			while (i < MaxDataItemsPerPage && num < base.DataSource.NumberOfDataItemsAvailable(0))
			{
				_view.AddItemAtBottom();
				RepopulateListItem(i);
				i++;
				num++;
			}
			int num2 = Mathf.Clamp(base.DataSource.NumberOfDataItemsAvailable(0) - _pageNumber * MaxDataItemsPerPage, 0, MaxDataItemsPerPage);
			int currentItemCount = _view.CurrentItemCount;
			if (currentItemCount > num2)
			{
				int num3 = currentItemCount - num2;
				for (int j = 0; j < num3; j++)
				{
					_view.RemoveItemFromBottom();
				}
				RepopulateAllListItems();
			}
			_view.RepositionToTop();
		}

		protected virtual void RepopulateAllListItems()
		{
			for (int i = 0; i < _view.CurrentItemCount; i++)
			{
				RepopulateListItem(i);
			}
		}

		protected virtual void RepopulateListItem(int listItemIndex)
		{
			int uniqueIdentifier = listItemIndex + _pageNumber * MaxDataItemsPerPage;
			T data = base.DataSource.QueryData<T>(uniqueIdentifier, 0);
			_view.SetListData(listItemIndex, data);
		}
	}
}
