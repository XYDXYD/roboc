using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace SocialServiceLayer.Photon
{
	internal class SocialRequestFactory : ServiceRequestFactory, ISocialRequestFactory, IServiceRequestFactory
	{
		public SocialRequestFactory()
		{
			AddRelation<IGetFriendListRequest, GetFriendListRequest>();
			AddRelation<IInviteFriendRequest, InviteFriendRequest, string>();
			AddRelation<IAcceptFriendRequest, AcceptFriendRequest, string>();
			AddRelation<IDeclineFriendRequest, DeclineFriendRequest, string>();
			AddRelation<ICancelFriendRequest, CancelFriendRequest, string>();
			AddRelation<IRemoveFriendRequest, RemoveFriendRequest, string>();
			AddRelation<IAcceptPlatoonInviteRequest, AcceptPlatoonInviteRequest>();
			AddRelation<IDeclinePlatoonInviteRequest, DeclinePlatoonInviteRequest>();
			AddRelation<IGetPlatoonDataRequest, GetPlatoonDataRequest>();
			AddRelation<IGetPlayerCanBeInvitedToCustomGameRequest, GetPlayerCanBeInvitedToCustomGameRequest, string>();
			AddRelation<IGetPlayerCanBeInvitedToRegularPartyRequest, GetPlayerCanBeInvitedToRegularPartyRequest, string>();
			AddRelation<IInviteToPlatoonRequest, InviteToPlatoonRequest, string>();
			AddRelation<IKickFromPlatoonRequest, KickFromPlatoonRequest, string>();
			AddRelation<ILeavePlatoonRequest, LeavePlatoonRequest>();
			AddRelation<ISetPlatoonMemberStatusRequest, SetPlatoonMemberStatusRequest, SetPlatoonMemberStatusDependency>();
			AddRelation<IGetPlatoonPendingInviteRequest, GetPlatoonPendingInviteRequest>();
			AddRelation<IClearPlatoonCacheRequest, ClearPlatoonCacheCacheRequest>();
			AddRelation<ICheckPartyRobotTiersRequest, CheckPartyRobotTiersRequest>();
			AddRelation<IGetSocialSettingsRequest, GetSocialSettingsRequest>();
			AddRelation<ISetSocialSettingsRequest, SetSocialSettingsRequest, Dictionary<string, object>>();
			AddRelation<ICreateClanRequest<CreateClanRequestDependancyTencent>, CreateClanRequest<CreateClanRequestDependancyTencent>, CreateClanRequestDependancyTencent>();
			AddRelation<ISearchClansRequest, SearchClansRequest, SearchClanDependency>();
			AddRelation<IGetClanInfoAndMembersRequest, GetClanInfoAndMembersRequest, string>();
			AddRelation<IGetMyClanInfoAndMembersRequest, GetMyClanInfoAndMembersRequest>();
			AddRelation<IJoinClanRequest, JoinClanRequest, string>();
			AddRelation<IInviteToClanRequest, InviteToClanRequest, string>();
			AddRelation<IAcceptClanInviteRequest, AcceptClanInviteRequest, string>();
			AddRelation<IDeclineClanInviteRequest, DeclineClanInviteRequest, string>();
			AddRelation<IChangeClanMemberRankRequest, ChangeClanMemberRankRequest, ChangeClanMemberRankDependency>();
			AddRelation<IGetClanInvitesRequest, GetClanInvitesRequest>();
			AddRelation<ILeaveClanRequest, LeaveClanRequest>();
			AddRelation<IRemoveFromClanRequest, RemoveFromClanRequest, string>();
			AddRelation<IGetMyClanInfoRequest, GetMyClanInfoRequest>();
			AddRelation<IDeclineAllClanInvitesRequest, DeclineAllClanInvitesRequest>();
			AddRelation<ICancelClanInviteRequest, CancelClanInviteRequest, string>();
			AddRelation<IChangeClanDataRequest, ChangeClanDataRequest, ChangeClanDataDependency>();
			AddRelation<IRenameClanRequest, RenameClanRequest, ClanRenameDependency>();
			AddRelation<IFetchSeasonRewardsRequest, FetchSeasonRewardsRequest, FetchSeasonRewardsDependancy>();
			AddRelation<IReclaimSeasonRewardsRequest, ReclaimSeasonRewardRequest, string>();
			AddRelation<IPollClanExperienceRequest, PollClanExperienceRequest, string>();
			AddRelation<IGetNewPreviousBattleRewardsRequest, GetNewPreviousBattleRewardsRequest, string>();
			AddRelation<IHasNewPreviousBattleRewardsRequest, HasNewPreviousBattleRewardsRequest, string>();
			AddRelation<ICollectPreviousBattleRewardsRequest, CollectPreviousBattleRewardsRequest, string>();
			AddRelation<IValidateSeasonRewardsRequest, ValidateSeasonRewardsRequest>();
			AddRelation<IPlatoonRobotTierChangeRequest, PlatoonRobotTierChangeRequest, int>();
			AddRelation<IMultiAvatarLoadRequest, MultiAvatarLoadRequest, MultiAvatarRequestDependency>();
			AddRelation<IAvatarUpdatedRequest, AvatarUpdatedRequest>();
			AddRelation<IGetAvatarAtlasForBattleRequest, GetAvatarAtlasForBattleRequest, GetAvatarAtlasRequestDependancy>();
			AddRelation<IGetClanAvatarAtlasForBattleRequest, GetClanAvatarAtlasForBattleRequest, GetClanAvatarAtlasRequestDependancy>();
			AddRelation<IClearAvatarCacheRequest, ClearAvatarCacheRequest>();
		}
	}
}
