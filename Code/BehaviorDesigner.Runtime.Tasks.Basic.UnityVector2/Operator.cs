using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Performs a math operation on two Vector2s: Add, Subtract, Multiply, Divide, Min, or Max.")]
	public class Operator : Action
	{
		public enum Operation
		{
			Add,
			Subtract,
			Scale
		}

		[Tooltip("The operation to perform")]
		public Operation operation;

		[Tooltip("The first Vector2")]
		public SharedVector2 firstVector2;

		[Tooltip("The second Vector2")]
		public SharedVector2 secondVector2;

		[Tooltip("The variable to store the result")]
		public SharedVector2 storeResult;

		public Operator()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			switch (operation)
			{
			case Operation.Add:
				storeResult.set_Value(firstVector2.get_Value() + secondVector2.get_Value());
				break;
			case Operation.Subtract:
				storeResult.set_Value(firstVector2.get_Value() - secondVector2.get_Value());
				break;
			case Operation.Scale:
				storeResult.set_Value(Vector2.Scale(firstVector2.get_Value(), secondVector2.get_Value()));
				break;
			}
			return 2;
		}

		public override void OnReset()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			operation = Operation.Add;
			firstVector2 = (secondVector2 = (storeResult = Vector2.get_zero()));
		}
	}
}
