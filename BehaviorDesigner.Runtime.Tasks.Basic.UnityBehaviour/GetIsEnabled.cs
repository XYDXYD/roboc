using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityBehaviour
{
	[TaskCategory("Basic/Behaviour")]
	[TaskDescription("Stores the enabled state of the object. Returns Success.")]
	public class GetIsEnabled : Action
	{
		[Tooltip("The Object to use")]
		public SharedObject specifiedObject;

		[Tooltip("The enabled/disabled state")]
		[RequiredField]
		public SharedBool storeValue;

		public GetIsEnabled()
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
			storeValue.set_Value((specifiedObject.get_Value() as Behaviour).get_enabled());
			return 2;
		}

		public override void OnReset()
		{
			if (specifiedObject != null)
			{
				specifiedObject.set_Value(null);
			}
			storeValue = false;
		}
	}
}
