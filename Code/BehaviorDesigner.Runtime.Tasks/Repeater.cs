namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The repeater task will repeat execution of its child task until the child task has been run a specified number of times. It has the option of continuing to execute the child task even if the child task returns a failure.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=37")]
	[TaskIcon("{SkinColor}RepeaterIcon.png")]
	public class Repeater : Decorator
	{
		[Tooltip("The number of times to repeat the execution of its child task")]
		public SharedInt count = 1;

		[Tooltip("Allows the repeater to repeat forever")]
		public SharedBool repeatForever;

		[Tooltip("Should the task return if the child task returns a failure")]
		public SharedBool endOnFailure;

		private int executionCount;

		private TaskStatus executionStatus;

		public Repeater()
			: this()
		{
		}

		public override bool CanExecute()
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			return (repeatForever.get_Value() || executionCount < count.get_Value()) && (!endOnFailure.get_Value() || (endOnFailure.get_Value() && (int)executionStatus != 1));
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			executionCount++;
			executionStatus = childStatus;
		}

		public override void OnEnd()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			executionCount = 0;
			executionStatus = 0;
		}

		public override void OnReset()
		{
			count = 0;
			endOnFailure = true;
		}
	}
}
