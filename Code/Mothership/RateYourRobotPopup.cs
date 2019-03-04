using Fabric;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class RateYourRobotPopup : MonoBehaviour, IInitialize, IChainListener
	{
		public List<UISprite> CombatStars;

		public List<UISprite> StyleStars;

		public UILabel NameLabel;

		public UITexture Thumbnail;

		private RatingState _CombatRatingState;

		private RatingState _StyleRatingState;

		public Color ratedColor = new Color(84f / 85f, 131f / 255f, 4f / 51f);

		public Color unRatedColor = Color.get_black();

		[Inject]
		internal RobotShopRatingController ratingController
		{
			private get;
			set;
		}

		public event Action<int, int> OnRatingConfirmedEvent;

		public RateYourRobotPopup()
			: this()
		{
		}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)


		void IInitialize.OnDependenciesInjected()
		{
			this.get_gameObject().SetActive(false);
			ratingController.SetupRateYourRobotPopup(this);
			_CombatRatingState = new RatingState();
			_CombatRatingState.stars = CombatStars;
			_StyleRatingState = new RatingState();
			_StyleRatingState.stars = StyleStars;
			SetupEventListeners(_CombatRatingState);
			SetupEventListeners(_StyleRatingState);
		}

		private unsafe void SetupEventListeners(RatingState ratingState)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Expected O, but got Unknown
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			_003CSetupEventListeners_003Ec__AnonStorey0 _003CSetupEventListeners_003Ec__AnonStorey;
			for (int i = 0; i < ratingState.stars.Count; i++)
			{
				UIEventListener component = ratingState.stars[i].GetComponent<UIEventListener>();
				UIEventListener obj = component;
				obj.onHover = Delegate.Combine((Delegate)obj.onHover, (Delegate)new BoolDelegate((object)_003CSetupEventListeners_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
				UIEventListener obj2 = component;
				obj2.onClick = Delegate.Combine((Delegate)obj2.onClick, (Delegate)new VoidDelegate((object)_003CSetupEventListeners_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		private void OnHoverStar(RatingState ratingState, GameObject starObject, bool hover)
		{
			if (!hover)
			{
				SetRating(ratingState, ratingState.validatedRating);
				return;
			}
			int num = -1;
			for (int i = 0; i < ratingState.stars.Count; i++)
			{
				if (ratingState.stars[i].get_gameObject() == starObject)
				{
					num = i + 1;
					break;
				}
			}
			if (num < 0)
			{
				Console.LogError("[RateYourRobotPopup] Wrong stars configuration - check game object stars");
				return;
			}
			ratingState.hoverRating = num;
			SetRating(ratingState, num);
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.GUI_KubeMenuHover_Top));
		}

		private void OnClickStar(RatingState ratingState, GameObject starObject)
		{
			ratingState.validatedRating = ratingState.hoverRating;
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
		}

		private void SetRating(RatingState ratingState, int rating)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			int i;
			for (i = 0; i < rating; i++)
			{
				ratingState.stars[i].set_color(ratedColor);
			}
			for (; i < ratingState.stars.Count; i++)
			{
				ratingState.stars[i].set_color(unRatedColor);
			}
		}

		public void Show(string robotName)
		{
			NameLabel.set_text(robotName);
			this.get_gameObject().SetActive(true);
			OnHoverStar(_CombatRatingState, _CombatRatingState.stars[2].get_gameObject(), hover: true);
			OnClickStar(_CombatRatingState, _CombatRatingState.stars[2].get_gameObject());
			OnHoverStar(_StyleRatingState, _StyleRatingState.stars[2].get_gameObject(), hover: true);
			OnClickStar(_StyleRatingState, _StyleRatingState.stars[2].get_gameObject());
		}

		public void ShowThumbnail(Texture2D thumbnail)
		{
			Thumbnail.set_mainTexture(thumbnail);
		}

		public void Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			ButtonType buttonType = (ButtonType)Convert.ToInt32(message);
			if (buttonType == ButtonType.Confirm)
			{
				if (this.OnRatingConfirmedEvent != null)
				{
					this.OnRatingConfirmedEvent(_CombatRatingState.validatedRating, _StyleRatingState.validatedRating);
				}
				this.get_gameObject().SetActive(false);
			}
		}
	}
}
