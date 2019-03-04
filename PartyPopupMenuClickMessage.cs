internal class PartyPopupMenuClickMessage
{
	public readonly string partyMamberName;

	public readonly PartyPopupMenuItems itemClicked;

	public PartyPopupMenuClickMessage(string partyMember, PartyPopupMenuItems menuItem)
	{
		partyMamberName = partyMember;
		itemClicked = menuItem;
	}
}
