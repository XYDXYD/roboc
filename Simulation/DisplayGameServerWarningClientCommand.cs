using Battle;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using Utility;

namespace Simulation
{
	internal sealed class DisplayGameServerWarningClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private StringCodeDependency _dependency;

		private bool _triggeredSwitch;

		[Inject]
		internal IGUIInputControllerSim guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal BattleParameters battleParameters
		{
			private get;
			set;
		}

		[Inject]
		internal BonusManager bonusManager
		{
			private get;
			set;
		}

		[Inject]
		internal GameStateClient gameStateClient
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as StringCodeDependency);
			return this;
		}

		public void Execute()
		{
			if (_dependency.errorCode == GameServerErrorCodes.STR_ERR_HAX_AFK)
			{
				gameStateClient.ChangeStateToGameEnded(GameStateResult.AFK, _dependency.errorCode);
			}
			else
			{
				gameStateClient.ChangeStateToGameEnded(GameStateResult.DisconnectByGameServer, _dependency.errorCode);
			}
			Console.Log("Received warning from server: " + StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.TITLE_STRING_SUFFIX));
			string @string;
			string string2;
			if (_dependency.errorCode == GameServerErrorCodes.STR_ERR_CUSTOM_STRING)
			{
				@string = StringTableBase<StringTable>.Instance.GetString("strWarningTitle");
				string2 = StringTableBase<StringTable>.Instance.GetString(_dependency.errorMessage);
			}
			else
			{
				@string = StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.TITLE_STRING_SUFFIX);
				string2 = StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.BODY_STRING_SUFFIX);
			}
			GenericErrorData error = new GenericErrorData(@string, string2, StringTableBase<StringTable>.Instance.GetString("strOK"), ReturnToMothership);
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			ErrorWindow.ShowErrorWindow(error);
		}

		private void RemoteLogExpectedPlayersWithWarning()
		{
			OnGameGuidRetrieved(battleParameters.BattleId);
		}

		private void OnGameGuidRetrieved(string gameGuid)
		{
			try
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				IDictionary<string, PlayerDataDependency> expectedPlayersDict = battlePlayers.GetExpectedPlayersDict();
				if (expectedPlayersDict != null)
				{
					int num = 0;
					foreach (string key in expectedPlayersDict.Keys)
					{
						dictionary.Add("expected player" + num++.ToString(), key);
					}
				}
				RemoteLogger.Error("Received warning from server: " + _dependency.errorCode.ToString(), null, null, dictionary);
			}
			catch (Exception e)
			{
				RemoteLogger.Error(e);
			}
		}

		private void ReturnToMothership()
		{
			if (!_triggeredSwitch)
			{
				commandFactory.Build<SwitchToMothershipCommand>().Inject(fastSwitch: true).Execute();
				bonusManager.IgnoreReplyFromGameServer();
				_triggeredSwitch = true;
			}
		}
	}
}
