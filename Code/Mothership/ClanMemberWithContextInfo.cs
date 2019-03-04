using SocialServiceLayer;
using UnityEngine;

namespace Mothership
{
	internal class ClanMemberWithContextInfo : IClanMemberListSortingData
	{
		public ClanMember Member;

		public ClanMemberRank RequesterRank;

		public bool RequestingYourClan;

		public bool OnlineStatus => Member.OnlineStatus;

		public bool IsInvited => Member.IsInvited;

		public bool IsLeader => Member.IsLeader;

		public bool IsOfficer => Member.IsOfficer;

		public int SeasonXP => Member.SeasonXP;

		public bool CanBeInvitedToParty
		{
			get;
			set;
		}

		public Texture2D PlayerAvatarTexture
		{
			get;
			set;
		}

		public ClanMemberWithContextInfo(ClanMember member_, ClanMemberRank RequesterRank_, bool RequestingYourClan_)
		{
			Member = member_;
			RequesterRank = RequesterRank_;
			RequestingYourClan = RequestingYourClan_;
		}
	}
}
