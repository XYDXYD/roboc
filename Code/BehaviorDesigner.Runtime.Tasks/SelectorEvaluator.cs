namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The selector evaluator is a selector task which reevaluates its children every tick. It will run the lowest priority child which returns a task status of running. This is done each tick. If a higher priority child is running and the next frame a lower priority child wants to run it will interrupt the higher priority child. The selector evaluator will return success as soon as the first child returns success otherwise it will keep trying higher priority children. This task mimics the conditional abort functionality except the child tasks don't always have to be conditional tasks.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=109")]
	[TaskIcon("{SkinColor}SelectorEvaluatorIcon.png")]
	public class SelectorEvaluator : Composite
	{
		private int currentChildIndex;

		private TaskStatus executionStatus;

		private int storedCurrentChildIndex = -1;

		private TaskStatus storedExecutionStatus;

		public SelectorEvaluator()
			: this()
		{
		}

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override void OnChildStarted(int childIndex)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			currentChildIndex++;
			executionStatus = 3;
		}

		public override bool CanExecute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			if ((int)executionStatus == 2 || (int)executionStatus == 3)
			{
				return false;
			}
			if (storedCurrentChildIndex != -1)
			{
				return currentChildIndex < storedCurrentChildIndex - 1;
			}
			return currentChildIndex < base.children.Count;
		}

		public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Invalid comparison between Unknown and I4
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((int)childStatus == 0 && base.children[childIndex].get_Disabled())
			{
				executionStatus = 1;
			}
			if ((int)childStatus != 0 && (int)childStatus != 3)
			{
				executionStatus = childStatus;
			}
		}

		public override void OnConditionalAbort(int childIndex)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			currentChildIndex = childIndex;
			executionStatus = 0;
		}

		public override void OnEnd()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = 0;
			currentChildIndex = 0;
		}

		public override TaskStatus OverrideStatus(TaskStatus status)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return executionStatus;
		}

		public override bool CanRunParallelChildren()
		{
			return true;
		}

		public override bool CanReevaluate()
		{
			return true;
		}

		public override bool OnReevaluationStarted()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if ((int)executionStatus == 0)
			{
				return false;
			}
			storedCurrentChildIndex = currentChildIndex;
			storedExecutionStatus = executionStatus;
			currentChildIndex = 0;
			executionStatus = 0;
			return true;
		}

		public override void OnReevaluationEnded(TaskStatus status)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			if ((int)executionStatus != 1 && (int)executionStatus != 0)
			{
				BehaviorManager.instance.Interrupt(this.get_Owner(), base.children[storedCurrentChildIndex - 1], this);
			}
			else
			{
				currentChildIndex = storedCurrentChildIndex;
				executionStatus = storedExecutionStatus;
			}
			storedCurrentChildIndex = -1;
			storedExecutionStatus = 0;
		}
	}
}
