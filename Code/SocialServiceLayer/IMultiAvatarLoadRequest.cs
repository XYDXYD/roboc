using Svelto.ServiceLayer;
using Svelto.Tasks;
using UnityEngine;

namespace SocialServiceLayer
{
	internal interface IMultiAvatarLoadRequest : IServiceRequest<MultiAvatarRequestDependency>, IAnswerOnComplete<Texture2D>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
