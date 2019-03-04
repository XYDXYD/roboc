using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(Image))]
	public class TouchJoystickExample : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEventSystemHandler
	{
		public bool allowMouseControl = true;

		public int radius = 50;

		private Vector2 origAnchoredPosition;

		private Vector3 origWorldPosition;

		private Vector2 origScreenResolution;

		private ScreenOrientation origScreenOrientation;

		[NonSerialized]
		private bool hasFinger;

		[NonSerialized]
		private int lastFingerId;

		public Vector2 position
		{
			get;
			private set;
		}

		public TouchJoystickExample()
			: this()
		{
		}

		private void Start()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			if ((int)SystemInfo.get_deviceType() == 1)
			{
				allowMouseControl = false;
			}
			StoreOrigValues();
		}

		private void Update()
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((float)Screen.get_width() != origScreenResolution.x || (float)Screen.get_height() != origScreenResolution.y || Screen.get_orientation() != origScreenOrientation)
			{
				Restart();
				StoreOrigValues();
			}
		}

		private void Restart()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			hasFinger = false;
			(this.get_transform() as RectTransform).set_anchoredPosition(origAnchoredPosition);
			position = Vector2.get_zero();
		}

		private void StoreOrigValues()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			origAnchoredPosition = (this.get_transform() as RectTransform).get_anchoredPosition();
			origWorldPosition = this.get_transform().get_position();
			origScreenResolution = new Vector2((float)Screen.get_width(), (float)Screen.get_height());
			origScreenOrientation = Screen.get_orientation();
		}

		private void UpdateValue(Vector3 value)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = origWorldPosition - value;
			val.y = 0f - val.y;
			val /= (float)radius;
			position = new Vector2(0f - val.x, val.y);
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (!hasFinger && (allowMouseControl || !IsMousePointerId(eventData.get_pointerId())))
			{
				hasFinger = true;
				lastFingerId = eventData.get_pointerId();
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (eventData.get_pointerId() == lastFingerId && (allowMouseControl || !IsMousePointerId(eventData.get_pointerId())))
			{
				Restart();
			}
		}

		void IDragHandler.OnDrag(PointerEventData eventData)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			if (hasFinger && eventData.get_pointerId() == lastFingerId)
			{
				Vector2 position = eventData.get_position();
				float num = position.x - origWorldPosition.x;
				Vector2 position2 = eventData.get_position();
				Vector3 val = default(Vector3);
				val._002Ector(num, position2.y - origWorldPosition.y);
				val = Vector3.ClampMagnitude(val, (float)radius);
				Vector3 val2 = origWorldPosition + val;
				this.get_transform().set_position(val2);
				UpdateValue(val2);
			}
		}

		private static bool IsMousePointerId(int id)
		{
			return id == -1 || id == -2 || id == -3;
		}
	}
}
