using UnityEngine;

namespace Simulation
{
	internal sealed class BlinkTrailEffectBehaviour : MonoBehaviour
	{
		private TrailRenderer[] _trailRenderers;

		private float _trailTime;

		public BlinkTrailEffectBehaviour()
			: this()
		{
		}

		private void Awake()
		{
			_trailRenderers = this.GetComponentsInChildren<TrailRenderer>();
			_trailTime = _trailRenderers[0].get_time();
		}

		private void OnEnable()
		{
			for (int i = 0; i < _trailRenderers.Length; i++)
			{
				_trailRenderers[i].set_enabled(true);
				_trailRenderers[i].set_time(_trailTime);
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < _trailRenderers.Length; i++)
			{
				_trailRenderers[i].set_time(-1f);
				_trailRenderers[i].set_enabled(false);
			}
		}
	}
}
