using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IGetClanInvitesRequest : IServiceRequest, IAnswerOnComplete<ClanInvite[]>
	{
		bool ForceRefresh
		{
			get;
			set;
		}
	}
}
