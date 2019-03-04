namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("The interrupt task will stop all child tasks from running if it is interrupted. The interruption can be triggered by the perform interruption task. The interrupt task will keep running its child until this interruption is called. If no interruption happens and the child task completed its execution the interrupt task will return the value assigned by the child task.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=35")]
	[TaskIcon("{SkinColor}InterruptIcon.png")]
	public class Interrupt : Decorator
	{
		private TaskStatus interruptStatus = 1;

		private TaskStatus executionStatus;

		public Interrupt()
			: this()
		{
		}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


		public override bool CanExecute()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			return (int)executionStatus == 0 || (int)executionStatus == 3;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = childStatus;
		}

		public void DoInterrupt(TaskStatus status)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			interruptStatus = status;
			BehaviorManager.instance.Interrupt(this.get_Owner(), this);
		}

		public override TaskStatus OverrideStatus()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return interruptStatus;
		}

		public override void OnEnd()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			interruptStatus = 1;
			executionStatus = 0;
		}
	}
}
