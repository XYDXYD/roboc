using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController
{
	[TaskCategory("Basic/CharacterController")]
	[TaskDescription("Stores the center of the CharacterController. Returns Success.")]
	public class GetCenter : Action
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The center of the CharacterController")]
		[RequiredField]
		public SharedVector3 storeValue;

		private CharacterController characterController;

		private GameObject prevGameObject;

		public GetCenter()
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
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (characterController == null)
			{
				Debug.LogWarning((object)"CharacterController is null");
				return 1;
			}
			storeValue.set_Value(characterController.get_center());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			targetGameObject = null;
			storeValue = Vector3.get_zero();
		}
	}
}
