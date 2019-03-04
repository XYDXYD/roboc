using UnityEngine;

namespace Simulation
{
	internal sealed class TeleportReadyEffect : MonoBehaviour, IReadyEffect
	{
		[SerializeField]
		internal Animator animator;

		public TeleportReadyEffect()
			: this()
		{
		}

		bool IReadyEffect.GetEffectActive()
		{
			return animator.GetBool("Activate");
		}

		void IReadyEffect.SetEffectActive(bool activate)
		{
			animator.SetBool("Activate", activate);
		}
	}
}
