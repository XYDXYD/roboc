namespace Mothership.GUI.Party
{
	internal interface IPartyInvitationDialogController
	{
		void CheckForHotkeyInput();

		void RegisterView(PartyInvitationDialogView guiView);

		void ReceiveMessage(object message);
	}
}
