using Authentication;
using CustomGames;
using Mothership.GUI.CustomGames;
using Robocraft.GUI;
using Services.Web.Photon;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

internal class GameModeChoiceFilterPresenter
{
	private IDataSource _dataSource;

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal CustomGameGameModeObservable gameModeChangeObservable
	{
		private get;
		set;
	}

	internal GameModeChoiceFilterView view
	{
		private get;
		set;
	}

	public void SetView(GameModeChoiceFilterView view_)
	{
		view = view_;
		Initialise();
	}

	public void Initialise()
	{
		_dataSource = new GameModeChoicesDataSource(serviceFactory);
		TaskRunner.get_Instance().Run(_dataSource.RefreshData());
	}

	public void OnGUIEvent(CustomGameGUIEvent ev)
	{
		switch (ev.type)
		{
		case CustomGameGUIEvent.Type.RefreshAndUpdateGameModeChoicesList:
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshThenSelect);
			break;
		case CustomGameGUIEvent.Type.LeaderSetGameModeChoice:
			SelectLeadersChoice(ev.Data);
			break;
		case CustomGameGUIEvent.Type.LeaderSet:
			HandleLeaderChanged(ev.Data);
			break;
		}
	}

	private void HandleLeaderChanged(string newLeader)
	{
		if (User.Username.CompareTo(newLeader) == 0)
		{
			view.SetDropDownAvailable(setting: true);
		}
		else
		{
			view.SetDropDownAvailable(setting: false);
		}
	}

	internal void OnItemSelected()
	{
		GameModeType gameModeType = (GameModeType)view.popupList.get_data();
		view.Dispatch(CustomGameGUIEvent.Type.UserSetGameModeChoice, gameModeType.ToString());
	}

	private IEnumerator RefreshThenSelect()
	{
		yield return _dataSource.RefreshData();
		view.popupList.Clear();
		for (int i = 0; i < _dataSource.NumberOfDataItemsAvailable(0); i++)
		{
			GameModeType gameModeType = _dataSource.QueryData<GameModeType>(i, 0);
			string text = "Unknown game mode";
			switch (gameModeType)
			{
			case GameModeType.Normal:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameModeStringNormal");
				break;
			case GameModeType.SuddenDeath:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameModeStringSuddenDeath");
				break;
			case GameModeType.Pit:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameModeStringPitMode");
				break;
			case GameModeType.TeamDeathmatch:
				text = StringTableBase<StringTable>.Instance.GetString("strCustomGameModeStringTeamDeathMatch");
				break;
			}
			view.popupList.AddItem(text, (object)gameModeType);
		}
		yield return SelectCorrectChoice();
	}

	private IEnumerator SelectCorrectChoice()
	{
		IRetrieveCustomGameSessionRequest retrieveCustomGameRequest = serviceFactory.Create<IRetrieveCustomGameSessionRequest>();
		retrieveCustomGameRequest.ClearCache();
		TaskService<RetrieveCustomGameSessionRequestData> retrieveTask = new TaskService<RetrieveCustomGameSessionRequestData>(retrieveCustomGameRequest);
		yield return retrieveTask;
		if (!retrieveTask.succeeded)
		{
			yield break;
		}
		if (retrieveTask.result.Data != null)
		{
			CustomGameSessionData data = retrieveTask.result.Data;
			string text = data.Config["GameMode"];
			GameModeType gameModeType = (GameModeType)Enum.Parse(typeof(GameModeType), text);
			gameModeChangeObservable.Dispatch(ref gameModeType);
			for (int i = 0; i < view.popupList.items.Count; i++)
			{
				object obj = view.popupList.itemData[i];
				string strB = ((GameModeType)obj).ToString();
				if (text.CompareTo(strB) == 0)
				{
					view.popupListLabel.set_text(view.popupList.items[i]);
				}
			}
		}
		else
		{
			view.popupListLabel.set_text(view.popupList.items[0]);
		}
	}

	private void SelectLeadersChoice(string leadersChoice)
	{
		Console.Log("Session leader changed game mode to: " + leadersChoice);
		GameModeType gameModeType = (GameModeType)Enum.Parse(typeof(GameModeType), leadersChoice);
		gameModeChangeObservable.Dispatch(ref gameModeType);
		for (int i = 0; i < view.popupList.items.Count; i++)
		{
			object obj = view.popupList.itemData[i];
			string strB = ((GameModeType)obj).ToString();
			if (leadersChoice.CompareTo(strB) == 0)
			{
				view.popupListLabel.set_text(view.popupList.items[i]);
			}
		}
	}
}
