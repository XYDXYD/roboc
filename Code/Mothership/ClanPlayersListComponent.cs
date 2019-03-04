using Robocraft.GUI;
using SocialServiceLayer;

namespace Mothership
{
	internal class ClanPlayersListComponent : GenericExpandeableListComponent<ClanMemberWithContextInfo>
	{
		public override int MinDataItemsPerPage => 0;

		public override int MaxDataItemsPerPage => 100;

		protected override void SynchroniseView()
		{
			base.SynchroniseView();
			int num = 0;
			for (int i = 0; i < base.DataSource.NumberOfDataItemsAvailable(0); i++)
			{
				ClanMember member = base.DataSource.QueryData<ClanMemberWithContextInfo>(i, 0).Member;
				if (member.ClanMemberState == ClanMemberState.Accepted)
				{
					num++;
				}
			}
			if (num < 10)
			{
				(_view as ClanPlayersListComponentView).ShowLessThanTenMembers(showLessThanTenMembersWarning: true);
			}
			else
			{
				(_view as ClanPlayersListComponentView).ShowLessThanTenMembers(showLessThanTenMembersWarning: false);
			}
		}

		public override void HandleMessage(GenericComponentMessage message)
		{
			MessageType message2 = message.Message;
			if (message2 == MessageType.ButtonWithinListClicked && message.Originator != "HeaderListButton")
			{
				PlayerListItemComponentDataContainer.PlayerListItemInfo playerListItemInfo = message.Data.UnpackData<PlayerListItemComponentDataContainer.PlayerListItemInfo>();
				if (_view.IsThisGameObjectOneOfMine(playerListItemInfo.listItemRef))
				{
					int indexOfGameObjectInList = _view.GetIndexOfGameObjectInList(playerListItemInfo.listItemRef);
					message.Consume();
					(View as GenericComponentViewBase).BubbleMessageUp(new GenericComponentMessage(MessageType.ButtonWithinListClicked, string.Empty, base.Name, new PlayerListItemComponentDataContainer(playerListItemInfo.listItemRef, indexOfGameObjectInList, playerListItemInfo.nameOfPlayer, playerListItemInfo.displayNameOfPlayer, message.Originator)));
					return;
				}
			}
			base.HandleMessage(message);
		}
	}
}
