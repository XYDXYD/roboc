using Mothership.Garage.Thumbnail;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class RobotShopRatingController
	{
		private RateYourRobotPopup _rateYourRobotPopup;

		[Inject]
		internal GaragePresenter garage
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
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal RobotThumbnailFetcher thumbnailsFetcher
		{
			private get;
			set;
		}

		public void SetupRateYourRobotPopup(RateYourRobotPopup rateYourRobotPopup)
		{
			_rateYourRobotPopup = rateYourRobotPopup;
			_rateYourRobotPopup.OnRatingConfirmedEvent += OnRatingConfirmed;
		}

		public bool IsActive()
		{
			return _rateYourRobotPopup.get_gameObject().get_activeSelf();
		}

		public IEnumerator ConditionalShowRateYourRobotPopup()
		{
			if (garage.CurrentSlotCanBeRated)
			{
				string thumbnailName = ThumbnailManager.GetGarageThumbnailFileName(garage.CurrentRobotIdentifier.ToString());
				thumbnailsFetcher.LoadTexture(thumbnailName, delegate(Texture2D t)
				{
					_rateYourRobotPopup.ShowThumbnail(t);
				}, ThumbnailType.Garage);
				_rateYourRobotPopup.Show(garage.CurrentRobotName);
				while (IsActive())
				{
					yield return null;
				}
			}
		}

		private void OnRatingConfirmed(int combatRating, int styleRating)
		{
			SubmitCRFRatingDependency submitCRFRatingDependency = new SubmitCRFRatingDependency();
			submitCRFRatingDependency.slotId = Convert.ToInt32(garage.currentGarageSlot);
			submitCRFRatingDependency.combatRating = combatRating;
			submitCRFRatingDependency.cosmeticRating = styleRating;
			loadingPresenter.NotifyLoading("ConfirmRating");
			ISubmitCRFRatingRequest submitCRFRatingRequest = serviceFactory.Create<ISubmitCRFRatingRequest, SubmitCRFRatingDependency>(submitCRFRatingDependency);
			submitCRFRatingRequest.SetAnswer(new ServiceAnswer(OnRatingUploaded, OnRatingFailed));
			submitCRFRatingRequest.Execute();
		}

		private void OnRatingUploaded()
		{
			SetHasRated();
		}

		private void OnRatingFailed(ServiceBehaviour s)
		{
			SetHasRated();
		}

		private void SetHasRated()
		{
			garage.CurrentSlotCanBeRated = false;
			loadingPresenter.NotifyLoadingDone("ConfirmRating");
		}
	}
}
