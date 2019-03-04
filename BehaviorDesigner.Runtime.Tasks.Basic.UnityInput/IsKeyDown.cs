using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityInput
{
	[TaskCategory("Basic/Input")]
	[TaskDescription("Returns success when the specified key is pressed.")]
	public class IsKeyDown : Conditional
	{
		[Tooltip("The key to test")]
		public KeyCode key;

		public IsKeyDown()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (!Input.GetKeyDown(key)) ? 1 : 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			key = 0;
		}
	}
}
