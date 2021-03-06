using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController
{
	[TaskCategory("Basic/CharacterController")]
	[TaskDescription("Stores the step offset of the CharacterController. Returns Success.")]
	public class GetStepOffset : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The step offset of the CharacterController")]
		[RequiredField]
		public SharedFloat storeValue;

		private CharacterController characterController;

		private GameObject prevGameObject;

		public GetStepOffset()
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
			storeValue.set_Value(characterController.get_stepOffset());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			storeValue = 0f;
		}
	}
}
