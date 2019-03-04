using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Clamps the magnitude of the Vector3.")]
	public class ClampMagnitude : Action
	{
		[Tooltip("The Vector3 to clamp the magnitude of")]
		public SharedVector3 vector3Variable;

		[Tooltip("The max length of the magnitude")]
		public SharedFloat maxLength;

		[Tooltip("The clamp magnitude resut")]
		[RequiredField]
		public SharedVector3 storeResult;

		public ClampMagnitude()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector3.ClampMagnitude(vector3Variable.get_Value(), maxLength.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			vector3Variable = (storeResult = Vector3.get_zero());
			maxLength = 0f;
		}
	}
}
