using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;

namespace LobbyServiceLayer
{
	internal class GetMultilayerAvatarInfoRequest : IGetMultilayerAvatarInfoRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<string, AvatarInfo>>
	{
		private IServiceAnswer<ReadOnlyDictionary<string, AvatarInfo>> _answer;

		public void Execute()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (!CacheDTO.AvatarInfo.get_isInitialized())
			{
				throw new Exception("No avatar info received from lobby - was this request fired at the wrong time?");
			}
			_answer.succeed(CacheDTO.AvatarInfo);
		}

		public IServiceRequest SetAnswer(IServiceAnswer<ReadOnlyDictionary<string, AvatarInfo>> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
