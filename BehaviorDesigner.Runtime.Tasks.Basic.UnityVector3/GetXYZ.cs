using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Stores the X, Y, and Z values of the Vector3.")]
	public class GetXYZ : Action
	{
		[Tooltip("The Vector3 to get the values of")]
		public SharedVector3 vector3Variable;

		[Tooltip("The X value")]
		[RequiredField]
		public SharedFloat storeX;

		[Tooltip("The Y value")]
		[RequiredField]
		public SharedFloat storeY;

		[Tooltip("The Z value")]
		[RequiredField]
		public SharedFloat storeZ;

		public GetXYZ()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			SharedFloat sharedFloat = storeX;
			Vector3 value = vector3Variable.get_Value();
			sharedFloat.set_Value(value.x);
			SharedFloat sharedFloat2 = storeY;
			Vector3 value2 = vector3Variable.get_Value();
			sharedFloat2.set_Value(value2.y);
			SharedFloat sharedFloat3 = storeZ;
			Vector3 value3 = vector3Variable.get_Value();
			sharedFloat3.set_Value(value3.z);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			vector3Variable = Vector3.get_zero();
			storeX = (storeY = (storeZ = 0f));
		}
	}
}
