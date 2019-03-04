using ServerStateServiceLayer;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Runtime.CompilerServices;

internal sealed class DevMessagePresenter : IInitialize, IWaitForFrameworkDestruction
{
	private static string lastMessageReceived = string.Empty;

	private IServiceEventContainer _serverStateEventContainer;

	[CompilerGenerated]
	private static Action<ServiceBehaviour> _003C_003Ef__mg_0024cache0;

	[Inject]
	internal IServerStateEventContainerFactory serverStateEventContainerFactory
	{
		private get;
		set;
	}

	[Inject]
	internal IServerStateRequestFactory serverStateRequestFactory
	{
		private get;
		set;
	}

	public DevMessageView view
	{
		private get;
		set;
	}

	void IInitialize.OnDependenciesInjected()
	{
		_serverStateEventContainer = serverStateEventContainerFactory.Create();
		_serverStateEventContainer.ListenTo<IDevMessageEventListener, DevMessage>(OnDevMessageReceived);
		serverStateRequestFactory.Create<IGetDevMessageRequest>().SetAnswer(new ServiceAnswer<DevMessage>(OnDevMessageReceived, ErrorWindow.ShowServiceErrorWindow)).Execute();
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		if (_serverStateEventContainer != null)
		{
			_serverStateEventContainer.Dispose();
			_serverStateEventContainer = null;
		}
	}

	private void OnDevMessageReceived(DevMessage devMessage)
	{
		if (view != null && devMessage.Text != lastMessageReceived && devMessage.Text.Length > 0)
		{
			view.SetMessage(devMessage.Text, devMessage.DisplayTime);
			lastMessageReceived = devMessage.Text;
		}
	}
}
