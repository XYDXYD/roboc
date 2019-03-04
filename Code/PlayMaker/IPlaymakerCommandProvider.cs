using System;

namespace PlayMaker
{
	internal interface IPlaymakerCommandProvider
	{
		void RegisterPlaymakerCommandHandlers(Action<Action<IPlaymakerCommandInputParameters>, Type> RegistrationCallback);
	}
}
