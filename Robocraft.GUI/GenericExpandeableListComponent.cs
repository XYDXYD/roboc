using System.Collections.Generic;

namespace Robocraft.GUI
{
	internal class GenericExpandeableListComponent<T> : GenericListComponent<T>
	{
		private IDataSource _headersDataSource;

		private Dictionary<int, int> _dataSourceIndexToListItemIndexMap = new Dictionary<int, int>();

		private HashSet<string> _collapsedHeaders = new HashSet<string>();

		protected IDataSource HeadersDataSource => _headersDataSource;

		public override void SetView(IGenericComponentView view)
		{
			_view = (view as GenericListComponentView);
		}

		public void SetHeadersDataSource(IDataSource headersDataSource)
		{
			_headersDataSource = headersDataSource;
		}

		protected override void OnAllDataChanged()
		{
			_dataSourceIndexToListItemIndexMap.Clear();
			SynchroniseView();
		}

		protected override void OnDataItemChanged(int index1, int index2)
		{
			T data = base.DataSource.QueryData<T>(index1, 0);
			if (_dataSourceIndexToListItemIndexMap.ContainsKey(index1))
			{
				int index3 = _dataSourceIndexToListItemIndexMap[index1];
				_view.SetListData(index3, data);
			}
		}

		public override void Activate()
		{
			_collapsedHeaders = new HashSet<string>();
			base.Activate();
		}

		protected override void SynchroniseView()
		{
			bool flag = true;
			while (flag)
			{
				flag = false;
				foreach (string collapsedHeader in _collapsedHeaders)
				{
					if (!ToggleMaximisationOfEntry(collapsedHeader, expandedSetting: false))
					{
						_collapsedHeaders.Remove(collapsedHeader);
						flag = true;
						break;
					}
				}
			}
			_view.ForceRefreshContents();
			_view.Clear();
			int num = CountExpectedItemsOnThisPage(_pageNumber, MaxDataItemsPerPage);
			for (int i = 0; i < num; i++)
			{
				if (IsHeaderSection(i, _pageNumber, MaxDataItemsPerPage))
				{
					(_view as GenericExpandeableListComponentView).AddNewHeaderItemAtBottom();
				}
				else
				{
					_view.AddItemAtBottom();
				}
			}
			RepopulateAllListItems();
			_view.RepositionToTop();
		}

		protected override void RepopulateAllListItems()
		{
			for (int i = 0; i < _view.CurrentItemCount; i++)
			{
				RepopulateListItem(i);
			}
		}

		private int CountExpectedItemsOnThisPage(int inputPage, int inputItemsPerPage)
		{
			int num = base.DataSource.NumberOfDataItemsAvailable(0);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			bool flag = true;
			bool flag2 = false;
			while (flag)
			{
				while (num2 < inputItemsPerPage && num4 < num)
				{
					if (num5 < _headersDataSource.NumberOfDataItemsAvailable(0) && GetDataForHeaderIndex(num5).positionInData == num4)
					{
						flag2 = !GetDataForHeaderIndex(num5).expandedStatus;
						num2++;
						num5++;
						continue;
					}
					if (!flag2)
					{
						num2++;
					}
					num4++;
				}
				if (num3 == inputPage)
				{
					return num2;
				}
				if (num4 >= num)
				{
					flag = false;
				}
				num2 = 0;
				num3++;
			}
			return 0;
		}

		private ExpandeableListHeaderData GetDataForHeaderIndex(int headerDataIndex)
		{
			return _headersDataSource.QueryData<ExpandeableListHeaderData>(headerDataIndex, 0);
		}

		private int FindTrueDataIndexForListItemIndex(int inputListItemIndex, int inputPage, int inputItemsPerPage, out bool wasHeaderSection)
		{
			wasHeaderSection = false;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = base.DataSource.NumberOfDataItemsAvailable(0);
			bool flag = true;
			bool flag2 = false;
			while (flag)
			{
				while (num < inputItemsPerPage && num3 < num5)
				{
					if (_headersDataSource.NumberOfDataItemsAvailable(0) > num4 && GetDataForHeaderIndex(num4).positionInData == num3)
					{
						if (inputListItemIndex == num && inputPage == num2)
						{
							wasHeaderSection = true;
							return -1;
						}
						num++;
						flag2 = !GetDataForHeaderIndex(num4).expandedStatus;
						num4++;
					}
					else
					{
						if (inputListItemIndex == num && inputPage == num2 && !flag2)
						{
							return num3;
						}
						if (!flag2)
						{
							num++;
						}
						num3++;
					}
				}
				if (num3 >= num5)
				{
					flag = false;
				}
				num2++;
				num = 0;
			}
			return -1;
		}

