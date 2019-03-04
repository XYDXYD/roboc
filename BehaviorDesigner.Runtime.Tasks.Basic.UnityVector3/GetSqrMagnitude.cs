using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Stores the square magnitude of the Vector3.")]
	public class GetSqrMagnitude : Action
	{
		[Tooltip("The Vector3 to get the square magnitude of")]
		public SharedVector3 vector3Variable;

		[Tooltip("The square magnitude of the vector")]
		[RequiredField]
		public SharedFloat storeResult;

		public GetSqrMagnitude()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			SharedFloat sharedFloat = storeResult;
			Vector3 value = vector3Variable.get_Value();
			sharedFloat.set_Value(value.get_sqrMagnitude());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			vector3Variable = Vector3.get_zero();
			storeResult = 0f;
		}
	}
}
