namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The sequence task is similar to an \"and\" operation. It will return failure as soon as one of its child tasks return failure. If a child task returns success then it will sequentially run the next task. If all child tasks return success then it will return success.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=25")]
	[TaskIcon("{SkinColor}SequenceIcon.png")]
	public class Sequence : Composite
	{
		private int currentChildIndex;

		private TaskStatus executionStatus;

		public Sequence()
			: this()
		{
		}

		public override int CurrentChildIndex()
		{
			return currentChildIndex;
		}

		public override bool CanExecute()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			return currentChildIndex < base.children.Count && (int)executionStatus != 1;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			currentChildIndex++;
			executionStatus = childStatus;
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
	}
}
