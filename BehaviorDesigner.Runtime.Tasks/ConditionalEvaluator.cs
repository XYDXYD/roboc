namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Evaluates the specified conditional task. If the conditional task returns success then the child task is run and the child status is returned. If the conditional task does not return success then the child task is not run and a failure status is immediately returned.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=146")]
	[TaskIcon("{SkinColor}ConditionalEvaluatorIcon.png")]
	public class ConditionalEvaluator : Decorator
	{
		[Tooltip("Should the conditional task be reevaluated every tick?")]
		public SharedBool reevaluate;

		[InspectTask]
		[Tooltip("The conditional task to evaluate")]
		public Conditional conditionalTask;

		private TaskStatus executionStatus;

		private bool checkConditionalTask = true;

		private bool conditionalTaskFailed;

		public ConditionalEvaluator()
			: this()
		{
		}

		public override void OnAwake()
		{
			if (conditionalTask != null)
			{
				conditionalTask.set_Owner(this.get_Owner());
				conditionalTask.set_GameObject(base.gameObject);
				conditionalTask.set_Transform(base.transform);
				conditionalTask.OnAwake();
			}
		}

		public override void OnStart()
		{
			if (conditionalTask != null)
			{
				conditionalTask.OnStart();
			}
		}

		public override bool CanExecute()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Invalid comparison between Unknown and I4
			if (checkConditionalTask)
			{
				checkConditionalTask = false;
				this.OnUpdate();
			}
			if (conditionalTaskFailed)
			{
				return false;
			}
			return (int)executionStatus == 0 || (int)executionStatus == 3;
		}

		public override bool CanReevaluate()
		{
			return reevaluate.get_Value();
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Invalid comparison between Unknown and I4
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			TaskStatus val = conditionalTask.OnUpdate();
			conditionalTaskFailed = (conditionalTask == null || (int)val == 1);
			return val;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = childStatus;
		}

		public override TaskStatus OverrideStatus()
		{
			return 1;
		}

		public override TaskStatus OverrideStatus(TaskStatus status)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (conditionalTaskFailed)
			{
				return 1;
			}
			return status;
		}

		public override void OnEnd()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = 0;
			checkConditionalTask = true;
			conditionalTaskFailed = false;
			if (conditionalTask != null)
			{
				conditionalTask.OnEnd();
			}
		}

		public override void OnReset()
		{
			conditionalTask = null;
		}
	}
}
