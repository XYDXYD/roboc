using Svelto.ServiceLayer;
using Svelto.Tasks;
using UnityEngine;

namespace SocialServiceLayer
{
	internal interface ILoadClanAvatarRequest : IServiceRequest<string>, IAnswerOnComplete<Texture2D>, ITask, IServiceRequest, IAbstractTask
	{
		bool ForceRefresh
		{
			get;
			set;
		}
	}
}
