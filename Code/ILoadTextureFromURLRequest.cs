using Svelto.ServiceLayer;
using UnityEngine;

internal interface ILoadTextureFromURLRequest : IServiceRequest<string>, IAnswerOnComplete<Texture2D>, IServiceRequest
{
}
