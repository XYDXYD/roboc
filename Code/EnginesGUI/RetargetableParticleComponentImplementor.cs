using UnityEngine;

namespace EnginesGUI
{
	public class RetargetableParticleComponentImplementor : MonoBehaviour, IRetargetableParticleComponent
	{
		private ParticleSystem _particle;

		public RetargetableParticleComponentImplementor()
			: this()
		{
		}

		private void Awake()
		{
			_particle = this.GetComponent<ParticleSystem>();
		}

		public void Retarget(ParticleConfiguration particleConfig)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (_particle.get_transform().get_localScale() != particleConfig.scale)
			{
				_particle.get_transform().set_localScale(particleConfig.scale);
			}
		}
	}
}
