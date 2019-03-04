using Svelto.IoC;
using UnityEngine;

namespace Simulation.BattleArena.GUI
{
	internal class HUDCaptureProgressView : MonoBehaviour, IInitialize
	{
		public HUDBattleArenaWidget view;

		[Inject]
		internal PlayerCapureStatePresenter presenter
		{
			private get;
			set;
		}

		public HUDCaptureProgressView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			if (presenter != null)
			{
				presenter.RegisterView(view);
			}
		}
	}
}
