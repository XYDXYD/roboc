using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Stores the up vector value.")]
	public class GetUpVector : Action
	{
		[Tooltip("The stored result")]
		[RequiredField]
		public SharedVector2 storeResult;

		public GetUpVector()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector2.get_up());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			storeResult = Vector2.get_zero();
		}
	}
}
