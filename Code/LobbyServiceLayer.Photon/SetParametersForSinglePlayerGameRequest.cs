using Svelto.ServiceLayer;
using System;

namespace LobbyServiceLayer.Photon
{
	internal class SetParametersForSinglePlayerGameRequest : ISetParametersForSinglePlayerGameRequest, IServiceRequest<SetParametersForSinglePlayerGameDependency>, IServiceRequest
	{
		private SetParametersForSinglePlayerGameDependency _dependency;

		public void Execute()
		{
			CacheDTO.MapName = _dependency.MapName;
			CacheDTO.ReconnectGameGUID = default(Guid).ToString();
			CacheDTO.GameMode = new GameModeKey(GameModeType.PraticeMode);
		}

		public void Inject(SetParametersForSinglePlayerGameDependency dependency)
		{
			_dependency = dependency;
		}
	}
}
