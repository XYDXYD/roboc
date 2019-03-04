using Svelto.ServiceLayer;
using UnityEngine;

namespace Services.Web
{
	internal interface ILoadImageTextureRequest : IServiceRequest<LoadImageDependency>, IAnswerOnComplete<Texture2D>, IServiceRequest
	{
	}
}
