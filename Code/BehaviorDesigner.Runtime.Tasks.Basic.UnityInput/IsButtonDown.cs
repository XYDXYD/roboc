using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Returns success when the specified button is pressed.")]
	public class IsButtonDown : Conditional
	{
		[Tooltip("The name of the button")]
		public SharedString buttonName;

		public IsButtonDown()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!Input.GetButtonDown(buttonName.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			buttonName = "Fire1";
		}
	}
}
