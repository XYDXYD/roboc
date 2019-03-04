using System;

namespace Svelto.ServiceLayer
{
	public class ServiceAnswer<T> : IServiceAnswer<T>, IServiceFailedAnswer
	{
		public Action<T> succeed
		{
			get;
			private set;
		}

		public Action<ServiceBehaviour> failed
		{
			get;
			private set;
		}

		public ServiceAnswer(Action<T> success, Action<ServiceBehaviour> fail)
		{
			succeed = success;
			failed = fail;
		}

		public ServiceAnswer(Action<T> success)
			: this(success, (Action<ServiceBehaviour>)null)
		{
		}
	}
	public class ServiceAnswer : IServiceAnswer, IServiceFailedAnswer
	{
		public Action succeed
		{
			get;
			private set;
		}

		public Action<ServiceBehaviour> failed
		{
			get;
			private set;
		}

		public ServiceAnswer(Action success, Action<ServiceBehaviour> fail)
		{
			succeed = success;
			failed = fail;
		}

		public ServiceAnswer(Action success)
			: this(success, null)
		{
		}

		public ServiceAnswer(Action<ServiceBehaviour> fail)
			: this(null, fail)
		{
		}
	}
}
