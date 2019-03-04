using Simulation;
using Svelto.Context;
using Svelto.IoC;
using System;

internal sealed class GameTimePresenter : IInitialize, IHudElement, IWaitForFrameworkDestruction
{
	private GameTimer _view;

	private bool _setVisible;

	[Inject]
	internal LobbyGameStartPresenter lobbyPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal MultiplayerGameTimerClient multiplayerGameTimer
	{
		private get;
		set;
	}

	[Inject]
	internal IHudStyleController battleHudStyleController
	{
		private get;
		set;
	}

	void IInitialize.OnDependenciesInjected()
	{
		LobbyGameStartPresenter lobbyPresenter = this.lobbyPresenter;
		lobbyPresenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyPresenter.OnInitialLobbyGuiClose, new Action(OnBattleLobbyClose));
		multiplayerGameTimer.OnTimeUpdated += OnTimerUpdated;
	}

	public void SetView(GameTimer view)
	{
		_view = view;
		battleHudStyleController.AddHud(this);
	}

	private void OnBattleLobbyClose()
	{
		if (WorldSwitching.GetGameModeType() != GameModeType.Pit && !_setVisible && _view.get_gameObject() != null)
		{
			_view.get_gameObject().SetActive(true);
			_setVisible = true;
		}
	}

	public void OnTimerUpdated(float time)
	{
		if (_view != null)
		{
			_view.SetTime(time);
		}
	}

	public void SetStyle(HudStyle style)
	{
		if (style == HudStyle.HideAllButChat)
		{
			_view.Hide();
		}
	}

	public void OnFrameworkDestroyed()
	{
		battleHudStyleController.RemoveHud(this);
	}
}
