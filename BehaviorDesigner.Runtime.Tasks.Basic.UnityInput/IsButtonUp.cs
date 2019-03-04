using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Returns success when the specified button is released.")]
	public class IsButtonUp : Conditional
	{
		[Tooltip("The name of the button")]
		public SharedString buttonName;

		public IsButtonUp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!Input.GetButtonUp(buttonName.get_Value())) ? 1 : 2;
		}

		public override void OnReset()
		{
			buttonName = "Fire1";
		}
	}
}
