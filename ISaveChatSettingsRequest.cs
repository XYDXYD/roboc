using Svelto.ServiceLayer;

internal interface ISaveChatSettingsRequest : IServiceRequest<ChatSettingsData>, IAnswerOnComplete, IServiceRequest
{
}
