using Services;
using Services.Analytics;
using Services.Requests.Interfaces;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

internal sealed class SettingsScreen : MonoBehaviour, IChainListener, IInitialize
{
	private struct VideoSettings
	{
		public Resolution resolution;

		public int quality;

		public bool fullScreen;
	}

	private struct ExtraSettings
	{
		public float musicVolume;

		public float afxVolume;

		public float voiceOverVolume;

		public float buildMouseSpeed;

		public float fightMouseSpeed;

		public bool invertY;

		public bool toggleZoom;

		public bool blockFriendInvites;

		public bool acceptPartyInvitesFromFriendsAndClanOnly;

		public int capFrameRateAmount;

		public bool capFrameRateEnabled;

		public bool showCenterOfMass;

		public bool enableCameraShake;

		public bool chatEnabled;

		public bool showEditHints;
	}

	private struct Settings
	{
		public VideoSettings video;

		public ExtraSettings extra;

		public string language;
	}

	[SerializeField]
	private UIToggle fullScreenCheckBox;

	[SerializeField]
	private UIPopupList qualityPopupList;

	[SerializeField]
	private UIPopupList resPopupList;

	[SerializeField]
	private GameObject confirmPanel;

	[SerializeField]
	private UISlider musicVolumeSlider;

	[SerializeField]
	private UILabel musicVolumeValue;

	[SerializeField]
	private UISlider sfxVolumeSlider;

	[SerializeField]
	private UILabel sfxVolumeValue;

	[SerializeField]
	private UISlider voiceOverSlider;

	[SerializeField]
	private UILabel speechVolumeValue;

	[SerializeField]
	private UISlider buildMouseSpeedSlider;

	[SerializeField]
	private UILabel buildMouseSpeedValue;

	[SerializeField]
	private UISlider fightMouseSpeedSlider;

	[SerializeField]
	private UILabel fightMouseSpeedValue;

	[SerializeField]
	private UIToggle invertYCheckBox;

	[SerializeField]
	private UIToggle blockFriendInvitesCheckBox;

	[SerializeField]
	private UIToggle accepPartyInvitesFromFriendsAndClanOnlyCheckBox;

	[SerializeField]
	private UIPopupList capFrameRatePopupList;

	[SerializeField]
	private UIToggle capFrameFrateCheckBox;

	[SerializeField]
	private UIToggle toggleZoomCheckBox;

	[SerializeField]
	private UIToggle showCenterOfMassCheckBox;

	[SerializeField]
	private UIToggle enableCameraShakeCheckBox;

	[SerializeField]
	private UIToggle disableChatCheckBox;

	[SerializeField]
	private GameObject localisationSection;

	[SerializeField]
	private UIToggle showEditHints;

	[SerializeField]
	private GameObject generalTab;

	[SerializeField]
	private GameObject displayTab;

	[SerializeField]
	private GameObject audioTab;

	[SerializeField]
	private GameObject gameplayTab;

	[SerializeField]
	private TabButton generalButton;

	[SerializeField]
	private TabButton displayButton;

	[SerializeField]
	private TabButton audioButton;

	[SerializeField]
	private TabButton gameplayButton;

	private static readonly List<string> _qualitySettings = new List<string>();

	private static readonly List<Resolution> _resolutions = new List<Resolution>();

	private static readonly List<string> _resStrings = new List<string>();

	private const string _modeErrorString = "strError";

	private string qualityValue;

	private string resValue;

	private string frameRateCapValue;

	private bool _isActive;

	private Settings _backupSettings;

	private Settings _newSettings;

	[Inject]
	internal SettingsDisplay settingsDisplay
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	[Inject]
	internal ChatSettings chatSettings
	{
		private get;
		set;
	}

	[Inject]
	internal MasterVolumeController masterVolumeController
	{
		private get;
		set;
	}

	[Inject]
	internal MouseSettings mouseSettings
	{
		private get;
		set;
	}

	[Inject]
	internal CameraSettings cameraSettings
	{
		private get;
		set;
	}

	[Inject]
	internal SocialSettings socialSettings
	{
		private get;
		set;
	}

	[Inject]
	internal LocalisationSettings localisationSettings
	{
		private get;
		set;
	}

	[Inject]
	internal CapFrameRateSettings capFrameRateSettings
	{
		private get;
		set;
	}

	[Inject]
	internal AdvancedRobotEditSettings advancedRobotEditSettings
	{
		private get;
		set;
	}

