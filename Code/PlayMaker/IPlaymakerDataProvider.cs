using System;

namespace PlayMaker
{
	internal interface IPlaymakerDataProvider
	{
		void RegisterPlaymakerRequestHandlers(Action<Type, Action<IPlayMakerDataRequest>> RegisterPlayMakerRequestHandler);
	}
}
