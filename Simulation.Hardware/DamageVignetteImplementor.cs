using UnityEngine;

namespace Simulation.Hardware
{
	public class DamageVignetteImplementor : MonoBehaviour, IDamageVignetteComponent
	{
		[SerializeField]
		private DamageVignetteIndicator indicatorPrefab;

		DamageVignetteIndicator IDamageVignetteComponent.IndicatorPrefab
		{
			get
			{
				return indicatorPrefab;
			}
		}

		public DamageVignetteImplementor()
			: this()
		{
		}
	}
}
