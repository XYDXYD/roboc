using Svelto.ServiceLayer;

internal interface IValidateCurrentMachineRequest : IServiceRequest, IAnswerOnComplete<ValidateCurrentMachineResult>
{
	void Inject(LobbyType gameMode);
}
