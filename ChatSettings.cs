using Services.Requests.Interfaces;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

internal sealed class ChatSettings
{
	private ChatSettingsData _data;

	private bool _allDataLoaded;

	[Inject]
	internal IServiceRequestFactory _serviceFactory
	{
		private get;
		set;
	}

	public bool isLoaded => _allDataLoaded;

	private event Action OnLoaded = delegate
	{
	};

	public IEnumerator Load(IServiceRequestFactory serviceFactory)
	{
		ILoadPlatformConfigurationRequest platformConfigRequest = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> platformConfigTask = platformConfigRequest.AsTask();
		yield return platformConfigTask;
		ILoadChatSettingsRequest loadSettings = serviceFactory.Create<ILoadChatSettingsRequest>();
		TaskService<ChatSettingsData> loadSettingsTask = new TaskService<ChatSettingsData>(loadSettings);
		yield return loadSettingsTask;
		if (loadSettingsTask.succeeded && platformConfigTask.succeeded)
		{
			_data = loadSettingsTask.result;
			OnChatSettingsLoaded();
		}
		else
		{
			ErrorWindow.ShowServiceErrorWindow(loadSettingsTask.behaviour);
			OnChatSettingsLoaded();
		}
	}

	internal void RegisterOnLoaded(Action callback)
	{
		if (_allDataLoaded)
		{
			callback();
		}
		else
		{
			OnLoaded += callback;
		}
	}

	internal void DeregisterOnLoaded(Action callback)
	{
		OnLoaded -= callback;
	}

	internal bool IsChatEnabled()
	{
		return _data.chatEnabled;
	}

	internal void SetOptions(bool enableChat)
	{
		bool flag = false;
		if (enableChat != _data.chatEnabled)
		{
			_data.chatEnabled = enableChat;
			flag = true;
		}
		if (flag)
		{
			TaskRunner.get_Instance().Run(Save());
		}
	}

	private void OnChatSettingsLoaded()
	{
		_allDataLoaded = true;
		this.OnLoaded();
	}

	private IEnumerator Save()
	{
		ISaveChatSettingsRequest saveSettings = _serviceFactory.Create<ISaveChatSettingsRequest, ChatSettingsData>(_data);
		TaskService saveSettingsTask = new TaskService(saveSettings);
		yield return saveSettingsTask;
		if (saveSettingsTask.succeeded)
		{
			Console.Log("chat settings written");
		}
		else
		{
			Console.LogError("ChatSettings.SaveSettings failed!");
		}
	}
}
