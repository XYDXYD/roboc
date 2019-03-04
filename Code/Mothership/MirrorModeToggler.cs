using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class MirrorModeToggler : MonoBehaviour, IInitialize
	{
		[Inject]
		internal MirrorMode mirrorMode
		{
			private get;
			set;
		}

		public MirrorModeToggler()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			mirrorMode.OnMirrorModeChanged += HandleOnMirrorModeChanged;
			HandleOnMirrorModeChanged(enabled: false);
		}

		private void HandleOnMirrorModeChanged(bool enabled)
		{
			this.get_gameObject().SetActive(enabled);
		}
	}
}
