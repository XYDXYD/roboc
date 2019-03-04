using UnityEngine;

namespace Mothership
{
	internal sealed class WeaponReorderCursor : MonoBehaviour
	{
		public Transform cursorTransform;

		public UISprite cursorSprite;

		private Vector3 _mousePosition = Vector3.get_zero();

		private bool _isActive;

		public WeaponReorderCursor()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)


		public void LateUpdate()
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			if (_isActive)
			{
				ref Vector3 mousePosition = ref _mousePosition;
				Vector3 mousePosition2 = Input.get_mousePosition();
				float num = mousePosition2.x - (float)Screen.get_width() * 0.5f;
				Vector3 mousePosition3 = Input.get_mousePosition();
				float num2 = mousePosition3.y - (float)Screen.get_height() * 0.5f;
				Vector3 localPosition = cursorTransform.get_localPosition();
				mousePosition.Set(num, num2, localPosition.z);
				cursorTransform.get_transform().set_localPosition(_mousePosition);
				Cursor.set_visible(false);
			}
		}

		public void Deactivate()
		{
			ToggleActive(active: false);
		}

		public void ShowSpriteAndActivate(string spriteName)
		{
			cursorSprite.set_spriteName(spriteName);
			ToggleActive(active: true);
		}

		private void ToggleActive(bool active)
		{
			_isActive = active;
			Cursor.set_visible(!active);
			cursorTransform.get_gameObject().SetActive(active);
		}
	}
}
