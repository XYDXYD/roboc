namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Performs a math operation on two bools: AND, OR, NAND, or XOR.")]
	public class BoolOperator : Action
	{
		public enum Operation
		{
			AND,
			OR,
			NAND,
			XOR
		}

		[Tooltip("The operation to perform")]
		public Operation operation;

		[Tooltip("The first bool")]
		public SharedBool bool1;

		[Tooltip("The second bool")]
		public SharedBool bool2;

		[Tooltip("The variable to store the result")]
		public SharedBool storeResult;

		public BoolOperator()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.AND:
				storeResult.set_Value(bool1.get_Value() && bool2.get_Value());
				break;
			case Operation.OR:
				storeResult.set_Value(bool1.get_Value() || bool2.get_Value());
				break;
			case Operation.NAND:
				storeResult.set_Value(!bool1.get_Value() || !bool2.get_Value());
				break;
			case Operation.XOR:
				storeResult.set_Value(bool1.get_Value() ^ bool2.get_Value());
				break;
			}
			return 2;
		}

		public override void OnReset()
		{
			operation = Operation.AND;
			bool1.set_Value(false);
			bool2.set_Value(false);
			storeResult.set_Value(false);
		}
	}
}
