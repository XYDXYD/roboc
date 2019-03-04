using UnityEngine;

namespace Simulation
{
	internal sealed class ShieldModuleReadyEffect : MonoBehaviour, IReadyEffect
	{
		[SerializeField]
		private Animator animation;

		public ShieldModuleReadyEffect()
			: this()
		{
		}

		bool IReadyEffect.GetEffectActive()
		{
			return animation.GetBool("Active");
		}

		void IReadyEffect.SetEffectActive(bool activate)
		{
			animation.SetBool("Active", activate);
			for (int i = 0; i < this.get_transform().get_childCount(); i++)
			{
				this.get_transform().GetChild(i).get_gameObject()
					.SetActive(activate);
			}
		}
	}
}
