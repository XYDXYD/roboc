using System;
using System.Collections.Generic;

namespace PlayMaker
{
	internal sealed class PlayMakerScreenControllerCommandsSetup
	{
		private Dictionary<Type, Action<IPlaymakerCommandInputParameters>> _commandHandlers = new Dictionary<Type, Action<IPlaymakerCommandInputParameters>>();

		private IPlaymakerCommandProvider _playMakerCommandProvider;

		public void Initialise(IPlaymakerCommandProvider commandProvider)
		{
			commandProvider.RegisterPlaymakerCommandHandlers(RegisterPlaymakerCommandHandlerToDictionary);
		}

		public void InvokeCommand(Action OnFinishedCallback, IPlaymakerCommandInputParameters inputParameter)
		{
			Type type = inputParameter.GetType();
			if (_commandHandlers.ContainsKey(type))
			{
				_commandHandlers[type](inputParameter);
			}
			OnFinishedCallback();
		}

		private void RegisterPlaymakerCommandHandlerToDictionary(Action<IPlaymakerCommandInputParameters> actionToInvoke, Type commandTypeKey)
		{
			_commandHandlers[commandTypeKey] = actionToInvoke;
		}
	}
}
