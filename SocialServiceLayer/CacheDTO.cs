using Avatars;
using System.Collections.Generic;

namespace SocialServiceLayer
{
	internal static class CacheDTO
	{
		internal static List<Friend> friendList = null;

		internal static Dictionary<string, object> socialSettings = null;

		internal static Platoon platoon = new Platoon();

		internal static PlatoonInvite platoonInvite = null;

		internal static ClanInfo MyClanInfo = null;

		internal static Dictionary<string, ClanMember> MyClanMembers;

		internal static ClanInfo OtherClanInfo = null;

		internal static ClanMember[] OtherClanMembers;

		internal static float? XPConversionFactor = null;

		internal static Dictionary<string, ClanInvite> ClanInvites;

		internal static MultiAvatarCache AvatarCache = new MultiAvatarCache();
	}
}
