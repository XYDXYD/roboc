using SocialServiceLayer;
using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal class SetClanInfosRequest : ISetClanInfosRequest, IServiceRequest<ReadOnlyDictionary<string, ClanInfo>>, IServiceRequest
	{
		private ReadOnlyDictionary<string, ClanInfo> _dependency;

		public void Inject(ReadOnlyDictionary<string, ClanInfo> dependency)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_dependency = dependency;
		}

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			CacheDTO.ClanInfos = _dependency;
		}
	}
}
