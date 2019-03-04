using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Stores the dot product of two Vector3 values.")]
	public class Dot : Action
	{
		[Tooltip("The left hand side of the dot product")]
		public SharedVector3 leftHandSide;

		[Tooltip("The right hand side of the dot product")]
		public SharedVector3 rightHandSide;

		[Tooltip("The dot product result")]
		[RequiredField]
		public SharedFloat storeResult;

		public Dot()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector3.Dot(leftHandSide.get_Value(), rightHandSide.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			leftHandSide = (rightHandSide = Vector3.get_zero());
			storeResult = 0f;
		}
	}
}
