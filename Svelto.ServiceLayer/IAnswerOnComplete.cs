namespace Svelto.ServiceLayer
{
	public interface IAnswerOnComplete
	{
		IServiceRequest SetAnswer(IServiceAnswer answer);
	}
	public interface IAnswerOnComplete<TSuccessParam>
	{
		IServiceRequest SetAnswer(IServiceAnswer<TSuccessParam> answer);
	}
}
