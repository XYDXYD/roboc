using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Stores the mouse position.")]
	public class GetMousePosition : Action
	{
		[RequiredField]
		[Tooltip("The stored result")]
		public SharedVector2 storeResult;

		public GetMousePosition()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector2.op_Implicit(Input.get_mousePosition()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			storeResult = Vector2.get_zero();
		}
	}
}
