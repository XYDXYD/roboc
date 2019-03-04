using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class ReceiveTestDataClientCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal TestConnection testConnection
		{
			private get;
			set;
		}

		public void Execute()
		{
			testConnection.MessageReceived();
		}
	}
}
