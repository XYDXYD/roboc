using Svelto.ServiceLayer;

internal interface ILoadChatSettingsRequest : IServiceRequest, IAnswerOnComplete<ChatSettingsData>
{
}
