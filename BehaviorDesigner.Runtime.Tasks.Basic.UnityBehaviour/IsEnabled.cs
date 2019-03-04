using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityBehaviour
{
	[TaskCategory("Basic/Behaviour")]
	[TaskDescription("Returns Success if the object is enabled, otherwise Failure.")]
	public class IsEnabled : Conditional
	{
		[Tooltip("The Object to use")]
		public SharedObject specifiedObject;

		public IsEnabled()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (specifiedObject == null && !(specifiedObject.get_Value() is Behaviour))
			{
				Debug.LogWarning((object)"SpecifiedObject is null or not a subclass of UnityEngine.Behaviour");
				return 1;
			}
			return (!(specifiedObject.get_Value() as Behaviour).get_enabled()) ? 1 : 2;
		}

		public override void OnReset()
		{
			if (specifiedObject != null)
			{
				specifiedObject.set_Value(null);
			}
		}
	}
}
