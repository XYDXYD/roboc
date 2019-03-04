using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityVector2
{
	[TaskCategory("Basic/Vector2")]
	[TaskDescription("Stores the X and Y values of the Vector2.")]
	public class GetXY : Action
	{
		[Tooltip("The Vector2 to get the values of")]
		public SharedVector2 vector2Variable;

		[Tooltip("The X value")]
		[RequiredField]
		public SharedFloat storeX;

		[Tooltip("The Y value")]
		[RequiredField]
		public SharedFloat storeY;

		public GetXY()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			SharedFloat sharedFloat = storeX;
			Vector2 value = vector2Variable.get_Value();
			sharedFloat.set_Value(value.x);
			SharedFloat sharedFloat2 = storeY;
			Vector2 value2 = vector2Variable.get_Value();
			sharedFloat2.set_Value(value2.y);
			return 2;
		}

		public override void OnReset()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			vector2Variable = Vector2.get_zero();
			storeX = (storeY = 0f);
		}
	}
}
