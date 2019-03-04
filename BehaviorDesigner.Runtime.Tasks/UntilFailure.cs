namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The until failure task will keep executing its child task until the child task returns failure.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=41")]
	[TaskIcon("{SkinColor}UntilFailureIcon.png")]
	public class UntilFailure : Decorator
	{
		private TaskStatus executionStatus;

		public UntilFailure()
			: this()
		{
		}

		public override bool CanExecute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			return (int)executionStatus == 2 || (int)executionStatus == 0;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = childStatus;
		}

		public override void OnEnd()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = 0;
		}
	}
}
