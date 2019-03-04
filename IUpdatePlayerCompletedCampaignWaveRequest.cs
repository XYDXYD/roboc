using Svelto.ServiceLayer;

public interface IUpdatePlayerCompletedCampaignWaveRequest : IServiceRequest<UpdatePlayerCompletedCampaignWaveDependency>, IAnswerOnComplete, IServiceRequest
{
}
