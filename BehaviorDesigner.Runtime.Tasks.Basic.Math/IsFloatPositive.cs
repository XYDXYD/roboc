namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Is the float a positive value?")]
	public class IsFloatPositive : Conditional
	{
		[Tooltip("The float to check if positive")]
		public SharedFloat floatVariable;

		public IsFloatPositive()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!(floatVariable.get_Value() > 0f)) ? 1 : 2;
		}

		public override void OnReset()
		{
			floatVariable = 0f;
		}
	}
}
