using UnityEngine;

namespace Simulation
{
	internal sealed class HUDBattleArenaMarkerWidget : MonoBehaviour
	{
		public float alphaFadeSpeed = 10f;

		public UIPanel panel;

		public Vector3 screenPosOffset = new Vector3(0f, 12f, 0f);

		public Vector3 worldPosOffset = new Vector3(0f, 0.6f, 0f);

		public PlayerMarkerConfig playerMarkerConfig;

		private Transform _transform;

		private Camera _mainCamera;

		private Transform _mainCameraTransform;

		private Vector3 _yOffset = Vector3.get_zero();

		private Vector3 _worldPosition;

		private float _cameraMachineDistance = 1f;

		private bool _infrontOfCamera;

		private float _currentAlpha;

		public HUDBattleArenaMarkerWidget()
			: this()
		{
		}//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)


		private void Start()
		{
			_mainCamera = Camera.get_main();
			_mainCameraTransform = _mainCamera.get_transform();
			_transform = this.get_transform();
		}

		internal void UpdateWidget()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			_cameraMachineDistance = Vector3.Distance(_mainCameraTransform.get_position(), _worldPosition);
			_infrontOfCamera = (Vector3.Dot(_mainCameraTransform.get_forward(), _mainCameraTransform.get_position() - _worldPosition) < 0f);
			_transform.set_localPosition(CalculateHUDPosition());
			UpdateScale();
			UpdateAlpha(_infrontOfCamera);
		}

		internal void InitWidget(Vector3 position, Vector3 size)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_worldPosition = position;
			_yOffset.y = size.y;
		}

		private void UpdateAlpha(bool visible)
		{
			_currentAlpha = ((!visible) ? 0f : Mathf.Clamp01(_currentAlpha + alphaFadeSpeed * Time.get_deltaTime()));
			panel.set_alpha(_currentAlpha);
			panel.set_enabled(_currentAlpha > 0f);
		}

		private void UpdateScale()
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			float num = _cameraMachineDistance - playerMarkerConfig.minDistance;
			float num2 = 1f - Mathf.Clamp01(num / (playerMarkerConfig.maxDistance - playerMarkerConfig.minDistance));
			Vector3 localScale = Vector3.get_one() * (playerMarkerConfig.maxDistanceScale + num2 * (playerMarkerConfig.minDistanceScale - playerMarkerConfig.maxDistanceScale));
			_transform.set_localScale(localScale);
		}

		private Vector3 CalculateHUDPosition()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			return GuiUtil.CalculateHUDPosition(_worldPosition + _yOffset + worldPosOffset, screenPosOffset, this.get_transform(), _infrontOfCamera);
		}
	}
}
