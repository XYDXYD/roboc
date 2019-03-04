namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Performs comparison between two floats: less than, less than or equal to, equal to, not equal to, greater than or equal to, or greater than.")]
	public class FloatComparison : Conditional
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

		[Tooltip("The first float")]
		public SharedFloat float1;

		[Tooltip("The second float")]
		public SharedFloat float2;

		public FloatComparison()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.LessThan:
				return (!(float1.get_Value() < float2.get_Value())) ? 1 : 2;
			case Operation.LessThanOrEqualTo:
				return (!(float1.get_Value() <= float2.get_Value())) ? 1 : 2;
			case Operation.EqualTo:
				return (float1.get_Value() != float2.get_Value()) ? 1 : 2;
			case Operation.NotEqualTo:
				return (float1.get_Value() == float2.get_Value()) ? 1 : 2;
			case Operation.GreaterThanOrEqualTo:
				return (!(float1.get_Value() >= float2.get_Value())) ? 1 : 2;
			case Operation.GreaterThan:
				return (!(float1.get_Value() > float2.get_Value())) ? 1 : 2;
			default:
				return 1;
			}
		}

		public override void OnReset()
		{
			operation = Operation.LessThan;
			float1.set_Value(0f);
			float2.set_Value(0f);
		}
	}
}
