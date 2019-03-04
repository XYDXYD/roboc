using Mothership;
using Services.Web.Photon;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

public sealed class BuildModeShortcutHintsPresenter : IInitialize, IWaitForFrameworkDestruction
{
	private BuildModeShortcutHintsView _view;

	private BuildModeShortcutHintsObserver _buildModeHintsObserver;

	[Inject]
	internal IGUIInputControllerMothership inputController
	{
		get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	public BuildModeShortcutHintsPresenter(BuildModeShortcutHintsObserver buildModeShortcutHintsObserver)
	{
		_buildModeHintsObserver = buildModeShortcutHintsObserver;
	}

	public void RegisterView(BuildModeShortcutHintsView view)
	{
		_view = view;
	}

	unsafe void IInitialize.OnDependenciesInjected()
	{
		_buildModeHintsObserver.AddAction(new ObserverAction<BuildModeHintEvent>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		inputController.OnScreenStateChange += delegate
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleOnScreenChange);
		};
	}

	unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		_buildModeHintsObserver.RemoveAction(new ObserverAction<BuildModeHintEvent>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private IEnumerator HandleOnScreenChange()
	{
		IRetrieveCustomGameSessionRequest retrieveRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
		TaskService<RetrieveCustomGameSessionRequestData> retrieveTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveRequest);
		yield return retrieveTask;
		if (retrieveTask.succeeded)
		{
			RetrieveCustomGameSessionRequestData result = retrieveTask.result;
			_view.SetStyleForWhenCustomGameShown(result.Response == CustomGameSessionRetrieveResponse.SessionRetrieved);
		}
	}

	private void OnShortcutHintChange(ref BuildModeHintEvent type)
	{
		if (type == BuildModeHintEvent.ShowHints)
		{
			_view.Show();
		}
		if (type == BuildModeHintEvent.HideHints)
		{
			_view.Hide();
		}
	}
}
