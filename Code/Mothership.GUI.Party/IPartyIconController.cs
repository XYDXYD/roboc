namespace Mothership.GUI.Party
{
	internal interface IPartyIconController
	{
		string PlayerAssignedToSlot
		{
			get;
		}

		bool IsBlockedFromInteraction
		{
			get;
		}

		void ReceiveMessage(object message);

		void RegisterView(ISharedPartyIcon guiView);

		bool IsLeader();
	}
}
