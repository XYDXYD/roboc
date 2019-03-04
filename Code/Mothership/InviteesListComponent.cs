using Robocraft.GUI;
using SocialServiceLayer;

namespace Mothership
{
	internal class InviteesListComponent : GenericExpandeableListComponent<ClanInvite>
	{
		public override int MinDataItemsPerPage => 0;

		public override int MaxDataItemsPerPage => 100;

		public override void HandleMessage(GenericComponentMessage message)
		{
			MessageType message2 = message.Message;
			if (message2 == MessageType.ButtonWithinListClicked)
			{
				InviteeListItemComponentDataContainer.ListItemInfo listItemInfo = message.Data.UnpackData<InviteeListItemComponentDataContainer.ListItemInfo>();
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
						(View as GenericComponentViewBase).BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, base.Name, new InviteeListItemComponentDataContainer(listItemInfo.nameOfPlayer, listItemInfo.nameOfClan, listItemInfo.action, listItemInfo.listItemRef)));
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
