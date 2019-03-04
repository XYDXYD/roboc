using Mothership.GUI.CustomGames;

namespace Mothership
{
	internal class ShowPartyInviteDropDownMessageForCustomGame
	{
		public UIWidget anchorWidget;

		public UIWidget areaWidget;

		public CustomGameTeamChoice teamChoice;

		public bool rebroadcastFromRoot;

		public ShowPartyInviteDropDownMessageForCustomGame(UIWidget anchor, UIWidget area, CustomGameTeamChoice teamChoice_)
		{
			anchorWidget = anchor;
			areaWidget = area;
			teamChoice = teamChoice_;
			rebroadcastFromRoot = false;
		}
	}
}