		internal bool IsHeaderSection(int listItemIndex, int page, int itemsPerPage)
		{
			FindTrueDataIndexForListItemIndex(listItemIndex, page, itemsPerPage, out bool wasHeaderSection);
			return wasHeaderSection;
		}

		private ExpandeableListHeaderData FindHeaderSectionForItemIndexAndPage(int listItemIndex, int pageNumber, int itemsPerPage)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = base.DataSource.NumberOfDataItemsAvailable(0);
			bool flag = false;
			while (true)
			{
				if (num < itemsPerPage && num3 < num5)
				{
					if (num4 < _headersDataSource.NumberOfDataItemsAvailable(0) && GetDataForHeaderIndex(num4).positionInData == num3)
					{
						if (listItemIndex == num && pageNumber == num2)
						{
							return GetDataForHeaderIndex(num4);
						}
						num++;
						flag = !GetDataForHeaderIndex(num4).expandedStatus;
						num4++;
					}
					else
					{
						if (!flag)
						{
							num++;
						}
						num3++;
					}
				}
				else
				{
					if (num4 == _headersDataSource.NumberOfDataItemsAvailable(0))
					{
						return null;
					}
					if (num3 >= num5)
					{
						break;
					}
					num2++;
					num = 0;
				}
			}
			return null;
		}

		internal bool ToggleMaximisationOfEntry(string listItemHeaderText, bool expandedSetting)
		{
			for (int i = 0; i < _headersDataSource.NumberOfDataItemsAvailable(0); i++)
			{
				ExpandeableListHeaderData dataForHeaderIndex = GetDataForHeaderIndex(i);
				if (dataForHeaderIndex.headerText.Equals(listItemHeaderText))
				{
					GetDataForHeaderIndex(i).expandedStatus = expandedSetting;
					return true;
				}
			}
			return false;
		}

		internal void ToggleMaximisationOfEntryForCurrentPage(int listItemIndex)
		{
			ExpandeableListHeaderData expandeableListHeaderData = FindHeaderSectionForItemIndexAndPage(listItemIndex, _pageNumber, MaxDataItemsPerPage);
			expandeableListHeaderData.expandedStatus = !expandeableListHeaderData.expandedStatus;
			if (!expandeableListHeaderData.expandedStatus)
			{
				_collapsedHeaders.Add(expandeableListHeaderData.headerText);
			}
			else
			{
				_collapsedHeaders.Remove(expandeableListHeaderData.headerText);
			}
			SynchroniseView();
		}

		public override void HandleMessage(GenericComponentMessage message)
		{
			MessageType message2 = message.Message;
			if (message2 == MessageType.ButtonWithinListClicked)
			{
				ListItemComponentDataContainer.ListItemInfo listItemInfo = message.Data.UnpackData<ListItemComponentDataContainer.ListItemInfo>();
				if (_view.IsThisGameObjectOneOfMine(listItemInfo.listItemRef))
				{
					message.Consume();
					int indexOfGameObjectInList = _view.GetIndexOfGameObjectInList(listItemInfo.listItemRef);
					if (IsHeaderSection(indexOfGameObjectInList, _pageNumber, MaxDataItemsPerPage))
					{
						ToggleMaximisationOfEntryForCurrentPage(indexOfGameObjectInList);
					}
					else
					{
						base.HandleMessage(message);
					}
				}
			}
			else
			{
				base.HandleMessage(message);
			}
		}

		protected override void RepopulateListItem(int listItemIndex)
		{
			if (IsHeaderSection(listItemIndex, _pageNumber, MaxDataItemsPerPage))
			{
				ExpandeableListHeaderData expandeableListHeaderData = FindHeaderSectionForItemIndexAndPage(listItemIndex, _pageNumber, MaxDataItemsPerPage);
				ListHeaderInfo data = new ListHeaderInfo(expandeableListHeaderData.headerText, expandeableListHeaderData.colorIndex, expandeableListHeaderData.expandedStatus);
				_view.SetListData(listItemIndex, data);
			}
			else
			{
				bool wasHeaderSection;
				int num = FindTrueDataIndexForListItemIndex(listItemIndex, _pageNumber, MaxDataItemsPerPage, out wasHeaderSection);
				T data2 = base.DataSource.QueryData<T>(num, 0);
				_dataSourceIndexToListItemIndexMap[num] = listItemIndex;
				_view.SetListData(listItemIndex, data2);
			}
		}
	}
}
