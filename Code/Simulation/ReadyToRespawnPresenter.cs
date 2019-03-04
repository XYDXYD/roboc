using Svelto.Command;
using Svelto.IoC;

namespace Simulation
{
	internal sealed class ReadyToRespawnPresenter
	{
		private ReadyToRespawnView _view;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		internal void Register(ReadyToRespawnView readyToRespawnView)
		{
			_view = readyToRespawnView;
			spawnDispatcher.OnLocalPlayerReadyToRespawn += AllowPlayerToRespawn;
		}

		internal void AnyKeyDown()
		{
			RequestRespawnPointClientCommand requestRespawnPointClientCommand = commandFactory.Build<RequestRespawnPointClientCommand>();
			requestRespawnPointClientCommand.Execute();
			_view.get_gameObject().SetActive(false);
		}

		private void AllowPlayerToRespawn(int id)
		{
			_view.get_gameObject().SetActive(true);
		}
	}
}
