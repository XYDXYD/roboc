using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	[RequireComponent(typeof(Image))]
	public class TouchButtonExample : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		public bool allowMouseControl = true;

		public bool isPressed
		{
			get;
			private set;
		}

		public TouchButtonExample()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Invalid comparison between Unknown and I4
			if ((int)SystemInfo.get_deviceType() == 1)
			{
				allowMouseControl = false;
			}
		}

		private void Restart()
		{
			isPressed = false;
		}

		void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
		{
			if (allowMouseControl || !IsMousePointerId(eventData.get_pointerId()))
			{
				isPressed = true;
			}
		}

		void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
		{
			if (allowMouseControl || !IsMousePointerId(eventData.get_pointerId()))
			{
				isPressed = false;
			}
		}

		private static bool IsMousePointerId(int id)
		{
			return id == -1 || id == -2 || id == -3;
		}
	}
}
