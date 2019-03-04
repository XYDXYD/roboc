using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.Math
{
	[TaskCategory("Basic/Math")]
	[TaskDescription("Performs a math operation on two floats: Add, Subtract, Multiply, Divide, Min, or Max.")]
	public class FloatOperator : Action
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

		[Tooltip("The first float")]
		public SharedFloat float1;

		[Tooltip("The second float")]
		public SharedFloat float2;

		[Tooltip("The variable to store the result")]
		public SharedFloat storeResult;

		public FloatOperator()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			switch (operation)
			{
			case Operation.Add:
				storeResult.set_Value(float1.get_Value() + float2.get_Value());
				break;
			case Operation.Subtract:
				storeResult.set_Value(float1.get_Value() - float2.get_Value());
				break;
			case Operation.Multiply:
				storeResult.set_Value(float1.get_Value() * float2.get_Value());
				break;
			case Operation.Divide:
				storeResult.set_Value(float1.get_Value() / float2.get_Value());
				break;
			case Operation.Min:
				storeResult.set_Value(Mathf.Min(float1.get_Value(), float2.get_Value()));
				break;
			case Operation.Max:
				storeResult.set_Value(Mathf.Max(float1.get_Value(), float2.get_Value()));
				break;
			case Operation.Modulo:
				storeResult.set_Value(float1.get_Value() % float2.get_Value());
				break;
			}
			return 2;
		}

		public override void OnReset()
		{
			operation = Operation.Add;
			float1.set_Value(0f);
			float2.set_Value(0f);
			storeResult.set_Value(0f);
		}
	}
}
