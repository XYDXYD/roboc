using System;
using System.Collections.Generic;

namespace PlayMaker
{
	internal sealed class PlayMakerScreenControllerRequestsSetup
	{
		private IPlaymakerDataProvider _playMakerDataProvider;

		private Dictionary<Type, Action<IPlayMakerDataRequest>> _requestHandlers = new Dictionary<Type, Action<IPlayMakerDataRequest>>();

		public void Initialise(IPlaymakerDataProvider dataProvider)
		{
			_playMakerDataProvider = dataProvider;
			_playMakerDataProvider.RegisterPlaymakerRequestHandlers(RegisterPlaymakerRequestHandlerToDictionary);
		}

		private void RegisterPlaymakerRequestHandlerToDictionary(Type requestClassType, Action<IPlayMakerDataRequest> actionToInvoke)
		{
			_requestHandlers[requestClassType] = actionToInvoke;
		}

		public void RequestData(IPlayMakerDataRequest dataRequest)
		{
			Type type = dataRequest.GetType();
			if (_requestHandlers.ContainsKey(type))
			{
				_requestHandlers[type](dataRequest);
			}
		}
	}
}
