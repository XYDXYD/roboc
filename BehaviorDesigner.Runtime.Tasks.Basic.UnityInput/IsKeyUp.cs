using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Returns success when the specified key is released.")]
	public class IsKeyUp : Conditional
	{
		[Tooltip("The key to test")]
		public KeyCode key;

		public IsKeyUp()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (!Input.GetKeyUp(key)) ? 1 : 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			key = 0;
		}
	}
}
