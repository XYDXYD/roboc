using Svelto.ServiceLayer;
using Svelto.Tasks;

public class TaskService<T> : TaskService
{
	public T result;

	public TaskService(IAnswerOnComplete<T> service)
		: base(service as IServiceRequest)
	{
		service.SetAnswer(new ServiceAnswer<T>(delegate(T success)
		{
			result = success;
			base.isDone = true;
		}, delegate(ServiceBehaviour behaviour)
		{
			base.behaviour = behaviour;
			base.isDone = true;
		}));
		_service = (IServiceRequest)service;
	}

	public override void Execute()
	{
		result = default(T);
		base.Execute();
	}
}
public class TaskService : ITask, IAbstractTask
{
	public ServiceBehaviour behaviour;

	protected IServiceRequest _service;

	public bool isDone
	{
		get;
		protected set;
	}

	public bool succeeded => behaviour == null;

	public TaskService(IAnswerOnComplete service)
		: this(service as IServiceRequest)
	{
		service.SetAnswer(new ServiceAnswer(delegate
		{
			isDone = true;
		}, delegate(ServiceBehaviour behaviour)
		{
			this.behaviour = behaviour;
			isDone = true;
		}));
	}

	protected TaskService(IServiceRequest service)
	{
		_service = service;
	}

	public virtual void Execute()
	{
		behaviour = null;
		isDone = false;
		_service.Execute();
	}
}
