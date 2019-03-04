using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Stores the value of the specified axis and stores it in a float.")]
	public class GetAxis : Action
	{
		[Tooltip("The name of the axis")]
		public SharedString axisName;

		[Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range")]
		public SharedFloat multiplier;

		[RequiredField]
		[Tooltip("The stored result")]
		public SharedFloat storeResult;

		public GetAxis()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			float num = Input.GetAxis(axisName.get_Value());
			if (!multiplier.get_IsNone())
			{
				num *= multiplier.get_Value();
			}
			storeResult.set_Value(num);
			return 2;
		}

		public override void OnReset()
		{
			axisName = string.Empty;
			multiplier = 1f;
			storeResult = 0f;
		}
	}
}
