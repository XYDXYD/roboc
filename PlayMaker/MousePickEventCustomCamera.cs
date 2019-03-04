using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("Sends Events based on mouse interactions with a Game Object: MouseOver, MouseDown, MouseUp, MouseOff. Specify a camera target")]
	public class MousePickEventCustomCamera : FsmStateAction
	{
		[Tooltip("target game object or parent..")]
		public FsmGameObject targetObject;

		[Tooltip("Length of the ray to cast from the camera.")]
		public FsmFloat rayDistance = FsmFloat.op_Implicit(100f);

		[Tooltip("Event to send when the mouse is over the GameObject.")]
		public FsmEvent mouseOver;

		[Tooltip("Event to send when the mouse is pressed while over the GameObject.")]
		public FsmEvent mouseDown;

		[Tooltip("Event to send when the mouse is released while over the GameObject.")]
		public FsmEvent mouseUp;

		[Tooltip("Event to send when the mouse moves off the GameObject.")]
		public FsmEvent mouseOff;

		[Tooltip("Pick only from these layers.")]
		[UIHint(/*Could not decode attribute arguments.*/)]
		public FsmInt layersMask;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[Tooltip("store outcome in this boolean.")]
		public FsmBool outputIsPicked;

		[Tooltip("Camera Target")]
		[CheckForComponent(typeof(Camera))]
		public FsmOwnerDefault camera;

		private Camera _cameraTarget;

		private static Ray ray;

		public MousePickEventCustomCamera()
			: this()
		{
		}

		public override void Reset()
		{
			rayDistance = FsmFloat.op_Implicit(100f);
			mouseOver = null;
			mouseDown = null;
			mouseUp = null;
			mouseOff = null;
			layersMask = FsmInt.op_Implicit(0);
			everyFrame = true;
			camera = null;
			_cameraTarget = null;
		}

		public override void OnEnter()
		{
			_cameraTarget = camera.get_GameObject().get_Value().GetComponent<Camera>();
			outputIsPicked.set_Value(false);
			DoMousePickEvent();
			if (!everyFrame)
			{
				this.Finish();
			}
		}

		public override void OnUpdate()
		{
			DoMousePickEvent();
		}

		private void DoMousePickEvent()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			bool flag = DoRaycast();
			this.get_Fsm().set_RaycastHitInfo(ActionHelpers.mousePickInfo);
			if (flag)
			{
				if (mouseDown != null && Input.GetMouseButtonDown(0))
				{
					this.get_Fsm().Event(mouseDown);
				}
				if (mouseOver != null)
				{
					this.get_Fsm().Event(mouseOver);
				}
				if (mouseUp != null && Input.GetMouseButtonUp(0))
				{
					this.get_Fsm().Event(mouseUp);
				}
				outputIsPicked.set_Value(true);
			}
			else
			{
				if (mouseOff != null)
				{
					this.get_Fsm().Event(mouseOff);
				}
				outputIsPicked.set_Value(false);
			}
		}

		private bool DoRaycast()
		{
			return IsMouseOver(targetObject.get_Value(), rayDistance.get_Value(), _cameraTarget, layersMask.get_Value());
		}

		private bool IsMouseOver(GameObject target, float maxRayDistance, Camera camera, int layerMask)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			ray = camera.ScreenPointToRay(Input.get_mousePosition());
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(ray, ref val, maxRayDistance, 1 << layerMask) && Object.op_Implicit(val.get_collider()))
			{
				GameObject gameObject = val.get_collider().get_gameObject();
				while (gameObject != null)
				{
					if (gameObject == target)
					{
						Debug.DrawLine(ray.get_origin(), ray.get_direction(), Color.get_green());
						return true;
					}
					if (Object.op_Implicit(gameObject.get_transform().get_parent()))
					{
						gameObject = gameObject.get_transform().get_parent().get_gameObject();
						continue;
					}
					return false;
				}
			}
			Debug.DrawLine(ray.get_origin(), ray.get_direction(), Color.get_red());
			return false;
		}

		public override string ErrorCheck()
		{
			string empty = string.Empty;
			return empty + ActionHelpers.CheckRayDistance(rayDistance.get_Value());
		}
	}
}
