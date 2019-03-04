using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityDebug
{
	[TaskCategory("Basic/Debug")]
	[TaskDescription("Draws a debug line")]
	public class DrawLine : Action
	{
		[Tooltip("The start position")]
		public SharedVector3 start;

		[Tooltip("The end position")]
		public SharedVector3 end;

		[Tooltip("The color")]
		public SharedColor color = Color.get_white();

		[Tooltip("Duration the line will be visible for in seconds.\nDefault: 0 means 1 frame.")]
		public SharedFloat duration;

		[Tooltip("Whether the line should show through world geometry.")]
		public SharedBool depthTest = true;

		public DrawLine()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


		public override TaskStatus OnUpdate()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			Debug.DrawLine(start.get_Value(), end.get_Value(), color.get_Value(), duration.get_Value(), depthTest.get_Value());
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			start = Vector3.get_zero();
			end = Vector3.get_zero();
			color = Color.get_white();
			duration = 0f;
			depthTest = true;
		}
	}
}
