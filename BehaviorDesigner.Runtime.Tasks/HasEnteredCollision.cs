using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when a collision starts. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
	[TaskCategory("Physics")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=110")]
	public class HasEnteredCollision : Conditional
	{
		[Tooltip("The tag of the GameObject to check for a collision against")]
		public SharedString tag = string.Empty;

		[Tooltip("The object that started the collision")]
		public SharedGameObject collidedGameObject;

		private bool enteredCollision;

		public HasEnteredCollision()
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

		public override void OnCollisionEnter(Collision collision)
		{
			if (string.IsNullOrEmpty(tag.get_Value()) || tag.get_Value().Equals(collision.get_gameObject().get_tag()))
			{
				collidedGameObject.set_Value(collision.get_gameObject());
				enteredCollision = true;
			}
		}

		public override void OnReset()
		{
			tag = string.Empty;
			collidedGameObject = null;
		}
	}
}
