using Svelto.DataStructures;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal class SetMultilayerAvatarInfoRequest : ISetMultilayerAvatarInfoRequest, IServiceRequest<ReadOnlyDictionary<string, AvatarInfo>>, IServiceRequest
	{
		private ReadOnlyDictionary<string, AvatarInfo> _dependency;

		public void Inject(ReadOnlyDictionary<string, AvatarInfo> dependency)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_dependency = dependency;
		}

		public void Execute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			CacheDTO.AvatarInfo = _dependency;
		}
	}
}
