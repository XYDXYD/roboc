using Avatars;
using Services.Analytics;
using Simulation;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class AvatarSelectionPresenter : IInitialize, IWaitForFrameworkDestruction, IGUIDisplay, IComponent
	{
		private const float InputDelayDuration = 0.25f;

		private bool _premiumPurchasedState;

		private bool _customAvatarsAvailableByStyle;

		private bool _loadPlayerAvatarInfo;

		private AvatarInfo _localAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);

		private CustomAvatarInfo _newCustomAvatarInfo;

		private FileBrowser _fileBrowser;

		private PresetAvatarMap _avatarMap;

		private AvatarSelectionView _view;

		private Action<ShowAvatarSelectionScreenCommandCallbackParameters> _onSaveExitCallback;

		private Action<IEnumerator> _fetchLocalAvatarEnumerator;

		[Inject]
		internal IServiceRequestFactory RequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputController GuiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembershipActivatedMediator PremiumMembershipActivatedMediator
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter LoadingIcon
		{
			private get;
			set;
		}

		[Inject]
		internal PresetAvatarMapProvider PresetAvatarMapProvider
		{
			private get;
			set;
		}

		[Inject]
		internal IMultiAvatarLoader AvatarLoader
		{
			private get;
			set;
		}

		[Inject]
		internal AvatarAvailableObserver AvatarAvailableObserver
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
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

		public GuiScreens screenType => GuiScreens.AvatarSelection;

		public TopBarStyle topBarStyle => TopBarStyle.SameAsPrevious;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public HudStyle battleHudStyle => HudStyle.Full;

		public bool isScreenBlurred => false;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		unsafe void IInitialize.OnDependenciesInjected()
		{
			_avatarMap = PresetAvatarMapProvider.GetAvatarMap();
			PremiumMembershipActivatedMediator.Register(PremiumPurchased);
			AvatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void ConfigureStyle(string AvatarSelectionScreenTitle, bool LoadPlayerAvatarInfo, Action<ShowAvatarSelectionScreenCommandCallbackParameters> OnExitCallback, bool customAvatarCannotBeSelected)
		{
			_loadPlayerAvatarInfo = LoadPlayerAvatarInfo;
			_onSaveExitCallback = OnExitCallback;
			_customAvatarsAvailableByStyle = !customAvatarCannotBeSelected;
			_view.SetTitle(AvatarSelectionScreenTitle);
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarData)
		{
			if (avatarData.avatarName == AvatarUtils.LocalPlayerAvatarName && avatarData.avatarType == AvatarType.PlayerAvatar)
			{
				_view.NewLocalPlayerTextureAvailable(avatarData.texture);
			}
		}

		private void ShowCorrectSectionsAndRebuildButtons()
		{
			if (_customAvatarsAvailableByStyle)
			{
				_view.SetRegionVisibility(avatarButtonsRegionVisible: true, uploadButtonSectionVisible: false, bottomOKandCancelButtonsVisible: true);
			}
			else
			{
				_view.SetRegionVisibility(avatarButtonsRegionVisible: true, uploadButtonSectionVisible: false, bottomOKandCancelButtonsVisible: true);
			}
			RebuildButtonsList();
		}

		private void RebuildButtonsList()
		{
			Console.Log("rebuilding buttons...");
			_view.EraseAllBuiltButtons();
			int num = 0;
			PresetAvatarMap.PresetAvatar[] avatars = _avatarMap.Avatars;
			foreach (PresetAvatarMap.PresetAvatar presetAvatar in avatars)
			{
				_view.AddNewDefaultButton(presetAvatar.Texture, num);
				num++;
			}
			_view.ArrangeButtonsAndFinalise();
			_view.BroadcastMessage(new AvatarSelectionChangedData(_localAvatarInfo));
			_view.SetSelectedStateOfCustomButton(_localAvatarInfo.UseCustomAvatar);
		}

		private IEnumerator LoadPremiumState()
		{
			LoadingIcon.NotifyLoading("AvatarSelection");
			ILoadPremiumDataRequest request = RequestFactory.Create<ILoadPremiumDataRequest>();
			TaskService<PremiumInfoData> task = new TaskService<PremiumInfoData>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				LoadingIcon.NotifyLoading("AvatarSelection");
			}, delegate
			{
				LoadingIcon.NotifyLoadingDone("AvatarSelection");
			}).GetEnumerator();
			LoadingIcon.NotifyLoadingDone("AvatarSelection");
			if (task.succeeded)
			{
				PremiumInfoData result = task.result;
				_premiumPurchasedState = result.HasPremium;
			}
		}

		private IEnumerator LoadAvatarInfo()
		{
			_localAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			if (!_loadPlayerAvatarInfo)
			{
				yield break;
			}
			LoadingIcon.NotifyLoading("AvatarSelection");
			yield return _fetchLocalAvatarEnumerator;
			IGetAvatarInfoRequest request = RequestFactory.Create<IGetAvatarInfoRequest>();
			TaskService<AvatarInfo> task = new TaskService<AvatarInfo>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				LoadingIcon.NotifyLoading("AvatarSelection");
			}, delegate
			{
				LoadingIcon.NotifyLoadingDone("AvatarSelection");
			}).GetEnumerator();
			LoadingIcon.NotifyLoadingDone("AvatarSelection");
			if (task.succeeded)
			{
				_localAvatarInfo = task.result;
				Console.Log("use local player avatar state :" + _localAvatarInfo.UseCustomAvatar);
				if (_localAvatarInfo.UseCustomAvatar)
				{
					AvatarLoader.RequestAvatar(AvatarType.PlayerAvatar, AvatarUtils.LocalPlayerAvatarName);
				}
			}
			else
			{
				_localAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			PremiumMembershipActivatedMediator.Deregister(PremiumPurchased);
			AvatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void RegisterView(AvatarSelectionView avatarSelectionView)
		{
			_view = avatarSelectionView;
			_fileBrowser = avatarSelectionView.GetComponent<FileBrowser>();
		}

		public void ReceiveMessage(object message)
		{
			if (message is SelectDefaultAvatarButtonData)
			{
				int buttonID = (message as SelectDefaultAvatarButtonData).ButtonID;
				_localAvatarInfo.UseCustomAvatar = false;
				_localAvatarInfo.AvatarId = buttonID;
				_view.BroadcastMessage(new AvatarSelectionChangedData(_localAvatarInfo));
				_view.SetSelectedStateOfCustomButton(selected: false);
			}
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.Confirm:
				{
					ShowAvatarSelectionScreenCommandCallbackParameters obj = new ShowAvatarSelectionScreenCommandCallbackParameters(_localAvatarInfo, _newCustomAvatarInfo);
					_onSaveExitCallback(obj);
					GuiInputController.ToggleCurrentScreen();
					break;
				}
				case ButtonType.Cancel:
					GuiInputController.ToggleCurrentScreen();
					break;
				case ButtonType.SelectCustomAvatar:
					_localAvatarInfo.UseCustomAvatar = true;
					_view.SetSelectedStateOfCustomButton(selected: true);
					_view.BroadcastMessage(new AvatarSelectionChangedData(_localAvatarInfo));
					break;
				case ButtonType.PurchasePremium:
					StartPurchasePremium();
					break;
				case ButtonType.Upload:
					OpenLoadFileDialogue();
					break;
				}
			}
		}

		private void SelectedANewCustomAvatarFile(string path)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Expected O, but got Unknown
			GuiInputController.SetShortCutMode(shortCutMode);
			if (!string.IsNullOrEmpty(path))
			{
				byte[] array;
				try
				{
					array = File.ReadAllBytes(path);
				}
				catch (Exception innerException)
				{
					throw new Exception("Failed to load selected file", innerException);
				}
				Texture2D val = new Texture2D(1, 1, 5, false);
				if (!ImageConversion.LoadImage(val, array))
				{
					throw new Exception("Failed to load image data");
				}
				TextureScale.Bilinear(val, 100, 100);
				array = ImageConversion.EncodeToJPG(val);
				if (array.Length > 50000)
				{
					throw new Exception("File too big after resizing");
				}
				ImageConversion.LoadImage(val, array);
				TaskRunner.get_Instance().Run(TryUploadNewCustomAvatarChoice(array, val));
			}
		}

		private IEnumerator TryUploadNewCustomAvatarChoice(byte[] rawData, Texture2D textureData)
		{
			LoadingIcon.NotifyLoading("AvatarSelection");
			yield return (object)new WaitForSecondsEnumerator(0.25f);
			IUploadCustomAvatarRequest request = RequestFactory.Create<IUploadCustomAvatarRequest>();
			request.Inject(rawData);
			TaskService task = new TaskService(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				LoadingIcon.NotifyLoading("AvatarSelection");
			}, delegate
			{
				LoadingIcon.NotifyLoadingDone("AvatarSelection");
			}).GetEnumerator();
			LoadingIcon.NotifyLoadingDone("AvatarSelection");
			if (task.succeeded)
			{
				ISetAvatarInfoRequest setRequest = RequestFactory.Create<ISetAvatarInfoRequest>();
				setRequest.Inject(new AvatarInfo(useCustomAvatar: true, 0));
				LoadingIcon.NotifyLoading("AvatarSelection");
				TaskService setTask = new TaskService(setRequest);
				yield return new HandleTaskServiceWithError(setTask, delegate
				{
					LoadingIcon.NotifyLoading("AvatarSelection");
				}, delegate
				{
					LoadingIcon.NotifyLoadingDone("AvatarSelection");
				}).GetEnumerator();
				LoadingIcon.NotifyLoadingDone("AvatarSelection");
				if (setTask.succeeded)
				{
					_localAvatarInfo.UseCustomAvatar = true;
					_newCustomAvatarInfo = new CustomAvatarInfo(rawData);
					_view.NewLocalPlayerTextureAvailable(textureData);
					_view.BroadcastMessage(new AvatarSelectionChangedData(_localAvatarInfo));
					_view.SetSelectedStateOfCustomButton(selected: true);
					_view.SetCustomButtonState(CustomAvatarButton.AvatarButtonState.AvatarIsSelectable);
				}
			}
		}

		private void StartPurchasePremium()
		{
			PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "AvatarSelection", startsNewChain: true);
			guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
		}

		private unsafe void OpenLoadFileDialogue()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Expected O, but got Unknown
			GuiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			_fileBrowser.ShowBrowser(Localization.Get("strAvatarFileBrowserTitle", true), new FinishedCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleServiceError(ServiceBehaviour serviceBehaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
		}

		private void PremiumPurchased(TimeSpan obj)
		{
			_premiumPurchasedState = true;
			_view.ShowGetPremiumButtonInUploadCustomSection(setting: false);
			RebuildButtonsList();
		}

		public GUIShowResult Show()
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			_newCustomAvatarInfo = null;
			_view.EraseAllBuiltButtons();
			_view.Show();
			SerialTaskCollection val = new SerialTaskCollection();
			val.Add(LoadAvatarInfo());
			val.Add(LoadPremiumState());
			val.add_onComplete((Action)delegate
			{
				ShowCorrectSectionsAndRebuildButtons();
			});
			TaskRunner.get_Instance().Run((IEnumerator)val);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			_view.Hide();
			return true;
		}

		public void EnableBackground(bool enable)
		{
			throw new NotImplementedException();
		}

		public bool IsActive()
		{
			return _view.get_gameObject().get_activeInHierarchy();
		}
	}
}
