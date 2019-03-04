using Svelto.Context;
using Svelto.IoC;

namespace Mothership
{
	internal class HUDHiderMothershipPresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private HUDHiderMothership _view;

		[Inject]
		internal IGUIInputController inputController
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
		}

		public void SetView(HUDHiderMothership view)
		{
			_view = view;
		}

		public void HandleSetVisibility(bool visible)
		{
			if (visible)
			{
				_view.ToggleVisibility(setting: true);
			}
			else
			{
				_view.ToggleVisibility(setting: false);
			}
		}
	}
}
