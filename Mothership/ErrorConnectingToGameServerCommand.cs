using Svelto.Command;
using Svelto.IoC;

namespace Mothership
{
	internal class ErrorConnectingToGameServerCommand : ICommand
	{
		[Inject]
		internal NetworkEventRegistrationMothership networkEventRegistrationMothership
		{
			private get;
			set;
		}

		public void Execute()
		{
			networkEventRegistrationMothership.Stop();
		}
	}
}
