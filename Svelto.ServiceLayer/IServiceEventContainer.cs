using System;

namespace Svelto.ServiceLayer
{
	internal interface IServiceEventContainer : IDisposable
	{
		bool IsConnected
		{
			get;
		}

		event Action ReconnectedEvent;

		event Action DisconnectedEvent;

		void ListenTo<TListener>(Action callBack, Action<Exception> errorCallback = null) where TListener : IServiceEventListener;

		void ListenTo<TListener, TEventData>(Action<TEventData> callBack, Action<Exception> errorCallback = null) where TListener : IServiceEventListener<TEventData>;

		void ListenTo<TListener, TData1, TData2>(Action<TData1, TData2> callBack, Action<Exception> errorCallback = null) where TListener : IServiceEventListener<TData1, TData2>;

		void Reconnected();

		void Disconnected();
	}
}
