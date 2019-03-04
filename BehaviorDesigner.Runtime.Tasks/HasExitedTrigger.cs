using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when an object exits the trigger. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
	[TaskCategory("Physics")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=110")]
	public class HasExitedTrigger : Conditional
	{
		[Tooltip("The tag of the GameObject to check for a trigger against")]
		public SharedString tag = string.Empty;

		[Tooltip("The object that exited the trigger")]
		public SharedGameObject otherGameObject;

		private bool exitedTrigger;

		public HasExitedTrigger()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!exitedTrigger) ? 1 : 2;
		}

		public override void OnEnd()
		{
			exitedTrigger = false;
		}

		public override void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(tag.get_Value()) || tag.get_Value().Equals(other.get_gameObject().get_tag()))
			{
				otherGameObject.set_Value(other.get_gameObject());
				exitedTrigger = true;
			}
		}

		public override void OnReset()
		{
			tag = string.Empty;
			otherGameObject = null;
		}
	}
}
