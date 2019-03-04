namespace Svelto.ServiceLayer
{
	public static class IAnswerOnCompleteExt
	{
		public static TaskService AsTask(this IAnswerOnComplete request)
		{
			return new TaskService(request);
		}

		public static TaskService<TReturnData> AsTask<TReturnData>(this IAnswerOnComplete<TReturnData> request)
		{
			return new TaskService<TReturnData>(request);
		}
	}
}
