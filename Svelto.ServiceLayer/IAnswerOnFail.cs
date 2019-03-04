namespace Svelto.ServiceLayer
{
	public interface IAnswerOnFail
	{
		IServiceRequest SetAnswer(IServiceFailedAnswer answer);
	}
}
