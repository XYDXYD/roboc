using Mothership;
using Robocraft.GUI;

internal class ClanSearchListComponent : GenericListComponent<ClanPlusAvatarInfo>
{
	public override int MinDataItemsPerPage => 0;

	public override int MaxDataItemsPerPage => 1000;

	protected override void SynchroniseView()
	{
		(_view as ClanSearchListComponentView).DeParentLoadMoreButton();
		base.SynchroniseView();
		(_view as ClanSearchListComponentView).MoveButtonToBottomOfList();
	}

	public override void HandleMessage(GenericComponentMessage message)
	{
		switch (message.Message)
		{
		case MessageType.ButtonWithinListClicked:
		{
			ClanListItemComponentDataContainer.ClanListItemInfo clanListItemInfo = message.Data.UnpackData<ClanListItemComponentDataContainer.ClanListItemInfo>();
			if (_view.IsThisGameObjectOneOfMine(clanListItemInfo.listItemRef))
			{
				int indexOfGameObjectInList = _view.GetIndexOfGameObjectInList(clanListItemInfo.listItemRef);
				message.Consume();
				(View as GenericComponentViewBase).BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, base.Name, new ClanListItemComponentDataContainer(clanListItemInfo.listItemRef, indexOfGameObjectInList, clanListItemInfo.nameOfClan)));
				return;
			}
			break;
		}
		case MessageType.SetScroll:
			if (message.Data != null)
			{
				float num = message.Data.UnpackData<float>();
				if (num == 1f && (_view as ClanSearchListComponentView).DoesContentFitInScrollView())
				{
					return;
				}
				_view.ScrollToPosition(num);
			}
			else
			{
				_view.RepositionToTop();
			}
			break;
		}
		base.HandleMessage(message);
	}
}
