using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Returns the angle between two Vector3s.")]
	public class Angle : Action
	{
		[Tooltip("The first Vector3")]
		public SharedVector3 firstVector3;

		[Tooltip("The second Vector3")]
		public SharedVector3 secondVector3;

		[Tooltip("The angle")]
		[RequiredField]
		public SharedFloat storeResult;

		public Angle()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector3.Angle(firstVector3.get_Value(), secondVector3.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			firstVector3 = (secondVector3 = Vector3.get_zero());
			storeResult = 0f;
		}
	}
}
