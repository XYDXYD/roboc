using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Returns success when the specified mouse button is pressed.")]
	public class IsMouseDown : Conditional
	{
		[Tooltip("The button index")]
		public SharedInt buttonIndex;

		public IsMouseDown()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!Input.GetMouseButtonDown(buttonIndex.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			buttonIndex = 0;
		}
	}
}
