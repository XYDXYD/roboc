using System;

[Flags]
public enum PartyPopupMenuItems
{
	CancelPendingInvitation = 0x1,
	RemoveFromParty = 0x2,
	LeaveParty = 0x4,
	ChangeAvatar = 0x8
}
