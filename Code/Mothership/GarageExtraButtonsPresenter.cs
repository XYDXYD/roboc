using rail;
using Services.Requests.Interfaces;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal sealed class GarageExtraButtonsPresenter : IInitialize
	{
		private string _feedbackURL;

		private string _supportURL;

		private string _wikiURL;

		[Inject]
		private IServiceRequestFactory serviceRequestFactory
		{
			get;
			set;
		}

		public GarageExtraButtonsView view
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadURLs);
		}

		private IEnumerator LoadURLs()
		{
			ILoadPlatformConfigurationRequest loadPlatformConfigurationReq = serviceRequestFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> task = loadPlatformConfigurationReq.AsTask();
			yield return new HandleTaskServiceWithError(task, null, null).GetEnumerator();
			PlatformConfigurationSettings platformConfig = task.result;
			_feedbackURL = platformConfig.FeedbackURL;
			_supportURL = platformConfig.SupportURL;
			_wikiURL = platformConfig.WikiURL;
		}

		public void Show()
		{
			view.Show();
		}

		public void OnWikiButtonClicked()
		{
			Application.OpenURL(_wikiURL);
		}

		public void OnFeedbackButtonClicked()
		{
			Application.OpenURL(_feedbackURL);
		}

		public void OnCustomerServiceButtonClicked()
		{
			Application.OpenURL(_supportURL);
		}

		public void OnAchievementsButtonClicked()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			IRailFactory val = rail_api.RailFactory();
			IRailFloatingWindow val2 = val.RailFloatingWindow();
			val2.AsyncShowRailFloatingWindow(15, string.Empty);
		}
	}
}
