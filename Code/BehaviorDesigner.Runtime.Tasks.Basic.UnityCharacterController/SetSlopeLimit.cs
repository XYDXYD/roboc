using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController
{
	[TaskCategory("Basic/CharacterController")]
	[TaskDescription("Sets the slope limit of the CharacterController. Returns Success.")]
	public class SetSlopeLimit : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The slope limit of the CharacterController")]
		public SharedFloat slopeLimit;

		private CharacterController characterController;

		private GameObject prevGameObject;

		public SetSlopeLimit()
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
			characterController.set_slopeLimit(slopeLimit.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			slopeLimit = 0f;
		}
	}
}
