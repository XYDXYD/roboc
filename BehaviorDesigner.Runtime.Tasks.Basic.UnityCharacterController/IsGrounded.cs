using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController
{
	[TaskCategory("Basic/CharacterController")]
	[TaskDescription("Returns Success if the character is grounded, otherwise Failure.")]
	public class IsGrounded : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		private CharacterController characterController;

		private GameObject prevGameObject;

		public IsGrounded()
			: this()
		{
		}

		public override void OnStart()
		{
			GameObject defaultGameObject = this.GetDefaultGameObject(targetGameObject.get_Value());
			if (defaultGameObject != prevGameObject)
			{
				characterController = defaultGameObject.GetComponent<CharacterController>();
				prevGameObject = defaultGameObject;
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (characterController == null)
			{
				Debug.LogWarning((object)"CharacterController is null");
				return 1;
			}
			return (!characterController.get_isGrounded()) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
		}
	}
}
