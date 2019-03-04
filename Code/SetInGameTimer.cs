using Svelto.IoC;
using Svelto.ServiceLayer;

internal sealed class SetInGameTimer
{
	private bool _postedTime;

	private const float LOBBY_GAME_TIME = 200f;

	private const float TIME_SCALE = 0.5f;

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal GaragePresenter garage
	{
		private get;
		set;
	}

	public void SetGameTimer(float secondsRemaining)
	{
		if (!_postedTime)
		{
			SendRequest(secondsRemaining * 0.5f);
			_postedTime = true;
		}
	}

	public void SetGameFinished()
	{
		SendRequest(0f);
	}

	private void SendRequest(float secondsRemaining)
	{
		ISetRobotInGameRequest setRobotInGameRequest = serviceFactory.Create<ISetRobotInGameRequest, float>(secondsRemaining);
		setRobotInGameRequest.SetAnswer(new ServiceAnswer<bool>(OnSetSuccess, OnSetFail));
		setRobotInGameRequest.Execute();
	}

	private void OnSetSuccess(bool success)
	{
	}

	private void OnSetFail(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}
}
