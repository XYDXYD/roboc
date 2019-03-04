internal class ShowPartyPopupMenuMessage
{
	public readonly UIWidget anchorWidget;

	public readonly string playerName;

	public readonly PartyPopupMenuItems availableOptions;

	public bool rebroadcastFromRoot;

	public ShowPartyPopupMenuMessage(UIWidget anchor, string partyMember, PartyPopupMenuItems menuItems)
	{
		anchorWidget = anchor;
		playerName = partyMember;
		availableOptions = menuItems;
	}
}
