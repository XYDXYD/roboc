using Services.Web.Photon;
using Svelto.ServiceLayer;

internal interface IGetCurrentRobotControlsRequest : IServiceRequest, IAnswerOnComplete<GetRobotControlsResult>
{
}
