using Svelto.ServiceLayer;
using UnityEngine;

internal interface ILoadThumbnailTextureFromDiskCache : IServiceRequest<LoadThumbnailFromDiskCacheDependency>, IAnswerOnComplete<Texture2D>, IServiceRequest
{
}
