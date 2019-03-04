namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Similar to the parallel selector task, except the parallel complete task will return the child status as soon as the child returns success or failure.The child tasks are executed simultaneously.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=136")]
	[TaskIcon("{SkinColor}ParallelCompleteIcon.png")]
	public class ParallelComplete : Composite
	{
		private int currentChildIndex;

		private TaskStatus[] executionStatus;

		public ParallelComplete()
			: this()
		{
		}

		public override void OnAwake()
		{
			executionStatus = (TaskStatus[])new TaskStatus[base.children.Count];
		}

		public override void OnChildStarted(int childIndex)
		{
			currentChildIndex++;
			executionStatus[childIndex] = 3;
		}

		public override bool CanRunParallelChildren()
		{
			return true;
		}

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override bool CanExecute()
		{
			return currentChildIndex < base.children.Count;
		}

		public override void OnChildExecuted(int childIndex, TaskStatus childStatus)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Expected I4, but got Unknown
			executionStatus[childIndex] = (int)childStatus;
		}

		public override void OnConditionalAbort(int childIndex)
		{
			currentChildIndex = 0;
			for (int i = 0; i < executionStatus.Length; i++)
			{
				executionStatus[i] = 0;
			}
		}

		public override TaskStatus OverrideStatus(TaskStatus status)
		{
			for (int i = 0; i < executionStatus.Length; i++)
			{
				if ((int)executionStatus[i] == 2 || (int)executionStatus[i] == 1)
				{
					return executionStatus[i];
				}
				if ((int)executionStatus[i] == 0)
				{
					return 2;
				}
			}
			return 3;
		}

		public override void OnEnd()
		{
			for (int i = 0; i < executionStatus.Length; i++)
			{
				executionStatus[i] = 0;
			}
			currentChildIndex = 0;
		}
	}
}
