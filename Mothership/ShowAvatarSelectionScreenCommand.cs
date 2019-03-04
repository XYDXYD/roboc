using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class ShowAvatarSelectionScreenCommand : IInjectableCommand<ShowAvatarSelectionScreenCommandDependancy>, ICommand
	{
		private ShowAvatarSelectionScreenCommandDependancy _dependancy;

		[Inject]
		internal IGUIInputController GuiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarSelectionPresenter SelectionPresenter
		{
			private get;
			set;
		}

		public void Execute()
		{
			SelectionPresenter.ConfigureStyle(_dependancy.AvatarSelectionScreenTitle, _dependancy.LoadLocalPlayerAvatarInfo, _dependancy.OnSelectionCallback, _dependancy.CustomAvatarCannotBeSelected);
			GuiInputController.ShowScreen(GuiScreens.AvatarSelection);
		}

		public ICommand Inject(ShowAvatarSelectionScreenCommandDependancy dependency)
		{
			_dependancy = dependency;
			return this;
		}
	}
}
