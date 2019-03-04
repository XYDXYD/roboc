using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityBehaviour
{
	[TaskCategory("Basic/Behaviour")]
	[TaskDescription("Enables/Disables the object. Returns Success.")]
	public class SetIsEnabled : Action
	{
		[Tooltip("The Object to use")]
		public SharedObject specifiedObject;

		[Tooltip("The enabled/disabled state")]
		public SharedBool enabled;

		public SetIsEnabled()
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
			(specifiedObject.get_Value() as Behaviour).set_enabled(enabled.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			if (specifiedObject != null)
			{
				specifiedObject.set_Value(null);
			}
			enabled = false;
		}
	}
}
