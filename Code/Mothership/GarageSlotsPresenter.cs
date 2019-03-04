using Mothership.Garage.Thumbnail;
using Services.Requests.Interfaces;
using Svelto.DataStructures;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Mothership
{
	internal sealed class GarageSlotsPresenter : IInitialize, ITickable, ITickableBase
	{
		private const float MOUSEWHEEL_BASE_SPEED = 20f;

		private const string BAY_NUMBER_FORMAT = "D2";

		private const string CPU_NUMBER_FORMAT = "N0";

		private TiersData _tiersData;

		private int _lastScreenWidth = -1;

		private int _lastScreenHeight = -1;

		private int _slotsKnownAnchorWidth;

		private int _startGarageSlotIndexOnScreen = -1;

		private uint _currentGarageSlotIndex;

		private int _currentGarageSlotObjectIndex;

		private bool _lockSliderUpdate;

		private uint _currentGarageSlotId;

		private GarageView _garageView;

		private readonly List<GarageSlot> _garageSlots = new List<GarageSlot>();

		private FasterList<GarageSlotDependency> _garageSlotsDataList;

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ThumbnailManager thumbnailManager
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
		internal LoadingIconPresenter loadingPresenter
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
		internal ProfanityFilter profanityFilter
		{
			get;
			set;
		}

		[Inject]
		internal ICPUPower cpuPower
		{
			private get;
			set;
		}

		[Inject]
		internal GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		internal GarageSlotEdit garageSlotEdit
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			thumbnailManager.SetThumbnailReadyCallback(OnThumbnailReady);
			_lockSliderUpdate = true;
			_lockSliderUpdate = false;
		}

		public IEnumerator LoadData()
		{
			loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
			ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> task = request.AsTask();
			HandleTaskServiceWithError handleTSWithError2 = new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
			});
			yield return handleTSWithError2.GetEnumerator();
			loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
			if (!task.succeeded)
			{
				ErrorWindow.ShowServiceErrorWindow(task.behaviour);
			}
			loadingPresenter.NotifyLoading("LoadingTiersData");
			ILoadTiersBandingRequest loadTiersBandingReq = serviceFactory.Create<ILoadTiersBandingRequest>();
			TaskService<TiersData> loadTiersBandingTS = new TaskService<TiersData>(loadTiersBandingReq);
			handleTSWithError2 = new HandleTaskServiceWithError(loadTiersBandingTS, delegate
			{
				loadingPresenter.NotifyLoading("LoadingTiersData");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("LoadingTiersData");
			});
			yield return handleTSWithError2.GetEnumerator();
			loadingPresenter.NotifyLoadingDone("LoadingTiersData");
			if (loadTiersBandingTS.succeeded)
			{
				_tiersData = loadTiersBandingTS.result;
			}
		}

		public unsafe void RegisterView(GarageView garageView)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Expected O, but got Unknown
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Expected O, but got Unknown
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Expected O, but got Unknown
			_garageView = garageView;
			_garageView.slider.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_garageView.leftArrowButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_garageView.rightArrowButton.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		public void Tick(float deltaSec)
		{
			if (!(_garageView == null) && _tiersData != null && (Screen.get_width() != _lastScreenWidth || Screen.get_height() != _lastScreenHeight || _garageView.referenceElementForAnchoringGarageSlots.get_width() != _slotsKnownAnchorWidth))
			{
				_lastScreenWidth = Screen.get_width();
				_lastScreenHeight = Screen.get_height();
				_slotsKnownAnchorWidth = _garageView.referenceElementForAnchoringGarageSlots.get_width();
				RebuildSlots();
				SetupSlots();
				CalculateStartGarageSlotIndexOnScreen();
				int countOfSlotsToShowOnScreen = GetCountOfSlotsToShowOnScreen();
				ShowGarageSlots(countOfSlotsToShowOnScreen);
			}
		}

		public void SetCurrentSlotName(string newName)
		{
			bool flag = SocialInputValidation.DoesStringContainInvalidCharacters(ref newName);
			if (flag || profanityFilter.FilterString(newName) != newName)
			{
				string bodyText = (!flag) ? StringTableBase<StringTable>.Instance.GetString("strErrorRobotNameProfanityErrorBody") : StringTableBase<StringTable>.Instance.GetString("strUploadRobotInvalidCharError");
				ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strErrorRobotNameProfanityErrorTitle"), bodyText, StringTableBase<StringTable>.Instance.GetString("strOK")));
				_garageSlots[_currentGarageSlotObjectIndex].robotNameInput.set_value(GetCurrentRobotName());
			}
			else
			{
				garageSlotEdit.SetCurrentSlotName(newName);
			}
		}

		public void OnSelectRobotNameInputBox(GameObject gameObj, bool selected)
		{
			if (selected)
			{
				_garageSlots[_currentGarageSlotObjectIndex].robotNameInput.set_value(GetCurrentRobotName());
				guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			}
			else
			{
				guiInputController.SetShortCutMode(ShortCutMode.AllShortCuts);
			}
		}

		public void SwitchSlot(uint slotValue)
		{
			garageSlotEdit.SwitchSlot(slotValue);
		}

		public void PopulateSlots(FasterList<GarageSlotDependency> sortedSlotList, uint currentGarageSlotId, uint garageSlotLimit)
		{
			_garageSlotsDataList = sortedSlotList;
			_currentGarageSlotId = currentGarageSlotId;
			int num = SearchGarageSlotIndex(currentGarageSlotId);
			if (num == -1)
			{
				num = 0;
			}
			_currentGarageSlotIndex = (uint)num;
			int num2 = Mathf.Max(1, sortedSlotList.get_Count());
			SetScrollSpeed(20f / (float)num2);
			_lockSliderUpdate = true;
			SetupSlots();
			_lockSliderUpdate = false;
		}

		public void SetCurrentGarageData(uint currentGarageSlot, GarageSlotDependency garageSlot)
		{
			_currentGarageSlotId = currentGarageSlot;
			int num = (int)(_currentGarageSlotIndex = (uint)SearchGarageSlotIndex(currentGarageSlot));
			SetupSlots();
		}

		public void ReorderGarageSlots(FasterList<GarageSlotDependency> sortedSlotList, uint currentGarageSlotId)
		{
			_garageSlotsDataList = sortedSlotList;
			_currentGarageSlotId = currentGarageSlotId;
			int num = SearchGarageSlotIndex(currentGarageSlotId);
			int num2 = (int)_currentGarageSlotIndex - num;
			_currentGarageSlotIndex = (uint)num;
			_startGarageSlotIndexOnScreen -= num2;
			int num3 = Mathf.Max(1, sortedSlotList.get_Count() + 2);
			SetScrollSpeed(20f / (float)num3);
			_lockSliderUpdate = true;
			SetupSlots();
			_lockSliderUpdate = false;
		}

		private int SearchGarageSlotIndex(uint garageSlotId)
		{
			if (_garageSlotsDataList != null)
			{
				for (int i = 0; i < _garageSlotsDataList.get_Count(); i++)
				{
					if (garageSlotId == _garageSlotsDataList.get_Item(i).garageSlot)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private void RebuildSlots()
		{
			int slotsConfigurationForAspectRatio = GetSlotsConfigurationForAspectRatio();
			if (_garageSlots.Count < slotsConfigurationForAspectRatio)
			{
				int num = slotsConfigurationForAspectRatio - _garageSlots.Count;
				for (int i = 0; i < num; i++)
				{
					GameObject val = gameObjectFactory.Build(_garageView.garageSlotPrefab);
					val.get_transform().SetParent(_garageView.garageSlotParent);
					GarageSlot component = val.GetComponent<GarageSlot>();
					TaskRunner.get_Instance().Run(InitialiseSlot(component));
					_garageSlots.Add(component);
				}
			}
			else if (slotsConfigurationForAspectRatio < _garageSlots.Count)
			{
				int num2 = _garageSlots.Count - slotsConfigurationForAspectRatio;
				for (int j = 0; j < num2; j++)
				{
					Object.Destroy(_garageSlots[_garageSlots.Count - 1].get_gameObject());
					_garageSlots.RemoveAt(_garageSlots.Count - 1);
				}
			}
			UIWidget referenceElementForAnchoringGarageSlots = _garageView.referenceElementForAnchoringGarageSlots;
			UIWidget garageSlotTargetArea = _garageView.garageSlotPrefab.GetComponent<GarageSlot>().garageSlotTargetArea;
			float num3 = (float)garageSlotTargetArea.get_width() / (float)garageSlotTargetArea.get_height();
			float num4 = (float)referenceElementForAnchoringGarageSlots.get_height() * num3;
			float num5 = num4 * (float)slotsConfigurationForAspectRatio + (float)(_garageView.gapSizeInPixels * (slotsConfigurationForAspectRatio - 1));
			int num6 = (int)(num5 * 0.5f);
			referenceElementForAnchoringGarageSlots.leftAnchor.absolute = -num6;
			referenceElementForAnchoringGarageSlots.rightAnchor.absolute = num6;
		}

		private unsafe IEnumerator InitialiseSlot(GarageSlot slot)
		{
			slot.buttonBroadcaster.listener = _garageView.get_transform();
			slot.toggleButtonBroadcaster.listenerParent = _garageView.get_transform();
			slot.megabotTag.SetActive(false);
			yield return null;
			UIEventListener eventListener = slot.robotNameInput.GetComponent<UIEventListener>();
			eventListener.onSelect = new BoolDelegate((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			slot.robotNameInput.OnInputLoseFocus += delegate
			{
				OnRobotNameChange(slot);
			};
		}

		private void OnRobotNameChange(GarageSlot slot)
		{
			string value = slot.robotNameInput.get_value();
			value = value.Trim().Replace("<", string.Empty).Replace(">", string.Empty)
				.Replace("MOD", " ")
				.Replace("M0D", " ");
			value = value.Replace("|", string.Empty);
			if (value.Length > 0)
			{
				SetCurrentSlotName(value);
			}
			else
			{
				slot.robotNameInput.set_value(garagePresenter.CurrentRobotName);
			}
			slot.robotNameInput.set_isSelected(false);
		}

		private void CalculateStartGarageSlotIndexOnScreen()
		{
			if (_garageSlotsDataList == null)
			{
				return;
			}
			int count = _garageSlotsDataList.get_Count();
			int count2 = _garageSlots.Count;
			if (count2 != 0)
			{
				int num = 0;
				int num2 = count - count2;
				if (num2 < 0)
				{
					num2 = 0;
				}
				if (_startGarageSlotIndexOnScreen == -1)
				{
					int num3 = count2 / 2;
					_startGarageSlotIndexOnScreen = (int)_currentGarageSlotIndex - num3;
				}
				_startGarageSlotIndexOnScreen = Mathf.Clamp(_startGarageSlotIndexOnScreen, num, num2);
				if (_lockSliderUpdate)
				{
					float value = (float)(_startGarageSlotIndexOnScreen - num) / (float)(num2 - num);
					_garageView.slider.set_value(value);
					_garageView.slider.ForceUpdate();
				}
			}
		}

		private int GetCountOfSlotsToShowOnScreen()
		{
			if (_garageSlotsDataList == null)
			{
				return 0;
			}
			int count = _garageSlotsDataList.get_Count();
			int count2 = _garageSlots.Count;
			int num = 0;
			int num2 = count - count2;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (_startGarageSlotIndexOnScreen == -1)
			{
				int num3 = count2 / 2;
				_startGarageSlotIndexOnScreen = (int)_currentGarageSlotIndex - num3;
			}
			_startGarageSlotIndexOnScreen = Mathf.Clamp(_startGarageSlotIndexOnScreen, num, num2);
			int num4 = _garageSlotsDataList.get_Count() - _startGarageSlotIndexOnScreen;
			return Mathf.Min(num4, _garageSlots.Count);
		}

		private int GetSlotsConfigurationForAspectRatio()
		{
			float num = (float)Screen.get_width() / (float)Screen.get_height();
			int num2 = _garageView.configurations.Length;
			for (int i = 0; i < num2; i++)
			{
				GarageSlotsConfiguration garageSlotsConfiguration = _garageView.configurations[i];
				if (num < garageSlotsConfiguration.maxAspectRatio)
				{
					return garageSlotsConfiguration.numberOfSlots;
				}
			}
			return _garageView.configurations[num2 - 1].numberOfSlots;
		}

		private void ShowGarageSlots(int garageSlotToDisplayCount)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int i;
			for (i = 0; i < garageSlotToDisplayCount; i++)
			{
				_garageSlots[num].get_gameObject().SetActive(true);
				GarageSlot garageSlot = _garageSlots[num];
				AnchorSlot(garageSlot.garageSlotTargetArea, _garageView.referenceElementForAnchoringGarageSlots, i, _garageView.gapSizeInPixels);
				_garageSlots[num].get_gameObject().get_transform().set_localScale(Vector3.get_one());
				int num2 = _startGarageSlotIndexOnScreen + i;
				GarageSlotDependency garageSlotDependency = _garageSlotsDataList.get_Item(num2);
				bool flag = num2 == (int)_currentGarageSlotIndex;
				if (flag)
				{
					_currentGarageSlotObjectIndex = num;
				}
				SetSlotData(_garageSlots[num], garageSlotDependency, flag, num2, _tiersData);
				if (garageSlotDependency.starterRobotIndex >= 0)
				{
					GarageSlot slot = _garageSlots[num];
					Texture2D texture = _garageView.starterRobotThumbnails[garageSlotDependency.starterRobotIndex];
					ShowSlotThumbnail(slot, texture);
				}
				thumbnailManager.RequestThumbnailTexture(garageSlotDependency.uniqueSlotId);
				num++;
				num %= _garageSlots.Count;
			}
			HideInactiveGarageSlots(i, num);
		}

		private void HideInactiveGarageSlots(int currentSlotPositionOnScreen, int garageSlotObjectIndex)
		{
			while (currentSlotPositionOnScreen < _garageSlots.Count)
			{
				_garageSlots[garageSlotObjectIndex].get_gameObject().SetActive(false);
				garageSlotObjectIndex++;
				garageSlotObjectIndex %= _garageSlots.Count;
				currentSlotPositionOnScreen++;
			}
		}

		private static void AnchorSlot(UIWidget slot, UIWidget container, int index, int spacingBetweenElements)
		{
			slot.topAnchor.Set(container.get_transform(), 1f, 0f);
			slot.bottomAnchor.Set(container.get_transform(), 0f, 0f);
			float num = (float)slot.get_width() / (float)slot.get_height();
			float num2 = (float)container.get_height() * num;
			slot.leftAnchor.Set(container.get_transform(), 0f, num2 * (float)index + (float)(spacingBetweenElements * index));
			slot.rightAnchor.Set(container.get_transform(), 0f, num2 + num2 * (float)index + (float)(spacingBetweenElements * index));
			slot.SetDirty();
			slot.ResizeCollider();
		}

		private void SetSlotData(GarageSlot slot, GarageSlotDependency dependency, bool isSelected, int slotIndex, TiersData tiersData)
		{
			slot.slotId = dependency.garageSlot;
			string text = (slotIndex + 1).ToString("D2");
			slot.smallLabel.set_text(text);
			slot.smallLabelTitle.set_text(text);
			slot.smallLabelTitle.get_gameObject().SetActive(true);
			slot.smallThumbnail.get_gameObject().SetActive(false);
			slot.buttonBroadcaster.sendValue = slot.slotId;
			slot.robotNameLabel.set_text(dependency.name);
			slot.robotNameInput.set_value(dependency.name);
			uint num = RobotCPUCalculator.CalculateRobotActualCPU(dependency.totalRobotCPU, dependency.totalCosmeticCPU, cpuPower.MaxCosmeticCpuPool);
			slot.robotCpuLabel.set_text(num.ToString("N0", CultureInfo.InvariantCulture));
			bool flag = num > cpuPower.MaxCpuPower;
			slot.megabotTag.SetActive(flag);
			slot.selectedSettingsRoot.SetActive(isSelected);
			slot.editRobotNameGO.SetActive(true);
			slot.readOnlyGarageTag.SetActive(false);
			slot.normalGarageTag.SetActive(true);
			slot.editReadOnlyRobotNameLabel.set_text(dependency.name);
			slot.editReadOnlyRobotGO.SetActive(false);
			string text2 = RRAndTiers.ConvertRobotRankingToTierString(dependency.totalRobotRanking, flag, tiersData);
			slot.garageSlotRobotTier.set_text(text2);
		}

		private void SetupSlots()
		{
			if (_garageSlotsDataList != null && _garageSlotsDataList.get_Count() != 0)
			{
				CalculateStartGarageSlotIndexOnScreen();
				int countOfSlotsToShowOnScreen = GetCountOfSlotsToShowOnScreen();
				ShowGarageSlots(countOfSlotsToShowOnScreen);
				ShowArrows();
			}
		}

		private void ShowArrows()
		{
			int count = _garageSlotsDataList.get_Count();
			int count2 = _garageSlots.Count;
			bool isEnabled = _startGarageSlotIndexOnScreen != 0;
			bool isEnabled2 = count - _startGarageSlotIndexOnScreen > count2;
			if (count2 < count)
			{
				_garageView.leftArrowButton.set_isEnabled(isEnabled);
				_garageView.leftArrowButton.get_gameObject().SetActive(true);
				_garageView.slider.get_gameObject().SetActive(true);
				_garageView.rightArrowButton.set_isEnabled(isEnabled2);
				_garageView.rightArrowButton.get_gameObject().SetActive(true);
			}
			else
			{
				_garageView.leftArrowButton.get_gameObject().SetActive(false);
				_garageView.slider.get_gameObject().SetActive(false);
				_garageView.rightArrowButton.get_gameObject().SetActive(false);
			}
		}

		private void SlidePanel()
		{
			if (!_lockSliderUpdate)
			{
				int count = _garageSlots.Count;
				int num = 0;
				int num2 = _garageSlotsDataList.get_Count() - count;
				float num3 = (float)num + UIProgressBar.current.get_value() * (float)num2;
				int num4 = _startGarageSlotIndexOnScreen = Mathf.RoundToInt(num3);
				SetupSlots();
			}
		}

		private void SelectPrevious()
		{
			_lockSliderUpdate = true;
			if (0 < _startGarageSlotIndexOnScreen)
			{
				_startGarageSlotIndexOnScreen--;
			}
			SetupSlots();
			_lockSliderUpdate = false;
		}

		private void SelectNext()
		{
			_lockSliderUpdate = true;
			if (_startGarageSlotIndexOnScreen < _garageSlotsDataList.get_Count() - 1)
			{
				_startGarageSlotIndexOnScreen++;
			}
			SetupSlots();
			_lockSliderUpdate = false;
		}

		private void OnThumbnailReady(UniqueSlotIdentifier identifier, Texture2D thumbnailTexture)
		{
			if (thumbnailTexture == null)
			{
				return;
			}
			int countOfSlotsToShowOnScreen = GetCountOfSlotsToShowOnScreen();
			for (int i = 0; i < countOfSlotsToShowOnScreen; i++)
			{
				GetGarageSlotIndexesForScreenSlotPosition(i, out int garageSlotObjectIndexFound, out int garageSlotIndexFound);
				if (_garageSlotsDataList.get_Item(garageSlotIndexFound).uniqueSlotId.Equals(identifier))
				{
					GarageSlot slot = _garageSlots[garageSlotObjectIndexFound];
					ShowSlotThumbnail(slot, thumbnailTexture);
				}
			}
		}

		private void GetGarageSlotIndexesForScreenSlotPosition(int screenSlotPosition, out int garageSlotObjectIndexFound, out int garageSlotIndexFound)
		{
			garageSlotObjectIndexFound = -1;
			garageSlotIndexFound = -1;
			int count = _garageSlots.Count;
			int num = 0;
			int num2 = _garageSlotsDataList.get_Count() - count;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (_startGarageSlotIndexOnScreen == -1)
			{
				int num3 = count / 2;
				_startGarageSlotIndexOnScreen = (int)_currentGarageSlotIndex - num3;
			}
			_startGarageSlotIndexOnScreen = Mathf.Clamp(_startGarageSlotIndexOnScreen, num, num2);
			int countOfSlotsToShowOnScreen = GetCountOfSlotsToShowOnScreen();
			int num4 = 0;
			int num5 = 0;
			int num6;
			while (true)
			{
				if (num5 < countOfSlotsToShowOnScreen)
				{
					num6 = _startGarageSlotIndexOnScreen + num5;
					if (screenSlotPosition == num5)
					{
						break;
					}
					num4++;
					num4 %= _garageSlots.Count;
					num5++;
					continue;
				}
				return;
			}
			garageSlotIndexFound = num6;
			garageSlotObjectIndexFound = num4;
		}

		private static void ShowSlotThumbnail(GarageSlot slot, Texture2D texture)
		{
			slot.smallThumbnail.get_gameObject().SetActive(true);
			slot.smallThumbnail.set_mainTexture(texture);
			slot.smallLabelTitle.get_gameObject().SetActive(false);
		}

		private string GetCurrentRobotName()
		{
			string empty = string.Empty;
			for (int i = 0; i < _garageSlotsDataList.get_Count(); i++)
			{
				if (_garageSlotsDataList.get_Item(i).garageSlot == _currentGarageSlotId)
				{
					return _garageSlotsDataList.get_Item(i).name;
				}
			}
			return empty;
		}

		public uint GetCurrentBayViewIndex()
		{
			return _currentGarageSlotIndex;
		}

		public void ShowSlots()
		{
			_startGarageSlotIndexOnScreen = -1;
			_garageView.get_gameObject().SetActive(true);
			TaskRunner.get_Instance().Run((Func<IEnumerator>)InitialiseScrollBar);
		}

		private IEnumerator InitialiseScrollBar()
		{
			yield return null;
			_garageView.slider.get_foregroundWidget().set_enabled(true);
			yield return null;
			_garageView.slider.ForceUpdate();
		}

		public void HideSlots()
		{
			_garageView.get_gameObject().SetActive(false);
		}

		private void SetScrollSpeed(float speed)
		{
			_garageView.mouseWheelScroller.scrollSpeed = speed;
			_garageView.scroller.scrollSpeed = speed;
		}
	}
}
