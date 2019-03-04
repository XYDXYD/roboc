using Authentication;
using System;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal static class SocialStaticUtilities
	{
		public const string PARTY_INVITE_CHAT_COMMAND = "groupinvite";

		public const string LOCALIZATION_STRING_PLAYER_VAR = "{PLAYER}";

		public const int MaxPlatoonSize = 5;

		public static bool ValidateInviteeName(ref string inviteeName, out string errMsg)
		{
			errMsg = string.Empty;
			if (!SocialInputValidation.ValidateUserName(ref inviteeName))
			{
				errMsg = StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_INVALID_USERNAME.ToString());
				return false;
			}
			if (inviteeName.Equals(User.Username, StringComparison.InvariantCultureIgnoreCase))
			{
				errMsg = StringTableBase<StringTable>.Instance.GetString(SocialErrorCode.STR_SOCIAL_REASON_USER_IS_SELF.ToString());
				return false;
			}
			return true;
		}

		public static string GetInviteToPartyFailedMessage(string inviteeName, SocialErrorCode reason)
		{
			string empty = string.Empty;
			switch (reason)
			{
			case SocialErrorCode.STR_SOCIAL_REASON_INVITEE_ALREADY_IN_PLATOON:
				return StringTableBase<StringTable>.Instance.GetString("strPlayerAlreadyInAParty").Replace("{PLAYER}", inviteeName);
			case SocialErrorCode.STR_SOCIAL_REASON_USER_NOT_ONLINE:
				return StringTableBase<StringTable>.Instance.GetString("strPlayerNotOnline").Replace("{PLAYER}", inviteeName);
			case SocialErrorCode.STR_SOCIAL_REASON_USER_DOES_NOT_EXIST:
				return StringTableBase<StringTable>.Instance.GetString("strPlayerDoesNotExist").Replace("{PLAYER}", inviteeName);
			case SocialErrorCode.STR_SOCIAL_REASON_USER_BLOCKED_YOU:
				return StringTableBase<StringTable>.Instance.GetString(reason.ToString()).Replace("{PLAYER}", inviteeName);
			default:
				return string.Format("{0} {1} : {2}", StringTableBase<StringTable>.Instance.GetString("strSendPlatInviteErrorTo"), inviteeName, StringTableBase<StringTable>.Instance.GetString(reason.ToString()));
			}
		}

		public static bool IsParentOf(Transform root, Transform t)
		{
			while (t.get_parent() != null)
			{
				if (t.get_parent() == root)
				{
					return true;
				}
				t = t.get_parent();
			}
			return false;
		}

		private static bool IsMemberInParty(Platoon partyData, string playerName)
		{
			if (partyData.Size > 0)
			{
				for (int i = 0; i < partyData.members.Length; i++)
				{
					if (playerName.Equals(partyData.members[i].Name, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
