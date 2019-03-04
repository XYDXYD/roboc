using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when a collision ends. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
	[TaskCategory("Physics")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=110")]
	public class HasExitedCollision : Conditional
	{
		[Tooltip("The tag of the GameObject to check for a collision against")]
		public SharedString tag = string.Empty;

		[Tooltip("The object that exited the collision")]
		public SharedGameObject collidedGameObject;

		private bool exitedCollision;

		public HasExitedCollision()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!exitedCollision) ? 1 : 2;
		}

		public override void OnEnd()
		{
			exitedCollision = false;
		}

		public override void OnCollisionExit(Collision collision)
		{
			if (string.IsNullOrEmpty(tag.get_Value()) || tag.get_Value().Equals(collision.get_gameObject().get_tag()))
			{
				collidedGameObject.set_Value(collision.get_gameObject());
				exitedCollision = true;
			}
		}

		public override void OnReset()
		{
			collidedGameObject = null;
		}
	}
}
