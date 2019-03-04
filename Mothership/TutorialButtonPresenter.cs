using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class TutorialButtonPresenter
	{
		[Inject]
		private ICommandFactory commandFactory
		{
			get;
			set;
		}

		public void ButtonPressed()
		{
			commandFactory.Build<StartTutorialFromMothershipCommand>().Execute();
		}
	}
}
