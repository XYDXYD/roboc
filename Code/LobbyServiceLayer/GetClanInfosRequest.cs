using SocialServiceLayer;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using System;

namespace LobbyServiceLayer
{
	internal class GetClanInfosRequest : IGetClanInfosRequest, IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<string, ClanInfo>>
	{
		private IServiceAnswer<ReadOnlyDictionary<string, ClanInfo>> _answer;

		public void Execute()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (!CacheDTO.ClanInfos.get_isInitialized())
			{
				throw new Exception("No clan avatar info received from lobby - was this request fired at the wrong time?");
			}
			_answer.succeed(CacheDTO.ClanInfos);
		}

		public IServiceRequest SetAnswer(IServiceAnswer<ReadOnlyDictionary<string, ClanInfo>> answer)
		{
			_answer = answer;
			return this;
		}
	}
}
