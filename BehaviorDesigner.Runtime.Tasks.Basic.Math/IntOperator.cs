using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Performs a math operation on two integers: Add, Subtract, Multiply, Divide, Min, or Max.")]
	public class IntOperator : Action
	{
		public enum Operation
		{
			Add,
			Subtract,
			Multiply,
			Divide,
			Min,
			Max,
			Modulo
		}

		[Tooltip("The operation to perform")]
		public Operation operation;

		[Tooltip("The first integer")]
		public SharedInt integer1;

		[Tooltip("The second integer")]
		public SharedInt integer2;

		[RequiredField]
		[Tooltip("The variable to store the result")]
		public SharedInt storeResult;

		public IntOperator()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.Add:
				storeResult.set_Value(integer1.get_Value() + integer2.get_Value());
				break;
			case Operation.Subtract:
				storeResult.set_Value(integer1.get_Value() - integer2.get_Value());
				break;
			case Operation.Multiply:
				storeResult.set_Value(integer1.get_Value() * integer2.get_Value());
				break;
			case Operation.Divide:
				storeResult.set_Value(integer1.get_Value() / integer2.get_Value());
				break;
			case Operation.Min:
				storeResult.set_Value(Mathf.Min(integer1.get_Value(), integer2.get_Value()));
				break;
			case Operation.Max:
				storeResult.set_Value(Mathf.Max(integer1.get_Value(), integer2.get_Value()));
				break;
			case Operation.Modulo:
				storeResult.set_Value(integer1.get_Value() % integer2.get_Value());
				break;
			}
			return 2;
		}

		public override void OnReset()
		{
			operation = Operation.Add;
			integer1.set_Value(0);
			integer2.set_Value(0);
			storeResult.set_Value(0);
		}
	}
}
