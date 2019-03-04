using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using Utility;

namespace Mothership
{
	internal sealed class GameServerValidationFailedClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private StringCodeDependency _dependency;

		[Inject]
		public LobbyPresenter _lobbyPresenter
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputController _guiInputController
		{
			private get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (StringCodeDependency)dependency;
			return this;
		}

		public void Execute()
		{
			Console.LogWarning("Received warning from server: " + StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.TITLE_STRING_SUFFIX) + ": " + StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.BODY_STRING_SUFFIX));
			string headerText;
			string bodyText;
			if (_dependency.errorCode == GameServerErrorCodes.STR_ERR_CUSTOM_STRING)
			{
				headerText = Localization.Get("strWarningTitle", true);
				bodyText = _dependency.errorMessage;
			}
			else
			{
				headerText = StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.TITLE_STRING_SUFFIX);
				bodyText = StringTableBase<StringTable>.Instance.GetString(_dependency.errorCode.ToString() + GameServerErrorStringKeys.BODY_STRING_SUFFIX);
			}
			GenericErrorData error = new GenericErrorData(headerText, bodyText, StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
			});
			_guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			ErrorWindow.ShowErrorWindow(error);
			_lobbyPresenter.ReceivedGameServerWarning();
		}
	}
}
