using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierParticleScaler : MonoBehaviour
	{
		public bool scaleTransformOnly;

		private ParticleSystem _particleSystem;

		public AlignmentRectifierParticleScaler()
			: this()
		{
		}

		private void OnEnable()
		{
			if (_particleSystem == null)
			{
				_particleSystem = this.GetComponent<ParticleSystem>();
			}
		}

		public void ScaleSize(float scaleFactor)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localScale = this.get_transform().get_localScale();
			localScale *= scaleFactor;
			this.get_transform().set_localScale(localScale);
			if (!scaleTransformOnly)
			{
				ParticleSystem particleSystem = _particleSystem;
				particleSystem.set_startSize(particleSystem.get_startSize() * scaleFactor);
			}
		}
	}
}
