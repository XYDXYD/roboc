namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Performs comparison between two integers: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
	public class IntComparison : Conditional
	{
		public enum Operation
		{
			LessThan,
			LessThanOrEqualTo,
			EqualTo,
			NotEqualTo,
			GreaterThanOrEqualTo,
			GreaterThan
		}

		[Tooltip("The operation to perform")]
		public Operation operation;

		[Tooltip("The first integer")]
		public SharedInt integer1;

		[Tooltip("The second integer")]
		public SharedInt integer2;

		public IntComparison()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.LessThan:
				return (integer1.get_Value() >= integer2.get_Value()) ? 1 : 2;
			case Operation.LessThanOrEqualTo:
				return (integer1.get_Value() > integer2.get_Value()) ? 1 : 2;
			case Operation.EqualTo:
				return (integer1.get_Value() != integer2.get_Value()) ? 1 : 2;
			case Operation.NotEqualTo:
				return (integer1.get_Value() == integer2.get_Value()) ? 1 : 2;
			case Operation.GreaterThanOrEqualTo:
				return (integer1.get_Value() < integer2.get_Value()) ? 1 : 2;
			case Operation.GreaterThan:
				return (integer1.get_Value() <= integer2.get_Value()) ? 1 : 2;
			default:
				return 1;
			}
		}

		public override void OnReset()
		{
			operation = Operation.LessThan;
			integer1.set_Value(0);
			integer2.set_Value(0);
		}
	}
}
