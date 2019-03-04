namespace Mothership.GUI.Party
{
	internal class PartyMemberButtonClickMessage
	{
		internal readonly MouseButton ButtonClicked;

		internal readonly UIWidget UIElement;

		internal readonly int SlotIndex;

		internal readonly PartyIconState IconStateWhenClicked;

		internal readonly string PlayerAssignedToSlot;

		internal PartyMemberButtonClickMessage(PartyIconState iconState, UIWidget uiElement, int slotindex_, MouseButton mouseButton, string player_)
		{
			ButtonClicked = mouseButton;
			UIElement = uiElement;
			SlotIndex = slotindex_;
			IconStateWhenClicked = iconState;
			PlayerAssignedToSlot = player_;
		}
	}
}
