using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class GameAbortedClientCommand : IDispatchableCommand, ICommand
	{
		[Inject]
		internal ICommandFactory CommandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal BonusManager BonusManager
		{
			private get;
			set;
		}

		[Inject]
		internal WorldSwitching WorldSwitching
		{
			private get;
			set;
		}

		public void Execute()
		{
			Console.LogWarning("Game aborted because one or more players disconnected/failed to load");
			WorldSwitching.SetAdditionaLoadingScreenMessage(StringTableBase<StringTable>.Instance.GetString("strPlayerDisconnect"));
			CommandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
			BonusManager.IgnoreReplyFromGameServer();
			MultiplayerLoadingScreen multiplayerLoadingScreen = Object.FindObjectOfType<MultiplayerLoadingScreen>();
			if (multiplayerLoadingScreen != null)
			{
				Object.Destroy(multiplayerLoadingScreen.get_transform().get_root().get_gameObject());
			}
		}
	}
}
