using Svelto.Command;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class UICommandExecuteListener : MonoBehaviour, IChainListener
	{
		public enum CommandsToExecute
		{
			SwitchToMainMenuOrCustomGameScreenCommand
		}

		[SerializeField]
		private CommandsToExecute CommandToExecute;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public UICommandExecuteListener()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				if (buttonType == ButtonType.ExecuteCommandOnClick && CommandToExecute == CommandsToExecute.SwitchToMainMenuOrCustomGameScreenCommand)
				{
					commandFactory.Build<SwitchToMainMenuCommand>().Execute();
				}
			}
		}
	}
}
