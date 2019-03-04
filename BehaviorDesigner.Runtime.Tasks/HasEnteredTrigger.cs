using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Returns success when an object enters the trigger. This task will only receive the physics callback if it is being reevaluated (with a conditional abort or under a parallel task).")]
	[TaskCategory("Physics")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=110")]
	public class HasEnteredTrigger : Conditional
	{
		[Tooltip("The tag of the GameObject to check for a trigger against")]
		public SharedString tag = string.Empty;

		[Tooltip("The object that entered the trigger")]
		public SharedGameObject otherGameObject;

		private bool enteredTrigger;

		public HasEnteredTrigger()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			return (!enteredTrigger) ? 1 : 2;
		}

		public override void OnEnd()
		{
			enteredTrigger = false;
		}

		public override void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(tag.get_Value()) || tag.get_Value().Equals(other.get_gameObject().get_tag()))
			{
				otherGameObject.set_Value(other.get_gameObject());
				enteredTrigger = true;
			}
		}

		public override void OnReset()
		{
			tag = string.Empty;
			otherGameObject = null;
		}
	}
}
