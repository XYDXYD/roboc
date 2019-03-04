using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector3
{
	[TaskCategory("Basic/Vector3")]
	[TaskDescription("Rotate the current rotation to the target rotation.")]
	public class RotateTowards : Action
	{
		[Tooltip("The current rotation in euler angles")]
		public SharedVector3 currentRotation;

		[Tooltip("The target rotation in euler angles")]
		public SharedVector3 targetRotation;

		[Tooltip("The maximum delta of the degrees")]
		public SharedFloat maxDegreesDelta;

		[Tooltip("The maximum delta of the magnitude")]
		public SharedFloat maxMagnitudeDelta;

		[Tooltip("The rotation resut")]
		[RequiredField]
		public SharedVector3 storeResult;

		public RotateTowards()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			storeResult.set_Value(Vector3.RotateTowards(currentRotation.get_Value(), targetRotation.get_Value(), maxDegreesDelta.get_Value() * ((float)System.Math.PI / 180f) * Time.get_deltaTime(), maxMagnitudeDelta.get_Value()));
			return 2;
		}

		public override void OnReset()
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			currentRotation = (targetRotation = (storeResult = Vector3.get_zero()));
			maxDegreesDelta = (maxMagnitudeDelta = 0f);
		}
	}
}
