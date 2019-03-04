using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityCharacterController
{
	[TaskCategory("Basic/CharacterController")]
	[TaskDescription("Returns Success if the collider hit another object, otherwise Failure.")]
	public class HasColliderHit : Conditional
	{
		[Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
		public SharedGameObject targetGameObject;

		[Tooltip("The tag of the GameObject to check for a collision against")]
		public SharedString tag = string.Empty;

		[Tooltip("The object that started the collision")]
		public SharedGameObject collidedGameObject;

		private bool enteredCollision;

		public HasColliderHit()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!enteredCollision) ? 1 : 2;
		}

		public override void OnEnd()
		{
			enteredCollision = false;
		}

		public override void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (string.IsNullOrEmpty(tag.get_Value()) || tag.get_Value().Equals(hit.get_gameObject().get_tag()))
			{
				collidedGameObject.set_Value(hit.get_gameObject());
				enteredCollision = true;
			}
		}

		public override void OnReset()
		{
			targetGameObject = null;
			tag = string.Empty;
			collidedGameObject = null;
		}
	}
}
