using UnityEngine;

namespace Simulation
{
	internal class MapPingMover : MonoBehaviour
	{
		private Transform _mainCamera;

		private Vector3 _mapPingWorldPosition;

		private Vector3 _scaleFactor;

		private float _cameraDistancePercentage = 0.05f;

		public MapPingMover()
			: this()
		{
		}

		private void Start()
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			_mainCamera = Camera.get_main().get_transform();
			_mapPingWorldPosition = this.get_gameObject().get_transform().get_position();
			_scaleFactor = new Vector3(_cameraDistancePercentage, _cameraDistancePercentage, _cameraDistancePercentage);
			this.get_gameObject().get_transform().set_localScale(_scaleFactor);
		}

		private void Update()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _mainCamera.get_position();
			Vector3 position2 = Vector3.Lerp(position, _mapPingWorldPosition, _cameraDistancePercentage);
			this.get_gameObject().get_transform().set_position(position2);
		}
	}
}
