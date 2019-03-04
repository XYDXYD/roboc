using Mothership.GUI;
using Simulation;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Utility;

namespace Mothership
{
	internal sealed class AwardedItemsController : IGUIDisplay, IWaitForFrameworkDestruction, IComponent
	{
		public enum AwardedItemsAnimationType
		{
			ScreenEntry,
			CubeAppear,
			CubeDisappear
		}

		private struct CubeAnimationSequenceElement
		{
			public AwardedItemsAnimationType animType;

			public CubeTypeID cubeType;

			public uint cubeCount;

			public CubeTypeData cubeTypeData;

			public string cubeSpriteName;
		}

		private class AwardedCubeGUIItemEntityDescriptor : GenericEntityDescriptor<StatsHintPopupAreaEntityView>
		{
		}

		private AwardedItemsScreen _awardedItemsScreen;

		private int _currentAnimationElement;

		private List<CubeAnimationSequenceElement> _animationSequenceList = new List<CubeAnimationSequenceElement>();

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeList cubeList
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
		internal IEntityFactory entityFactory
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
		public ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.AwardedItemsScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public bool isReady => _awardedItemsScreen != null;

		public void EnableBackground(bool enable)
		{
		}

		public void SetScreen(AwardedItemsScreen screen)
		{
			_awardedItemsScreen = screen;
			StatsHintPopupAreaImplementor hintPopupArea = _awardedItemsScreen.hintPopupArea;
			int instanceID = hintPopupArea.get_gameObject().GetInstanceID();
			hintPopupArea.Initialize(instanceID);
			entityFactory.BuildEntity<AwardedCubeGUIItemEntityDescriptor>(instanceID, new object[1]
			{
				hintPopupArea
			});
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_awardedItemsScreen = null;
		}

		public IEnumerator GetAwardedCubeDataAndShowScreen()
		{
			loadingIconPresenter.NotifyLoading("CheckingCubeRewards");
			TaskService<string[]> requestTask = serviceFactory.Create<ICheckCubeRewardsRequest>().AsTask();
			yield return requestTask;
			loadingIconPresenter.NotifyLoadingDone("CheckingCubeRewards");
			if (!requestTask.succeeded)
			{
				RemoteLogger.Error("CheckCubeRewardsRequest failed", requestTask.behaviour.errorBody, requestTask.behaviour.exceptionThrown.StackTrace);
			}
			Dictionary<string, object> cubesAwarded = new Dictionary<string, object>();
			for (int i = 0; i < requestTask.result.Length; i++)
			{
				cubesAwarded.Add(requestTask.result[i], 1);
			}
			if (cubesAwarded.Count > 0)
			{
				yield return ShowScreenAndWaitUntilFinished(cubesAwarded);
			}
		}

		public IEnumerator ShowScreenAndWaitUntilFinished(Dictionary<string, object> cubesAwarded)
		{
			BuildAnimationSequenceFromCubesReceived(cubesAwarded);
			guiInputController.ShowScreen(GuiScreens.AwardedItemsScreen);
			while (IsActive())
			{
				yield return null;
			}
			yield return cubeInventory.RefreshAndWait();
			guiInputController.CloseCurrentScreen();
		}

