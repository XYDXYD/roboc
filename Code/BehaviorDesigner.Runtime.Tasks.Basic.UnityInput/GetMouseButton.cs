using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Stores the state of the specified mouse button.")]
	public class GetMouseButton : Action
	{
		[Tooltip("The index of the button")]
		public SharedInt buttonIndex;

		[RequiredField]
		[Tooltip("The stored result")]
		public SharedBool storeResult;

		public GetMouseButton()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			storeResult.set_Value(Input.GetMouseButton(buttonIndex.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			buttonIndex = 0;
			storeResult = false;
		}
	}
}
