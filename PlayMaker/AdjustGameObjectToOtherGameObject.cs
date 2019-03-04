using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("adjust one game object with respect to another")]
	public class AdjustGameObjectToOtherGameObject : FsmStateAction
	{
		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Game object position adjuster")]
		public FsmGameObject adjustedGameObject;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Game object scale adjuster")]
		public FsmGameObject gameObjectScaler;

		[UIHint(/*Could not decode attribute arguments.*/)]
		[Tooltip("Game object to use as reference")]
		public FsmGameObject referenceGameObject;

		[Tooltip("make object bigger if it is smaller than the reference")]
		public bool scaleTargetObjectUp;

		[Tooltip("make object smaller if it is bigger than the reference")]
		public bool scaleTargetObjectDown;

		[Tooltip("max object scale up factor")]
		public float maxScaleUpFactor;

		[Tooltip("centre the target object so its cumulative bounding box centre is at the centre of the reference objects cumulative bounding box centre")]
		public bool centreTargetObject;

		public AdjustGameObjectToOtherGameObject()
			: this()
		{
		}

		public override void OnEnter()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			Vector3 offsetToCentre = Vector3.get_zero();
			Vector3 offsetToCentre2 = Vector3.get_zero();
			GameObject value = adjustedGameObject.get_Value();
			Bounds targetObjectBounds = CalculateGameObjectRendererBounds(value, out offsetToCentre);
			Bounds referenceObjectBounds = CalculateGameObjectBounds(referenceGameObject.get_Value(), out offsetToCentre2);
			float scaleFactor = GetScaleFactor(targetObjectBounds, referenceObjectBounds);
			if (scaleTargetObjectUp && scaleFactor > 1f)
			{
				ScaleTargetObject(Mathf.Min(maxScaleUpFactor, scaleFactor));
			}
			if (scaleTargetObjectDown && scaleFactor < 1f)
			{
				ScaleTargetObject(scaleFactor);
			}
			if (centreTargetObject)
			{
				ShiftObjectDownToMinOfTarget();
			}
			this.Finish();
		}

		public override void OnUpdate()
		{
			this.Finish();
		}

		private void ShiftObjectDownToMinOfTarget()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			Vector3 offsetToCentre = Vector3.get_zero();
			Vector3 offsetToCentre2 = Vector3.get_zero();
			GameObject value = adjustedGameObject.get_Value();
			Bounds val = CalculateGameObjectRendererBounds(value, out offsetToCentre);
			val = CalculateGameObjectRendererBounds(value, out offsetToCentre);
			Bounds val2 = CalculateGameObjectBounds(referenceGameObject.get_Value(), out offsetToCentre2);
			Vector3 center = val.get_center();
			float x = center.x;
			Vector3 center2 = val2.get_center();
			float num = x - center2.x;
			Vector3 center3 = val.get_center();
			float y = center3.y;
			Vector3 center4 = val2.get_center();
			float num2 = y - center4.y;
			Vector3 center5 = val.get_center();
			float z = center5.z;
			Vector3 center6 = val2.get_center();
			float num3 = z - center6.z;
			float num4 = 0f - num;
			float num5 = 0f - num2;
			Vector3 size = val.get_size();
			Vector3 val3 = default(Vector3);
			val3._002Ector(num4, num5 + size.y / 2f, 0f - num3);
			Transform transform = adjustedGameObject.get_Value().get_transform();
			transform.set_position(transform.get_position() + val3);
		}

		private float GetScaleFactor(Bounds targetObjectBounds, Bounds referenceObjectBounds)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			Vector3 size = referenceObjectBounds.get_size();
			float x = size.x;
			Vector3 size2 = targetObjectBounds.get_size();
			float num = x / size2.x;
			Vector3 size3 = referenceObjectBounds.get_size();
			float y = size3.y;
			Vector3 size4 = targetObjectBounds.get_size();
			float num2 = y / size4.y;
			Vector3 size5 = referenceObjectBounds.get_size();
			float z = size5.z;
			Vector3 size6 = targetObjectBounds.get_size();
			float num3 = z / size6.z;
			float num4 = num;
			if (num2 < num4)
			{
				num4 = num2;
			}
			if (num3 < num4)
			{
				num4 = num3;
			}
			return num4;
		}

		private void ScaleTargetObject(float scaleFactor)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			GameObject value = gameObjectScaler.get_Value();
			value.get_transform().set_localScale(new Vector3(scaleFactor, scaleFactor, scaleFactor));
		}

		private Bounds CalculateGameObjectRendererBounds(GameObject targetGO, out Vector3 offsetToCentre)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			offsetToCentre = Vector3.get_zero();
			Vector3 position = targetGO.get_transform().get_position();
			targetGO.get_transform().set_position(Vector3.get_zero());
			Renderer[] componentsInChildren = targetGO.GetComponentsInChildren<Renderer>();
			Bounds result = default(Bounds);
			foreach (Renderer val in componentsInChildren)
			{
				if (result.get_extents() == Vector3.get_zero())
				{
					result = val.get_bounds();
				}
				else
				{
					result.Encapsulate(val.get_bounds());
				}
			}
			offsetToCentre = result.get_max() + result.get_min();
			offsetToCentre *= 0.5f;
			targetGO.get_transform().set_position(position);
			return result;
		}

		private Bounds CalculateGameObjectBounds(GameObject targetGO, out Vector3 offsetToCentre)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			offsetToCentre = Vector3.get_zero();
			Vector3 position = targetGO.get_transform().get_position();
			targetGO.get_transform().set_position(Vector3.get_zero());
			BoxCollider[] componentsInChildren = targetGO.GetComponentsInChildren<BoxCollider>();
			MeshCollider[] componentsInChildren2 = targetGO.GetComponentsInChildren<MeshCollider>();
			Bounds result = default(Bounds);
			result._002Ector(targetGO.get_transform().get_position(), Vector3.get_zero());
			foreach (BoxCollider val in componentsInChildren)
			{
				if (val.get_gameObject().get_layer() != GameLayers.CUBE_EXTENTS)
				{
					Bounds bounds = val.get_bounds();
					result.Encapsulate(bounds.get_min());
					Bounds bounds2 = val.get_bounds();
					result.Encapsulate(bounds2.get_max());
				}
			}
			foreach (MeshCollider val2 in componentsInChildren2)
			{
				if (val2.get_gameObject().get_layer() != GameLayers.CUBE_EXTENTS)
				{
					Bounds bounds3 = val2.get_bounds();
					result.Encapsulate(bounds3.get_min());
					Bounds bounds4 = val2.get_bounds();
					result.Encapsulate(bounds4.get_max());
				}
			}
			offsetToCentre = result.get_max() + result.get_min();
			offsetToCentre *= 0.5f;
			targetGO.get_transform().set_position(position);
			return result;
		}
	}
}
