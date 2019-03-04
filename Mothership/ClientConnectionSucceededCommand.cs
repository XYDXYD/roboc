using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class ClientConnectionSucceededCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal TestConnection testConnection
		{
			private get;
			set;
		}

		public void Execute()
		{
			testConnection.ConnectionResult(_result: true);
		}
	}
}
