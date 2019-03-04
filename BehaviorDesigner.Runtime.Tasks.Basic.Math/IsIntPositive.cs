namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Is the int a positive value?")]
	public class IsIntPositive : Conditional
	{
		[Tooltip("The int to check if positive")]
		public SharedInt intVariable;

		public IsIntPositive()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (intVariable.get_Value() <= 0) ? 1 : 2;
		}

		public override void OnReset()
		{
			intVariable = 0;
		}
	}
}
