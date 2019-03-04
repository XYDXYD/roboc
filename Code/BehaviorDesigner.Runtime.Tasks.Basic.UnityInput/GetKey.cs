using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Stores the pressed state of the specified key.")]
	public class GetKey : Action
	{
		[Tooltip("The key to test.")]
		public KeyCode key;

		[RequiredField]
		[Tooltip("The stored result")]
		public SharedBool storeResult;

		public GetKey()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Input.GetKey(key));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			key = 0;
			storeResult = false;
		}
	}
}
