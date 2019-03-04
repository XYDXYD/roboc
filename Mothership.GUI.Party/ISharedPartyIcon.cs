using UnityEngine;

namespace Mothership.GUI.Party
{
	public interface ISharedPartyIcon
	{
		int GetPositionIndex();

		bool CanDrag();

		void ChangeIconStatus(PartyIconState newState);

		void AssignAvatarTexture(Texture avatarTexture);

		Texture FetchCurrentAvatarTexture();

		bool SlotIsAvailable();

		bool IsBlockedFromInteraction();

		void Disable();

		void Enable();

		void SetPlayerName(string name);

		void SetPlayerRobotTier(string tier);

		void HideTierStatus();

		void UpdateBackgroundColour(PartyIconInfo info);
	}
}
