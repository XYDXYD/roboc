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

internal class MapChoiceFilterPresenter
{
	private IDataSource _dataSource;

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	internal MapChoiceFilterView view
	{
		private get;
		set;
	}

	public void SetView(MapChoiceFilterView view_)
	{
		view = view_;
		Initialise();
	}

	public void Initialise()
	{
		_dataSource = new MapChoicesDataSource(serviceFactory);
		TaskRunner.get_Instance().Run(_dataSource.RefreshData());
	}

	public void OnGUIEvent(CustomGameGUIEvent ev)
	{
		switch (ev.type)
		{
		case CustomGameGUIEvent.Type.RefreshAndUpdateMapChoicesList:
			TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshThenSelect);
			break;
		case CustomGameGUIEvent.Type.LeaderSetMapChoice:
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
		string arg = (string)view.popupList.get_data();
		view.Dispatch(CustomGameGUIEvent.Type.UserSetMapChoice, arg);
	}

	private IEnumerator RefreshThenSelect()
	{
		yield return _dataSource.RefreshData();
		view.popupList.Clear();
		for (int i = 0; i < _dataSource.NumberOfDataItemsAvailable(0); i++)
		{
			MapChoiceDataEntry mapChoiceDataEntry = _dataSource.QueryData<MapChoiceDataEntry>(i, 0);
			string mapNameKey = mapChoiceDataEntry.mapNameKey;
			string mapNameForDisplay = mapChoiceDataEntry.mapNameForDisplay;
			view.popupList.AddItem(mapNameForDisplay, (object)mapNameKey);
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
		CustomGameSessionData data = retrieveTask.result.Data;
		if (data != null)
		{
			string strB = data.Config["MapChoice"];
			for (int i = 0; i < view.popupList.items.Count; i++)
			{
				object obj = view.popupList.itemData[i];
				string text = (string)obj;
				if (text.CompareTo(strB) == 0)
				{
					view.popupListLabel.set_text(view.popupList.items[i]);
				}
			}
		}
		else if (view.popupList.items.Count > 0)
		{
			view.popupListLabel.set_text(view.popupList.items[0]);
		}
	}

	private void SelectLeadersChoice(string leadersChoice)
	{
		int num = 0;
		string mapNameForDisplay;
		while (true)
		{
			if (num < _dataSource.NumberOfDataItemsAvailable(0))
			{
				MapChoiceDataEntry mapChoiceDataEntry = _dataSource.QueryData<MapChoiceDataEntry>(num, 0);
				string mapNameKey = mapChoiceDataEntry.mapNameKey;
				mapNameForDisplay = mapChoiceDataEntry.mapNameForDisplay;
				if (mapNameKey.CompareTo(leadersChoice) == 0)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		view.popupListLabel.set_text(mapNameForDisplay);
	}
}