		private void BuildAnimationSequenceFromCubesReceived(Dictionary<string, object> cubesAwarded)
		{
			_animationSequenceList.Add(new CubeAnimationSequenceElement
			{
				animType = AwardedItemsAnimationType.ScreenEntry
			});
			int num = 0;
			foreach (KeyValuePair<string, object> item3 in cubesAwarded)
			{
				uint id = 0u;
				try
				{
					id = uint.Parse(item3.Key, NumberStyles.AllowHexSpecifier);
				}
				catch (Exception)
				{
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strErrorConvertingCubeTypeInAwardedItemsScreenHdr"), StringTableBase<StringTable>.Instance.GetString("strErrorConvertingCubeTypeInAwardedItemsScreenBody")));
				}
				CubeTypeID cubeTypeID = new CubeTypeID(id);
				CubeTypeData cubeTypeData = cubeList.CubeTypeDataOf(cubeTypeID);
				string cubeSpriteName = (!cubeTypeData.cubeData.localiseSprite) ? cubeTypeData.spriteName : StringTableBase<StringTable>.Instance.GetString(cubeTypeData.spriteName);
				CubeAnimationSequenceElement cubeAnimationSequenceElement = default(CubeAnimationSequenceElement);
				cubeAnimationSequenceElement.animType = AwardedItemsAnimationType.CubeAppear;
				cubeAnimationSequenceElement.cubeType = cubeTypeID;
				cubeAnimationSequenceElement.cubeCount = Convert.ToUInt32(item3.Value);
				cubeAnimationSequenceElement.cubeTypeData = cubeTypeData;
				cubeAnimationSequenceElement.cubeSpriteName = cubeSpriteName;
				CubeAnimationSequenceElement item = cubeAnimationSequenceElement;
				_animationSequenceList.Add(item);
				if (num < cubesAwarded.Count - 1)
				{
					cubeAnimationSequenceElement = default(CubeAnimationSequenceElement);
					cubeAnimationSequenceElement.animType = AwardedItemsAnimationType.CubeDisappear;
					cubeAnimationSequenceElement.cubeType = cubeTypeID;
					cubeAnimationSequenceElement.cubeCount = Convert.ToUInt32(item3.Value);
					cubeAnimationSequenceElement.cubeTypeData = cubeTypeData;
					cubeAnimationSequenceElement.cubeSpriteName = cubeSpriteName;
					CubeAnimationSequenceElement item2 = cubeAnimationSequenceElement;
					_animationSequenceList.Add(item2);
				}
				num++;
			}
			Console.Log(FastConcatUtility.FastConcat<string>("Awarded cubes to view: ", _currentAnimationElement.ToString()));
		}

		public GUIShowResult Show()
		{
			_awardedItemsScreen.Show();
			_currentAnimationElement = 0;
			PlayCurrentAnimation();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			return _awardedItemsScreen.Hide();
		}

		public bool IsActive()
		{
			return _awardedItemsScreen != null && _awardedItemsScreen.IsActive();
		}

		public void ScreenDisappearAnimationEnd()
		{
			guiInputController.CloseCurrentScreen();
		}

		private void PlayCurrentAnimation()
		{
			CubeAnimationSequenceElement cubeAnimationSequenceElement = _animationSequenceList[_currentAnimationElement];
			switch (cubeAnimationSequenceElement.animType)
			{
			case AwardedItemsAnimationType.CubeAppear:
			{
				CubeAnimationSequenceElement cubeAnimationSequenceElement2 = _animationSequenceList[_currentAnimationElement];
				_awardedItemsScreen.PlayCubeAppearAnimation(cubeAnimationSequenceElement2.cubeType, cubeAnimationSequenceElement2.cubeTypeData.nameStrKey, cubeAnimationSequenceElement2.cubeSpriteName, cubeAnimationSequenceElement2.cubeCount, HandleAnimationCompleteCallback);
				break;
			}
			case AwardedItemsAnimationType.CubeDisappear:
				_awardedItemsScreen.PlayCubeDisappearAnimation(HandleAnimationCompleteCallback);
				break;
			case AwardedItemsAnimationType.ScreenEntry:
				_awardedItemsScreen.PlayScreenAppearAnimation(HandleAnimationCompleteCallback);
				break;
			}
		}

		private void HandleAnimationCompleteCallback()
		{
			_currentAnimationElement++;
			if (_currentAnimationElement == _animationSequenceList.Count)
			{
				_animationSequenceList.Clear();
				_currentAnimationElement = 0;
				ShowButtonAndWaitToExit();
			}
			else
			{
				PlayCurrentAnimation();
			}
		}

		private void ShowButtonAndWaitToExit()
		{
			_awardedItemsScreen.ShowCollectButton();
		}
	}
}
