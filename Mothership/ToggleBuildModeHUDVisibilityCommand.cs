using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class ToggleBuildModeHUDVisibilityCommand : IInjectableCommand<bool>, ICommand
	{
		private bool _dependancy;

		[Inject]
		internal HUDHiderMothershipPresenter presenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			presenter.HandleSetVisibility(_dependancy);
		}

		public ICommand Inject(bool dependancy)
		{
			_dependancy = dependancy;
			return this;
		}
	}
}
