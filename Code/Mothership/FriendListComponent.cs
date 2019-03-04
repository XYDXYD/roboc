using Robocraft.GUI;

namespace Mothership
{
	internal class FriendListComponent : GenericExpandeableListComponent<FriendListEntryData>
	{
		public override int MinDataItemsPerPage => 0;

		public override int MaxDataItemsPerPage => 210;

		public override void HandleMessage(GenericComponentMessage message)
		{
			MessageType message2 = message.Message;
			if (message2 == MessageType.ButtonWithinListClicked)
			{
				FriendListItemComponentDataContainer.ListItemInfo listItemInfo = message.Data.UnpackData<FriendListItemComponentDataContainer.ListItemInfo>();
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
						(View as GenericComponentViewBase).BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, base.Name, new FriendListItemComponentDataContainer(listItemInfo.listItemRef, indexOfGameObjectInList, listItemInfo.friend, listItemInfo.buttonClicked)));
					}
				}
			}
			else
			{
				base.HandleMessage(message);
			}
		}
	}
}
