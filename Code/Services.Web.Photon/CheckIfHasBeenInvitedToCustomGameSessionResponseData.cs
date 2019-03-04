using ExitGames.Client.Photon;
using System;

namespace Services.Web.Photon
{
	internal class CheckIfHasBeenInvitedToCustomGameSessionResponseData
	{
		public AvatarInfo AvatarInfo;

		public string SessionGUID;

		public string SenderName;

		public string SenderDisplayName;

		public bool IsInvitationForTeamB;

		public void Deserialise(Hashtable inputData)
		{
			int avatarId = (int)inputData.get_Item((object)"AvatarId");
			bool useCustomAvatar = Convert.ToBoolean(inputData.get_Item((object)"UseCustomAvatar"));
			SessionGUID = (string)inputData.get_Item((object)"SessionGUID");
			SenderName = (string)inputData.get_Item((object)"SenderName");
			SenderDisplayName = (string)inputData.get_Item((object)"SenderDisplayName");
			IsInvitationForTeamB = (bool)inputData.get_Item((object)"IsInvitedToTeamB");
			AvatarInfo = new AvatarInfo(useCustomAvatar, avatarId);
		}
	}
}