	[Inject]
	internal IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingIconPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal ICursorMode cursorMode
	{
		private get;
		set;
	}

	[Inject]
	internal IAnalyticsRequestFactory analyticsRequestFactory
	{
		private get;
		set;
	}

	public SettingsScreen()
		: this()
	{
	}

	public void OnDependenciesInjected()
	{
		settingsDisplay.SetScreen(this);
		this.get_gameObject().SetActive(false);
		InitVolumeSliders();
		InitMouseSettings();
		InitSocialSettings();
		socialSettings.OnSettingsLoaded += RefreshSocialSettings;
		InitFrameRateCap();
		InitAdvancedSettings();
		InitValidResolutions();
		InitQualitySettingStrings();
		InitCameraSettings();
		capFrameRateSettings.InitialiseFrameRate();
		SaveButtonColours();
		ShowGeneralTab();
		TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadPlatformConfigurationValues);
	}

	private void OnDestroy()
	{
		socialSettings.OnSettingsLoaded -= RefreshSocialSettings;
	}

	public void Listen(object message)
	{
		if (message is ButtonType)
		{
			switch ((ButtonType)message)
			{
			case ButtonType.SettingsOk:
				OnSettingsOk();
				break;
			case ButtonType.SettingsCancel:
				OnSettingsCancel();
				break;
			case ButtonType.SettingsConfirmOk:
				OnSettingsConfirmOk();
				break;
			case ButtonType.Cancel:
			case ButtonType.SettingsConfirmCancel:
				OnSettingsConfirmCancel();
				break;
			case ButtonType.SettingsGeneral:
				ShowGeneralTab();
				break;
			case ButtonType.SettingsDisplay:
				ShowDisplayTab();
				break;
			case ButtonType.SettingsAudio:
				ShowAudioTab();
				break;
			case ButtonType.SettingsGameplay:
				ShowGameplayTab();
				break;
			}
		}
	}

	public unsafe void Show()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		_isActive = true;
		Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		this.get_gameObject().SetActive(true);
		confirmPanel.SetActive(false);
		OnShow();
	}

	public unsafe void Hide()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		_isActive = false;
		this.get_gameObject().SetActive(false);
		Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void OnChangeMusicVolumeSlider()
	{
		musicVolumeValue.set_text(FormatPercentage(musicVolumeSlider.get_value()));
		masterVolumeController.ChangeMasterMusicVolume(musicVolumeSlider.get_value());
	}

	public void OnChangeEffectVolumeSlider()
	{
		sfxVolumeValue.set_text(FormatPercentage(sfxVolumeSlider.get_value()));
		masterVolumeController.ChangeMasterEffectsVolume(sfxVolumeSlider.get_value());
	}

	public void OnChangeVoiceOverVolumeSettings()
	{
		speechVolumeValue.set_text(FormatPercentage(voiceOverSlider.get_value()));
		masterVolumeController.ChangeVoiceOverVolume(voiceOverSlider.get_value());
	}

	public void UpdateBuildMouseSpeedLabel()
	{
		buildMouseSpeedValue.set_text(FormatPercentage(buildMouseSpeedSlider.get_value()));
	}

	public void UpdateFightMouseSpeedLabel()
	{
		fightMouseSpeedValue.set_text(FormatPercentage(fightMouseSpeedSlider.get_value()));
	}

	private unsafe void InitQualitySettingStrings()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		ReloadQualitySettingStrings();
		qualityPopupList.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
	}

	private void ReloadQualitySettingStrings()
	{
		_qualitySettings.Clear();
		_qualitySettings.Add(StringTableBase<StringTable>.Instance.GetString("strFastest"));
		_qualitySettings.Add(StringTableBase<StringTable>.Instance.GetString("strFast"));
		_qualitySettings.Add(StringTableBase<StringTable>.Instance.GetString("strNormal"));
		_qualitySettings.Add(StringTableBase<StringTable>.Instance.GetString("strGood"));
		_qualitySettings.Add(StringTableBase<StringTable>.Instance.GetString("strBeautiful"));
		_qualitySettings.Add(StringTableBase<StringTable>.Instance.GetString("strFantastic"));
		qualityPopupList.items = _qualitySettings;
		qualityPopupList.set_value(_qualitySettings[_backupSettings.video.quality]);
	}

	private int GetQualitySettingIndex(string settingAsString)
	{
		int num = 0;
		foreach (string qualitySetting in _qualitySettings)
		{
			if (qualitySetting == settingAsString)
			{
				return num;
			}
			num++;
		}
		throw new Exception("quality string did not match any known values. Was " + settingAsString);
	}

	private unsafe void InitValidResolutions()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		Resolution[] resolutions = Screen.get_resolutions();
		_resStrings.Clear();
		_resolutions.Clear();
		List<Vector2> list = new List<Vector2>();
		Resolution[] array = resolutions;
		for (int i = 0; i < array.Length; i++)
		{
			Resolution item = array[i];
			if (item.get_width() >= 1024 && item.get_width() <= 3840 && item.get_height() >= 720 && item.get_height() <= 2160)
			{
				Vector2 item2 = default(Vector2);
				item2._002Ector((float)item.get_width(), (float)item.get_height());
				if (!list.Contains(item2))
				{
					_resolutions.Add(item);
					list.Add(item2);
					_resStrings.Add(item.get_width().ToString() + "x" + item.get_height().ToString());
				}
			}
		}
		if (_resStrings.Count == 0)
		{
			_resStrings.Add(StringTableBase<StringTable>.Instance.GetString("strError"));
			_resolutions.Add(Screen.get_currentResolution());
			Console.Log("Unity reported no valid monitor resolutions supported");
		}
		resPopupList.items = _resStrings;
		resPopupList.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
	}

	private void InitVolumeSliders()
	{
		musicVolumeSlider.set_value(masterVolumeController.GetMusicVolume());
		sfxVolumeSlider.set_value(masterVolumeController.GetAFXVolume());
		voiceOverSlider.set_value(masterVolumeController.GetVoiceOverVolume());
	}

	private void InitMouseSettings()
	{
		buildMouseSpeedSlider.set_value(mouseSettings.GetBuildSpeed());
		fightMouseSpeedSlider.set_value(mouseSettings.GetFightSpeed());
		invertYCheckBox.set_value(mouseSettings.IsInvertY());
		toggleZoomCheckBox.set_value(mouseSettings.IsToggleZoom());
	}

	private void InitSocialSettings()
	{
		RefreshSocialSettings();
	}

	private void RefreshSocialSettings()
	{
		blockFriendInvitesCheckBox.GetComponent<UIButton>().set_isEnabled(socialSettings.SettingsLoaded);
		accepPartyInvitesFromFriendsAndClanOnlyCheckBox.GetComponent<UIButton>().set_isEnabled(socialSettings.SettingsLoaded);
		blockFriendInvitesCheckBox.set_value(socialSettings.IsBlockFriendInvites());
		accepPartyInvitesFromFriendsAndClanOnlyCheckBox.set_value(socialSettings.GetAcceptPartyInvitesFromFriendsAndClanOnlySetting());
	}

	private unsafe void InitFrameRateCap()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		capFrameRatePopupList.items.Clear();
		capFrameRatePopupList.items.Add("60");
		capFrameRatePopupList.items.Add("120");
		capFrameRatePopupList.items.Add("240");
		capFrameRateSettings.InitialiseFrameRate();
		capFrameRatePopupList.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		frameRateCapValue = capFrameRateSettings.cappedFrameRateAmount.ToString();
	}

	private int GetResolutionIndex(int width, int height)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		string text = "Resolutions:";
		for (int i = 0; i < _resolutions.Count; i++)
		{
			Resolution val = _resolutions[i];
			if (width == val.get_width() && height == val.get_height())
			{
				return i;
			}
			string text2 = text;
			text = text2 + val.get_width() + ", " + val.get_height() + " - ";
		}
		Console.Log($"Current Resolution values:{width}x{height}.");
		Console.Log(text);
		return 0;
	}

	private int GetResolutionIndex(string resString)
	{
		string text = "Resolutions:";
		for (int i = 0; i < _resStrings.Count; i++)
		{
			if (_resStrings[i] == resString)
			{
				return i;
			}
			text = text + _resStrings[i] + ",";
		}
		Console.Log($"Current Resolution:{resString}.");
		Console.Log(text);
		return -1;
	}

	private void InitAdvancedSettings()
	{
		showCenterOfMassCheckBox.set_value(advancedRobotEditSettings.centerOfMass);
	}

	private void InitCameraSettings()
	{
		enableCameraShakeCheckBox.set_value(cameraSettings.IsCameraShakeEnabled());
	}

	private void OnShow()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		_backupSettings.video.resolution = Screen.get_currentResolution();
		_backupSettings.video.quality = QualitySettings.GetQualityLevel();
		_backupSettings.video.fullScreen = Screen.get_fullScreen();
		_backupSettings.extra.chatEnabled = chatSettings.IsChatEnabled();
		_backupSettings.extra.musicVolume = masterVolumeController.GetMusicVolume();
		_backupSettings.extra.afxVolume = masterVolumeController.GetAFXVolume();
		_backupSettings.extra.voiceOverVolume = masterVolumeController.GetVoiceOverVolume();
		_backupSettings.extra.buildMouseSpeed = mouseSettings.GetBuildSpeed();
		_backupSettings.extra.fightMouseSpeed = mouseSettings.GetFightSpeed();
		_backupSettings.extra.toggleZoom = mouseSettings.IsToggleZoom();
		_backupSettings.extra.invertY = mouseSettings.IsInvertY();
		_backupSettings.extra.enableCameraShake = cameraSettings.IsCameraShakeEnabled();
		_backupSettings.extra.acceptPartyInvitesFromFriendsAndClanOnly = socialSettings.GetAcceptPartyInvitesFromFriendsAndClanOnlySetting();
		_backupSettings.extra.blockFriendInvites = socialSettings.IsBlockFriendInvites();
		_backupSettings.extra.capFrameRateAmount = capFrameRateSettings.cappedFrameRateAmount;
		_backupSettings.extra.capFrameRateEnabled = capFrameRateSettings.capEnabled;
		_backupSettings.extra.showCenterOfMass = advancedRobotEditSettings.centerOfMass;
		_backupSettings.extra.showEditHints = advancedRobotEditSettings.showHints;
		_backupSettings.language = localisationSettings.GetCurrentLanguage();
		if (!Screen.get_fullScreen())
		{
			_backupSettings.video.resolution.set_width(Screen.get_width());
			_backupSettings.video.resolution.set_height(Screen.get_height());
		}
		int resolutionIndex = GetResolutionIndex(_backupSettings.video.resolution.get_width(), _backupSettings.video.resolution.get_height());
		resPopupList.set_value(_resStrings[resolutionIndex]);
		qualityPopupList.set_value(_qualitySettings[_backupSettings.video.quality]);
		fullScreenCheckBox.set_value(_backupSettings.video.fullScreen);
		musicVolumeSlider.set_value(_backupSettings.extra.musicVolume);
		sfxVolumeSlider.set_value(_backupSettings.extra.afxVolume);
		voiceOverSlider.set_value(_backupSettings.extra.voiceOverVolume);
		buildMouseSpeedSlider.set_value(_backupSettings.extra.buildMouseSpeed);
		fightMouseSpeedSlider.set_value(_backupSettings.extra.fightMouseSpeed);
		invertYCheckBox.set_value(_backupSettings.extra.invertY);
		toggleZoomCheckBox.set_value(_backupSettings.extra.toggleZoom);
		blockFriendInvitesCheckBox.set_value(_backupSettings.extra.blockFriendInvites);
		accepPartyInvitesFromFriendsAndClanOnlyCheckBox.set_value(_backupSettings.extra.acceptPartyInvitesFromFriendsAndClanOnly);
		capFrameRatePopupList.set_value(_backupSettings.extra.capFrameRateAmount.ToString());
		capFrameFrateCheckBox.set_value(_backupSettings.extra.capFrameRateEnabled);
		showCenterOfMassCheckBox.set_value(_backupSettings.extra.showCenterOfMass);
		enableCameraShakeCheckBox.set_value(_backupSettings.extra.enableCameraShake);
		disableChatCheckBox.set_value(!_backupSettings.extra.chatEnabled);
		showEditHints.set_value(_backupSettings.extra.showEditHints);
	}

	private void OnSettingsOk()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		int resolutionIndex = GetResolutionIndex(resValue);
		_newSettings.video.resolution = _resolutions[resolutionIndex];
		_newSettings.video.quality = GetQualitySettingIndex(qualityValue);
		_newSettings.video.fullScreen = fullScreenCheckBox.get_value();
		_newSettings.extra.musicVolume = musicVolumeSlider.get_value();
		_newSettings.extra.afxVolume = sfxVolumeSlider.get_value();
		_newSettings.extra.voiceOverVolume = voiceOverSlider.get_value();
		_newSettings.extra.buildMouseSpeed = buildMouseSpeedSlider.get_value();
		_newSettings.extra.fightMouseSpeed = fightMouseSpeedSlider.get_value();
		_newSettings.extra.invertY = invertYCheckBox.get_value();
		_newSettings.extra.toggleZoom = toggleZoomCheckBox.get_value();
		_newSettings.extra.blockFriendInvites = blockFriendInvitesCheckBox.get_value();
		_newSettings.extra.acceptPartyInvitesFromFriendsAndClanOnly = accepPartyInvitesFromFriendsAndClanOnlyCheckBox.get_value();
		_newSettings.extra.capFrameRateAmount = int.Parse(frameRateCapValue);
		_newSettings.extra.capFrameRateEnabled = capFrameFrateCheckBox.get_value();
		_newSettings.extra.showCenterOfMass = showCenterOfMassCheckBox.get_value();
		_newSettings.extra.enableCameraShake = enableCameraShakeCheckBox.get_value();
		_newSettings.extra.chatEnabled = !disableChatCheckBox.get_value();
		_newSettings.extra.showEditHints = showEditHints.get_value();
		bool flag = false;
		if (resValue != StringTableBase<StringTable>.Instance.GetString("strError") && (_newSettings.video.resolution.get_width() != _backupSettings.video.resolution.get_width() || _newSettings.video.resolution.get_height() != _backupSettings.video.resolution.get_height() || _newSettings.video.quality != _backupSettings.video.quality || _newSettings.video.fullScreen != _backupSettings.video.fullScreen))
		{
			flag = true;
		}
		if (flag)
		{
			ApplyVideoSettings(_newSettings.video);
			confirmPanel.SetActive(true);
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		}
		else
		{
			ApplyExtraSettings(_newSettings.extra);
			guiInputController.CloseCurrentScreen();
		}
		TaskRunner.get_Instance().Run(LoadAndRefreshBrawlStrings());
		if (_newSettings.extra.capFrameRateEnabled != capFrameRateSettings.capEnabled || _newSettings.extra.capFrameRateAmount != capFrameRateSettings.cappedFrameRateAmount)
		{
			capFrameRateSettings.ChangeSetting(_newSettings.extra.capFrameRateEnabled, _newSettings.extra.capFrameRateAmount);
		}
	}

	private IEnumerator LoadAndRefreshBrawlStrings()
	{
		yield return BrawlOverridePreloader.LoadBrawlLanguageStringOverrides(serviceFactory, loadingIconPresenter);
		Localization.onLocalize.Invoke();
	}

	private void OnSettingsCancel()
	{
		masterVolumeController.ChangeMasterVolumes(_backupSettings.extra.musicVolume, _backupSettings.extra.afxVolume, _backupSettings.extra.voiceOverVolume);
		localisationSettings.SetLanguage(_backupSettings.language);
		guiInputController.CloseCurrentScreen();
	}

	private void OnSettingsConfirmOk()
	{
		ApplyExtraSettings(_newSettings.extra);
		TaskRunner.get_Instance().Run((Func<IEnumerator>)DelayedShow);
	}

	private void OnSettingsConfirmCancel()
	{
		ApplyVideoSettings(_backupSettings.video);
		TaskRunner.get_Instance().Run((Func<IEnumerator>)DelayedShow);
	}

	private IEnumerator DelayedShow()
	{
		yield return (object)new WaitForSecondsEnumerator(0.1f);
		Show();
		guiInputController.SetShortCutMode(settingsDisplay.shortCutMode);
	}

	private void ApplyVideoSettings(VideoSettings video)
	{
		QualitySettings.SetQualityLevel(video.quality);
		Console.Log("setting resolution: " + video.resolution.get_width() + " , " + video.resolution.get_height() + " , " + video.fullScreen);
		Screen.SetResolution(video.resolution.get_width(), video.resolution.get_height(), video.fullScreen);
		cursorMode.Refresh();
	}

	private void ApplyExtraSettings(ExtraSettings extra)
	{
		chatSettings.SetOptions(extra.chatEnabled);
		masterVolumeController.SaveMasterVolumes(extra.musicVolume, extra.afxVolume, extra.voiceOverVolume);
		mouseSettings.ChangeSettings(extra.buildMouseSpeed, extra.fightMouseSpeed, extra.invertY, extra.toggleZoom);
		mouseSettings.SaveMouseSettings();
		socialSettings.ChangeSettings(extra.acceptPartyInvitesFromFriendsAndClanOnly, extra.blockFriendInvites);
		socialSettings.SaveSettings();
		advancedRobotEditSettings.UpdateSettings(extra.showCenterOfMass, extra.showEditHints);
		cameraSettings.ChangeSettings(extra.enableCameraShake);
		cameraSettings.SaveCameraSettings();
		float? musicVolume_ = (_backupSettings.extra.musicVolume != extra.musicVolume) ? new float?(extra.musicVolume) : null;
		float? sfxVolume_ = (_backupSettings.extra.afxVolume != extra.afxVolume) ? new float?(extra.afxVolume) : null;
		float? speechVolume_ = (_backupSettings.extra.voiceOverVolume != extra.voiceOverVolume) ? new float?(extra.voiceOverVolume) : null;
		float? buildMouseSpeed_ = (_backupSettings.extra.buildMouseSpeed != extra.buildMouseSpeed) ? new float?(extra.buildMouseSpeed) : null;
		float? fightMouseSpeed_ = (_backupSettings.extra.fightMouseSpeed != extra.fightMouseSpeed) ? new float?(extra.fightMouseSpeed) : null;
		string text = (!(_backupSettings.language == localisationSettings.GetCurrentLanguage())) ? StringTableBase<StringTable>.Instance.GetLanguageKeyFromString(localisationSettings.GetCurrentLanguage()) : null;
		bool? buildHintsEnabled_ = (_backupSettings.extra.showEditHints != extra.showEditHints) ? new bool?(extra.showEditHints) : null;
		if (musicVolume_.HasValue || sfxVolume_.HasValue || speechVolume_.HasValue || buildMouseSpeed_.HasValue || fightMouseSpeed_.HasValue || text != null || buildHintsEnabled_.HasValue)
		{
			LogSettingsChangedDependency logSettingsChangedDependency = new LogSettingsChangedDependency(musicVolume_, sfxVolume_, speechVolume_, buildMouseSpeed_, fightMouseSpeed_, text, buildHintsEnabled_);
			TaskRunner.get_Instance().Run(HandleAnalytics(logSettingsChangedDependency));
		}
	}

	private IEnumerator HandleAnalytics(LogSettingsChangedDependency logSettingsChangedDependency)
	{
		TaskService logSettingsChangedRequest = analyticsRequestFactory.Create<ILogSettingsChangedRequest, LogSettingsChangedDependency>(logSettingsChangedDependency).AsTask();
		yield return logSettingsChangedRequest;
		if (!logSettingsChangedRequest.succeeded)
		{
			throw new Exception("Log Settings Changed Request failed", logSettingsChangedRequest.behaviour.exceptionThrown);
		}
	}

	private IEnumerator LoadPlatformConfigurationValues()
	{
		ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> task = request.AsTask();
		yield return new HandleTaskServiceWithError(task, delegate
		{
			Console.Log("error occured in Settings Screen");
		}, delegate
		{
			Console.Log("error occured in Settings Screen");
		}).GetEnumerator();
		if (task.succeeded)
		{
			localisationSection.SetActive(task.result.LanguageSelectionAvailable);
		}
		else
		{
			OnLoadingFailed(task.behaviour);
		}
	}

	private void OnLoadingFailed(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}

	private void ShowGeneralTab()
	{
		ResetTabs();
		generalButton.HighlightButton();
		generalTab.SetActive(true);
	}

	private void ShowDisplayTab()
	{
		ResetTabs();
		displayButton.HighlightButton();
		displayTab.SetActive(true);
	}

	private void ShowAudioTab()
	{
		ResetTabs();
		audioButton.HighlightButton();
		audioTab.SetActive(true);
	}

	private void ShowGameplayTab()
	{
		ResetTabs();
		gameplayButton.HighlightButton();
		gameplayTab.SetActive(true);
	}

	private static string FormatPercentage(float value)
	{
		return Convert.ToString((int)(value * 100f));
	}

	private void SaveButtonColours()
	{
		generalButton.InitColours();
		displayButton.InitColours();
		audioButton.InitColours();
		gameplayButton.InitColours();
	}

	private void ResetTabs()
	{
		generalButton.ResetColours();
		displayButton.ResetColours();
		audioButton.ResetColours();
		gameplayButton.ResetColours();
		generalTab.SetActive(false);
		displayTab.SetActive(false);
		audioTab.SetActive(false);
		gameplayTab.SetActive(false);
	}
}
