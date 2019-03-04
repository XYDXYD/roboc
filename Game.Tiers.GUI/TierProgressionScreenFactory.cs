using Game.ECS.GUI.Implementors;
using Mothership.GUI;
using Robocraft.GUI.Iteration2;
using Services.Web.Photon;
using Simulation;
using Svelto.Factories;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Tiers.GUI
{
	internal static class TierProgressionScreenFactory
	{
		public unsafe static IEnumerator Build(IGameObjectFactory goFactory, IServiceRequestFactory requestFactory, LoadingIconPresenter loadingIcon, IGUIInputController guiInputController)
		{
			GameObject val = goFactory.Build("GUI_TierProgression");
			TierProgressionScreen screen = val.GetComponent<TierProgressionScreen>();
			screen.Initialize(val.GetInstanceID());
			GUIDisplayImplementor gUIDisplayImplementor = new GUIDisplayImplementor(GuiScreens.LeagueScreen, HudStyle.HideAll, doesntHideOnSwitch: false, hasBackground: false, isScreenBlurred: false, ShortCutMode.OnlyGUINoSwitching, TopBarStyle.FullScreenInterface, screen.isShown);
			guiInputController.AddDisplayScreens(new IGUIDisplay[1]
			{
				gUIDisplayImplementor
			});
			_003CBuild_003Ec__Iterator0._003CBuild_003Ec__AnonStorey1 _003CBuild_003Ec__AnonStorey;
			EventDelegate.Add(screen.backButton.onClick, new Callback((object)_003CBuild_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			IGetTierProgressRequest request = requestFactory.Create<IGetTierProgressRequest>();
			TaskService<TierProgress[]> task = new TaskService<TierProgress[]>(request);
			loadingIcon.NotifyLoading("LoadingTiersData");
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingIcon.NotifyLoading("LoadingTiersData");
			}, delegate
			{
				loadingIcon.NotifyLoadingDone("LoadingTiersData");
			}).GetEnumerator();
			loadingIcon.NotifyLoadingDone("LoadingTiersData");
			TierProgress[] tiers = task.result;
			for (int i = 0; i < tiers.Length; i++)
			{
				GameObject val2 = GenericWidgetFactory.InstantiateGui(screen.tierWidgetTemplate, screen.tierWidgetTemplate.get_transform().get_parent());
				TierProgressionWidget component = val2.GetComponent<TierProgressionWidget>();
				component.tier = i;
				component.tierString = StringTableBase<StringTable>.Instance.GetString("strTier") + " " + (uint)(i + 1);
				component.rank = tiers[i].rank;
				component.rankString = StringTableBase<StringTable>.Instance.GetReplaceString("strCurrentRank", "{rank}", RRAndTiers.GetRankDisplayableName(tiers[i].rank));
				component.progressInRank = tiers[i].progressInRank;
			}
			screen.tierWidgetTemplate.SetActive(false);
			screen.layoutTable.Reposition();
			screen.get_gameObject().SetActive(false);
		}
	}
}
