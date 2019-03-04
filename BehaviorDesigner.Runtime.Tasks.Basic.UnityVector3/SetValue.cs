using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Sets the value of the Vector3.")]
	public class SetValue : Action
	{
		[Tooltip("The Vector3 to get the values of")]
		public SharedVector3 vector3Value;

		[Tooltip("The Vector3 to set the values of")]
		public SharedVector3 vector3Variable;

		public SetValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			vector3Variable.set_Value(vector3Value.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			vector3Value = (vector3Variable = Vector3.get_zero());
		}
	}
}
