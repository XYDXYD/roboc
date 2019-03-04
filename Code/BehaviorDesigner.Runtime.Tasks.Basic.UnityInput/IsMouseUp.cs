using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Returns success when the specified mouse button is pressed.")]
	public class IsMouseUp : Conditional
	{
		[Tooltip("The button index")]
		public SharedInt buttonIndex;

		public IsMouseUp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!Input.GetMouseButtonUp(buttonIndex.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			buttonIndex = 0;
		}
	}
}
