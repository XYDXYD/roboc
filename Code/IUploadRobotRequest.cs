using Services.Web.Photon;
using Svelto.ServiceLayer;

internal interface IUploadRobotRequest : IServiceRequest<UploadRobotDependency>, IAnswerOnComplete<bool>, IServiceRequest
{
}
