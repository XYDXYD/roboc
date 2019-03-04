using Svelto.ServiceLayer;
using System;

namespace LobbyServiceLayer.Photon
{
	internal class SetParametersForCampaignRequest : ISetParametersForCampaignRequest, IServiceRequest<SetParametersForCampaignDependency>, IServiceRequest
	{
		private SetParametersForCampaignDependency _dependency;

		public void Execute()
		{
			CacheDTO.MapName = _dependency.MapName;
			CacheDTO.ReconnectGameGUID = default(Guid).ToString();
			CacheDTO.GameMode = new GameModeKey(GameModeType.Campaign);
		}

		public void Inject(SetParametersForCampaignDependency dependency)
		{
			_dependency = dependency;
		}
	}
}
