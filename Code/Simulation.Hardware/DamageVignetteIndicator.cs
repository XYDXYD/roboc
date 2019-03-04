using UnityEngine;

namespace Simulation.Hardware
{
	public class DamageVignetteIndicator : MonoBehaviour
	{
		[SerializeField]
		private Transform indicator;

		[SerializeField]
		private Transform pivot;

		public Transform Indicator => indicator;

		public Transform Pivot => pivot;

		public DamageVignetteIndicator()
			: this()
		{
		}

		public void Enable(Transform parent)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_parent(parent);
			this.get_transform().set_localScale(Vector3.get_one());
		}

		public void Disable()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
