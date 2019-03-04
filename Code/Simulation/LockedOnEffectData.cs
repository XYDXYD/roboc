using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal class LockedOnEffectData : MonoBehaviour
	{
		public GameObject lockedOnArrows;

		public int MaximumLockWarnings;

		[Inject]
		internal LockedOnEffectPresenter lockedEffectPresenter
		{
			private get;
			set;
		}

		public LockedOnEffectData()
			: this()
		{
		}

		private void Start()
		{
			lockedEffectPresenter.RegisterData(lockedOnArrows, MaximumLockWarnings);
		}
	}
}
