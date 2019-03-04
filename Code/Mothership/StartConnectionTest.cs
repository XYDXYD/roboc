using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class StartConnectionTest : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private TestConnectionParameters _parameters;

		[Inject]
		internal TestConnection testConnection
		{
			private get;
			set;
		}

		public void Execute()
		{
			testConnection.StartTest(_parameters.hostIP, _parameters.hostPort, _parameters.networkConfig, _parameters.encryptionParams);
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_parameters = (dependency as TestConnectionParameters);
			return this;
		}
	}
}
